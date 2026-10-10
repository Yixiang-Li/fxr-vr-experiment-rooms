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
                pipeline.mainLightShadowmapResolution = 1024;
                var settings = new SerializedObject(pipeline);
                settings.FindProperty("m_AdditionalLightShadowsSupported").boolValue = false;
                settings.FindProperty("m_SoftShadowsSupported").boolValue = false;
                settings.ApplyModifiedPropertiesWithoutUndo();
                ReplaceContents("Assets/Settings/" + name + "_RPAsset.asset", pipeline);
            }
            var global = ScriptableObject.CreateInstance<UniversalRenderPipelineGlobalSettings>();
            ReplaceContents("Assets/Settings/UniversalRenderPipelineGlobalSettings.asset", global);
            PrototypeBuilder.ConfigureProject();
            QuestReadiness.ClearDynamicFontCaches();
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

        [MenuItem("FXR/Check project references")]
        public static void ValidateReferences()
        {
            var report = new StringBuilder("Reference check: Unity " + Application.unityVersion + "\n");
            foreach (string path in Scenes)
            {
                var scene = EditorSceneManager.OpenScene(path);
                var objects = scene.GetRootGameObjects().SelectMany(x => x.GetComponentsInChildren<Transform>(true)).ToArray();
                int missingScripts = objects.Sum(x => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(x.gameObject));
                var meshes = objects.SelectMany(x => x.GetComponents<MeshFilter>()).ToArray();
                int missingMeshes = meshes.Count(x => x.sharedMesh == null || x.sharedMesh.vertexCount == 0);
                var renderers = objects.SelectMany(x => x.GetComponents<Renderer>()).ToArray();
                int missingMaterials = renderers.Sum(x => x.sharedMaterials.Count(m => m == null || m.shader == null || m.shader.name == "Hidden/InternalErrorShader"));
                report.AppendLine(path + ": " + objects.Length + " objects, " + meshes.Length + " meshes; missing scripts=" + missingScripts +
                    ", missing/empty meshes=" + missingMeshes + ", invalid materials=" + missingMaterials);
                if (missingScripts + missingMeshes + missingMaterials != 0) throw new Exception(report.ToString());
            }
            EditorSceneManager.OpenScene(Scenes[0]);
            Directory.CreateDirectory("Builds/Review");
            File.WriteAllText("Builds/Review/2022-reference-check.txt", report.ToString());
            Debug.Log(report.ToString());
        }
    }
}
