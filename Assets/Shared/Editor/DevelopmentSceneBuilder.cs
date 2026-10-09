using System;
using System.IO;
using System.Linq;
using FXR.Memory;
using FXR.Shared;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

namespace FXR.Editor
{
    public static class DevelopmentSceneBuilder
    {
        const string MainScene = "Assets/Shared/Scenes/MainScene.unity";
        public const string MemoryScene = "Assets/Rooms/Memory/Scenes/MemoryDev.unity";
        public const string RoomBScene = "Assets/Rooms/RoomB/Scenes/RoomBDev.unity";
        public const string RoomCScene = "Assets/Rooms/RoomC/Scenes/RoomCDev.unity";

        [MenuItem("FXR/Create missing development scenes")]
        public static void CreateMissingScenes()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            var setup = EditorSceneManager.GetSceneManagerSetup();
            try
            {
                CreateScene(MemoryScene, "MemoryRoom", true);
                CreateScene(RoomBScene, "RoomB", false);
                CreateScene(RoomCScene, "RoomC", false);
                AssetDatabase.SaveAssets();
                Debug.Log("FXR development scenes ready. Existing scenes and MainScene were not overwritten.");
            }
            finally
            {
                if (!Application.isBatchMode) EditorSceneManager.RestoreSceneManagerSetup(setup);
            }
        }

        static void CreateScene(string path, string roomName, bool memory)
        {
            if (File.Exists(path)) return;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            AssetDatabase.Refresh();
            if (!AssetDatabase.CopyAsset(MainScene, path)) throw new Exception("Could not copy scene to " + path);
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            foreach (var root in scene.GetRootGameObjects())
            {
                bool support = root.GetComponent<PlayerModeBootstrap>() != null
                    || root.GetComponent<XRInteractionManager>() != null
                    || root.GetComponent<EventSystem>() != null
                    || root.GetComponent<Light>() != null;
                bool ownRoom = memory && root.GetComponent<MemoryRoomController>() != null;
                if (!support && !ownRoom) UnityEngine.Object.DestroyImmediate(root);
                else if (ownRoom && PrefabUtility.IsPartOfPrefabInstance(root))
                    PrefabUtility.UnpackPrefabInstance(root, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
            if (!memory) CreateEmptyRoom(path, roomName);
            if (!EditorSceneManager.SaveScene(scene)) throw new Exception("Could not save " + path);
            Debug.Log("Created " + path);
        }

        static void CreateEmptyRoom(string scenePath, string name)
        {
            string folder = Path.GetDirectoryName(Path.GetDirectoryName(scenePath)).Replace('\\', '/');
            Directory.CreateDirectory(folder + "/Materials");
            AssetDatabase.Refresh();
            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.name = name + "Shell";
            material.color = new Color(.55f, .61f, .65f);
            AssetDatabase.CreateAsset(material, folder + "/Materials/" + material.name + ".mat");
            var root = new GameObject(name);
            CreateCube("Floor", root.transform, new Vector3(0, -.12f, 2), new Vector3(8, .24f, 8), material);
            CreateCube("Back wall", root.transform, new Vector3(0, 1.7f, 6), new Vector3(8, 3.4f, .15f), material);
            CreateCube("Left wall", root.transform, new Vector3(-4, 1.7f, 2), new Vector3(.15f, 3.4f, 8), material);
            CreateCube("Right wall", root.transform, new Vector3(4, 1.7f, 2), new Vector3(.15f, 3.4f, 8), material);
        }

        static void CreateCube(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = position;
            cube.transform.localScale = scale;
            cube.GetComponent<Renderer>().sharedMaterial = material;
        }

        public static void ValidateScenes()
        {
            var setup = EditorSceneManager.GetSceneManagerSetup();
            try
            {
                foreach (string path in new[] { MemoryScene, RoomBScene, RoomCScene })
                {
                    var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                    var roots = scene.GetRootGameObjects();
                    var components = roots.SelectMany(root => root.GetComponentsInChildren<Component>(true)).ToArray();
                    if (components.Any(component => component == null)) throw new Exception("Missing script in " + path);
                    var player = components.OfType<PlayerModeBootstrap>().Single();
                    if (player.desktopRig == null || player.xrRig == null || player.simulator == null)
                        throw new Exception("Missing player references in " + path);
                    if (components.OfType<XRInteractionManager>().Count() != 1 || components.OfType<EventSystem>().Count() != 1)
                        throw new Exception("Duplicate or missing interaction setup in " + path);
                    foreach (var renderer in components.OfType<MeshRenderer>())
                        if (renderer.sharedMaterials.Any(material => material == null)) throw new Exception("Missing material in " + path);
                    var room = roots.Single(root => root.name == (path == MemoryScene ? "MemoryRoom" : path == RoomBScene ? "RoomB" : "RoomC"));
                    if (PrefabUtility.IsPartOfPrefabInstance(room)) throw new Exception("Development room must be editable scene objects: " + path);
                    if (path == MemoryScene)
                    {
                        var controller = room.GetComponent<MemoryRoomController>();
                        if (controller.cards.Length != 12 || controller.cards.Any(card => card == null)
                            || controller.actionButton == null || controller.officeAudio.clip == null || controller.natureAudio.clip == null)
                            throw new Exception("Memory room references incomplete in " + path);
                    }
                    Debug.Log("Validated development scene: " + path);
                }
                if (EditorBuildSettings.scenes.Length != 1 || EditorBuildSettings.scenes[0].path != MainScene || !EditorBuildSettings.scenes[0].enabled)
                    throw new Exception("Final build must use MainScene only.");
            }
            finally
            {
                if (!Application.isBatchMode) EditorSceneManager.RestoreSceneManagerSetup(setup);
            }
        }

        public static void CreateAndValidate()
        {
            CreateMissingScenes();
            ValidateScenes();
        }
    }
}
