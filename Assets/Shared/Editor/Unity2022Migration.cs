using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FXR.Editor
{
    // One-time compatibility preparation. Does not regenerate any room or scene layout.
    public static class Unity2022Migration
    {
        static readonly string[] Scenes = {
            "Assets/Shared/Scenes/MainScene.unity",
            "Assets/Rooms/Memory/Scenes/MemoryDev.unity",
            "Assets/Rooms/RoomB/Scenes/RoomBDev.unity",
            "Assets/Rooms/RoomC/Scenes/RoomCDev.unity"
        };

        public static void Prepare()
        {
            if (Application.unityVersion != "2022.3.62f3")
                throw new Exception("Migration must run in Unity 2022.3.62f3.");
            foreach (string name in new[] { "Mobile", "PC" })
            {
                string rendererPath = "Assets/Settings/" + name + "_Renderer.asset";
                var renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                renderer.name = name + "_Renderer";
                renderer.useNativeRenderPass = false;
                var savedRenderer = ReplaceContents(rendererPath, renderer);
                var pipeline = UniversalRenderPipelineAsset.Create(savedRenderer);
                pipeline.name = name + "_RPAsset";
                pipeline.supportsHDR = false;
                pipeline.msaaSampleCount = 4;
                pipeline.renderScale = 1;
                pipeline.supportsCameraDepthTexture = false;
                pipeline.supportsCameraOpaqueTexture = false;
                pipeline.maxAdditionalLightsCount = 2;
                pipeline.shadowDistance = 12;
                var settings = new SerializedObject(pipeline);
                settings.FindProperty("m_MainLightShadowmapResolution").intValue = 1024;
                settings.FindProperty("m_AdditionalLightShadowsSupported").boolValue = false;
                settings.FindProperty("m_SoftShadowsSupported").boolValue = false;
                settings.ApplyModifiedPropertiesWithoutUndo();
                ReplaceContents("Assets/Settings/" + name + "_RPAsset.asset", pipeline);
            }
            // URP 14 keeps this settings type internal; create its native schema
            // through the pipeline assembly, retaining the existing asset GUID.
            var globalType = typeof(UniversalRenderPipelineAsset).Assembly.GetType(
                "UnityEngine.Rendering.Universal.UniversalRenderPipelineGlobalSettings", true);
            var global = ScriptableObject.CreateInstance(globalType);
            ReplaceContents("Assets/Settings/UniversalRenderPipelineGlobalSettings.asset", global);
            PrototypeBuilder.ConfigureProject();
            RepairFontMaterials();
            AssetDatabase.SaveAssets();
            // Write native Unity 2022 schemas, dropping unsupported Unity 6 fields.
            var serialized = AssetDatabase.GetAllAssetPaths().Where(p =>
                p.StartsWith("Assets/Settings/") ||
                p.StartsWith("Assets/Rooms/Memory/") &&
                (p.EndsWith(".asset") || p.EndsWith(".mat") || p.EndsWith(".prefab")));
            AssetDatabase.ForceReserializeAssets(serialized.ToArray());
            foreach (string path in Scenes)
            {
                var scene = EditorSceneManager.OpenScene(path);
                EditorSceneManager.SaveScene(scene);
            }
            ValidateReferences();
            AssetDatabase.SaveAssets();
            Debug.Log("Unity 2022 migration preparation completed; room layouts and asset GUIDs retained.");
        }

        static T ReplaceContents<T>(string path, T fresh) where T : UnityEngine.Object
        {
            var saved = AssetDatabase.LoadAssetAtPath<T>(path);
            if (saved == null) throw new Exception("Existing migration asset could not load: " + path);
            EditorUtility.CopySerialized(fresh, saved);
            EditorUtility.SetDirty(saved);
            UnityEngine.Object.DestroyImmediate(fresh);
            return saved;
        }

        public static void RepairFontMaterials()
        {
            // Unity 6 serialized TMP_Asset.m_Material; TMP 3 uses the public
            // material field. Preserve the embedded material and its file ID.
            foreach (var guid in AssetDatabase.FindAssets("t:TMP_FontAsset"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.StartsWith("Assets/")) continue;
                var font = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(path);
                if (font.material == null)
                {
                    font.material = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Material>().FirstOrDefault();
                    if (font.material == null) throw new Exception("Font material missing: " + path);
                }
                EditorUtility.SetDirty(font);
                Debug.Log("Font material retained: " + font.name + " / " + font.material.name);
            }
            QuestReadiness.ClearDynamicFontCaches();
            AssetDatabase.SaveAssets();
            AssetDatabase.ForceReserializeAssets(AssetDatabase.FindAssets("t:TMP_FontAsset")
                .Select(AssetDatabase.GUIDToAssetPath).Where(p => p.StartsWith("Assets/")).ToArray());
            ValidateReferences();
        }

        [MenuItem("FXR/Check project references")]
        public static void ValidateReferences()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            var previousScenes = EditorSceneManager.GetSceneManagerSetup();
            var report = new StringBuilder("Reference check: Unity " + Application.unityVersion + "\n");
            int errors = 0;
            try
            {
            foreach (string path in Scenes)
            {
                var scene = EditorSceneManager.OpenScene(path);
                var objects = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<Transform>(true)).ToArray();
                int missingScripts = objects.Sum(x => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(x.gameObject));
                // TMP creates its text mesh at runtime. Empty source material groups
                // are valid no-op meshes; report them separately from lost references.
                var meshes = objects.SelectMany(x => x.GetComponents<MeshFilter>())
                    .Where(x => x.GetComponent<TMPro.TMP_Text>() == null).ToArray();
                int missingMeshes = meshes.Count(x => x.sharedMesh == null);
                int unusedMeshes = meshes.Count(x => x.sharedMesh != null && x.sharedMesh.vertexCount == 0);
                var renderers = objects.SelectMany(x => x.GetComponents<Renderer>()).ToArray();
                int missingMaterials = renderers.Sum(x => x.sharedMaterials.Count(m => m == null || m.shader == null || m.shader.name == "Hidden/InternalErrorShader"));
                var ambience = objects.SelectMany(x => x.GetComponents<FXR.Memory.MemoryAtmosphere>())
                    .SelectMany(x => x.officeSources.Concat(x.natureSources)).ToArray();
                int missingAmbience = ambience.Count(x => x == null || x.clip == null || x.clip.length <= 0);
                var texts = objects.SelectMany(x => x.GetComponents<TMPro.TMP_Text>()).ToArray();
                int missingFonts = texts.Count(x => x.font == null || x.font.material == null || x.fontSharedMaterial == null);
                report.AppendLine(path + ": " + objects.Length + " objects, " + meshes.Length + " meshes; missing scripts=" + missingScripts +
                    ", missing meshes=" + missingMeshes + ", invalid materials=" + missingMaterials + ", unused source mesh groups=" + unusedMeshes + ", missing ambience clips=" + missingAmbience + ", missing font/material references=" + missingFonts);
                foreach (var audio in ambience.Where(x => x != null && x.clip != null))
                    report.AppendLine("  Ambience: " + audio.clip.name + " / " + audio.clip.length.ToString("F1") + " seconds");
                foreach (var mesh in meshes.Where(x => x.sharedMesh == null))
                    report.AppendLine("  Missing mesh: " + HierarchyPath(mesh.transform));
                errors += missingScripts + missingMeshes + missingMaterials + missingAmbience + missingFonts;
            }
            Directory.CreateDirectory("Builds/Review");
            File.WriteAllText("Builds/Review/2022-reference-check.txt", report.ToString());
            if (errors != 0) throw new Exception(report.ToString());
            Debug.Log(report.ToString());
            }
            finally
            {
                if (Application.isBatchMode) EditorSceneManager.OpenScene(Scenes[0]);
                else EditorSceneManager.RestoreSceneManagerSetup(previousScenes);
            }
        }

        static string HierarchyPath(Transform transform) => transform.parent == null ? transform.name : HierarchyPath(transform.parent) + "/" + transform.name;

        [MenuItem("FXR/Open Memory development scene")]
        public static void OpenMemoryDevelopment()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(DevelopmentSceneBuilder.MemoryScene);
        }
    }
}
