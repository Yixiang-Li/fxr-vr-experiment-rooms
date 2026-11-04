using System;
using System.IO;
using System.Linq;
using FXR.Memory;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace FXR.Editor
{
    // Makes small, repeatable additions to the owner's existing scene. This
    // deliberately does not call either prototype or room-layout generators.
    [InitializeOnLoad]
    public static class DesktopRoomUpgrade
    {
        const string Dev = "Assets/Rooms/Memory/Scenes/MemoryDev.unity";
        const string Prefab = "Assets/Rooms/Memory/Prefabs/MemoryRoom.prefab";
        const string Main = "Assets/Shared/Scenes/MainScene.unity";
        const string FocusKey = "FXR.FocusMemoryGameView";

        static DesktopRoomUpgrade() { EditorApplication.playModeStateChanged += FocusGameView; }

        [MenuItem("FXR/Desktop/Update Memory desktop setup")]
        public static void Apply()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            string backup = "Builds/Backups/Desktop-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
            Directory.CreateDirectory(backup);
            foreach (string file in new[] { Dev, Prefab, Main }) File.Copy(file, backup + "/" + Path.GetFileName(file));
            var scene = EditorSceneManager.OpenScene(Dev);
            var room = scene.GetRootGameObjects().Select(x => x.GetComponent<MemoryRoomController>()).Single(x => x != null);
            if (PrefabUtility.IsPartOfPrefabInstance(room.gameObject)) throw new Exception("The owner's scene must remain an ordinary editable room.");
            var material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Rooms/Memory/Materials/DesktopPractice.mat");
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                material.SetColor("_BaseColor", new Color(.95f, .52f, .17f)); material.SetFloat("_Smoothness", .25f);
                AssetDatabase.CreateAsset(material, "Assets/Rooms/Memory/Materials/DesktopPractice.mat");
            }
            if (room.transform.Find("Mouse practice") == null)
            {
                var practice = new GameObject("Mouse practice"); practice.transform.SetParent(room.transform, false);
                var baseObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                baseObject.name = "Practice plinth"; baseObject.transform.SetParent(practice.transform, false);
                baseObject.transform.localPosition = new Vector3(.2f, .55f, 1.45f);
                baseObject.transform.localScale = new Vector3(.65f, 1.1f, .5f);
                baseObject.GetComponent<Renderer>().sharedMaterial = room.transform.Find("Table").GetComponent<Renderer>().sharedMaterial;
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "Orange practice cube"; cube.transform.SetParent(practice.transform, false);
                cube.transform.localPosition = new Vector3(.2f, 1.215f, 1.45f); cube.transform.localScale = Vector3.one * .22f;
                cube.GetComponent<Renderer>().sharedMaterial = material;
                var body = cube.AddComponent<Rigidbody>(); body.mass = .25f; body.drag = .8f; body.angularDrag = .8f;
                body.interpolation = RigidbodyInterpolation.Interpolate; body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                cube.AddComponent<MemoryPracticeObject>().room = room;
                var label = new GameObject("Practice instruction").AddComponent<TextMeshPro>();
                label.transform.SetParent(practice.transform, false); label.transform.localPosition = new Vector3(.2f, 1.46f, 1.5f);
                label.font = room.statusText.font; label.text = "DRAG ME"; label.fontSize = .45f;
                label.alignment = TextAlignmentOptions.Center; label.rectTransform.sizeDelta = new Vector2(.72f, .16f);
                label.color = new Color(.93f, .8f, .57f);
            }
            var practiceLabel = room.transform.Find("Mouse practice/Practice instruction").GetComponent<TextMeshPro>();
            practiceLabel.text = "DRAG ME"; practiceLabel.fontSize = .45f;
            practiceLabel.rectTransform.sizeDelta = new Vector2(.72f, .16f);
            room.practiceArea = room.transform.Find("Mouse practice").gameObject;
            var accent = room.transform.Find("Environment accent light");
            if (accent == null)
            {
                accent = new GameObject("Environment accent light").transform; accent.SetParent(room.transform, false);
                accent.localPosition = new Vector3(-2.8f, 2.4f, 4.4f);
                var light = accent.gameObject.AddComponent<Light>(); light.type = LightType.Point;
                light.range = 2.4f; light.shadows = LightShadows.None;
            }
            room.environmentLight = accent.GetComponent<Light>();
            room.environmentLight.color = new Color(.65f, .8f, 1f); room.environmentLight.intensity = 1.1f;
            room.statusText.fontSize = 36;
            room.feedbackText.fontSize = 27;
            room.phaseText.fontSize = 29;
            var subtitle = room.transform.Find("Room subtitle").GetComponent<TextMeshPro>();
            subtitle.text = "Find six matching pairs. Same task, two settings.";
            var hint = room.transform.Find("Experiment panel/Interaction hint").GetComponent<TMP_Text>();
            hint.text = "Left click: cards and buttons\nF: task view  /  SPACE: start the round";
            var setupNote = room.transform.Find("Experiment panel/Setup controls/Setup note").GetComponent<TMP_Text>();
            setupNote.text = "Choose the order before starting.\nBoth rounds use the same task view and controls.";
            room.transform.Find("Experiment panel/Reset confirmation/Reset explanation").GetComponent<TMP_Text>().text =
                "Current progress and ratings will be cleared.\nSaved CSV files stay on this device.\nChoose Cancel to keep this session.";
            room.resetConfirmation.GetComponentsInChildren<Button>(true).Single(x => x.name == "KEEP PLAYING")
                .GetComponentInChildren<TMP_Text>().text = "CANCEL";
            DisableButtonNavigation(room);
            EditorUtility.SetDirty(room);
            EditorSceneManager.SaveScene(scene);
            UpdateExistingPrefab(room);
            AssetDatabase.SaveAssets();
            scene = EditorSceneManager.OpenScene(Main);
            var instance = scene.GetRootGameObjects().Single(x => x.GetComponent<MemoryRoomController>() != null);
            if (PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(instance) != Prefab)
                throw new Exception("MainScene does not use the expected Memory prefab.");
            EditorSceneManager.SaveScene(scene);
            PlayerSettings.bundleVersion = "0.4.0";
            PlayerSettings.resizableWindow = true;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = 1280; PlayerSettings.defaultScreenHeight = 800;
            AssetDatabase.SaveAssets();
            Unity2022Migration.ValidateReferences();
            EditorSceneManager.OpenScene(Dev);
            Debug.Log("Desktop Memory setup updated and integrated; teammate scenes retained.");
        }

        static void UpdateExistingPrefab(MemoryRoomController source)
        {
            // Edit the existing asset in place. Saving an unpacked development
            // root wholesale can regenerate nested prefab identifiers.
            var root = PrefabUtility.LoadPrefabContents(Prefab);
            try
            {
                var target = root.GetComponent<MemoryRoomController>();
                foreach (string name in new[] { "Mouse practice", "Environment accent light" })
                {
                    if (root.transform.Find(name) != null) continue;
                    var original = source.transform.Find(name);
                    var copy = UnityEngine.Object.Instantiate(original.gameObject);
                    copy.name = name; copy.transform.SetParent(root.transform, false);
                    copy.transform.localPosition = original.localPosition;
                    copy.transform.localRotation = original.localRotation;
                    copy.transform.localScale = original.localScale;
                }
                target.environmentLight = root.transform.Find("Environment accent light").GetComponent<Light>();
                target.practiceArea = root.transform.Find("Mouse practice").gameObject;
                foreach (var item in root.GetComponentsInChildren<MemoryPracticeObject>(true)) item.room = target;
                target.statusText.fontSize = source.statusText.fontSize;
                target.feedbackText.fontSize = source.feedbackText.fontSize;
                target.phaseText.fontSize = source.phaseText.fontSize;
                foreach (string name in new[] { "Room subtitle", "Experiment panel/Interaction hint", "Experiment panel/Setup controls/Setup note", "Mouse practice/Practice instruction", "Experiment panel/Reset confirmation/Reset explanation" })
                {
                    var label = root.transform.Find(name).GetComponent<TMP_Text>();
                    var original = source.transform.Find(name).GetComponent<TMP_Text>();
                    label.text = original.text; label.fontSize = original.fontSize;
                }
                target.resetConfirmation.GetComponentsInChildren<Button>(true).Single(x => x.name == "KEEP PLAYING")
                    .GetComponentInChildren<TMP_Text>().text = "CANCEL";
                DisableButtonNavigation(target);
                PrefabUtility.SaveAsPrefabAsset(root, Prefab);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }

        static void DisableButtonNavigation(MemoryRoomController room)
        {
            foreach (var button in room.GetComponentsInChildren<Button>(true))
            {
                var navigation = button.navigation; navigation.mode = Navigation.Mode.None; button.navigation = navigation;
            }
        }

        [MenuItem("FXR/Desktop/Play Memory Room")]
        public static void PlayMemory()
        {
            if (EditorApplication.isPlaying) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(Dev);
            SessionState.SetBool(FocusKey, true);
            EditorApplication.isPlaying = true;
        }

        static void FocusGameView(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(FocusKey, false)) return;
            SessionState.SetBool(FocusKey, false);
            var type = AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetType("UnityEditor.GameView")).FirstOrDefault(x => x != null);
            if (type != null) EditorWindow.GetWindow(type).Focus();
        }

        [MenuItem("FXR/Desktop/Build preview for this computer")]
        public static void BuildPreview()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Unity2022Migration.ValidateReferences();
            QuestReadiness.ClearDynamicFontCaches();
            var target = Application.platform == RuntimePlatform.WindowsEditor ? BuildTarget.StandaloneWindows64 : BuildTarget.StandaloneOSX;
            string path = target == BuildTarget.StandaloneWindows64 ? "Builds/Windows/FXRMemory.exe" : "Builds/Mac/FXRPreview.app";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var options = BuildOptions.Development;
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-fxr-clean-build") >= 0)
                options |= BuildOptions.CleanBuildCache;
            var result = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = new[] { Main }, locationPathName = path, target = target, options = options });
            if (result.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new Exception("Desktop build failed: " + result.summary.result);
            Debug.Log("Desktop preview built: " + path);
        }
    }
}
