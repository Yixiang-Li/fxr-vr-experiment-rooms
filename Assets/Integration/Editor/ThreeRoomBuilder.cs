using System;
using System.IO;
using System.Linq;
using FXR.Memory;
using FXR.Shared;
using FXR.Integration;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public static class ThreeRoomBuilder
{
    const string Main="Assets/Shared/Scenes/MainScene.unity";
    const string Museum="Assets/Integration/Prefabs/MuseumRoom.prefab";
    const string Stroop="Assets/Integration/Prefabs/StroopRoom.prefab";

    [MenuItem("FXR/Integration/Rebuild integrated room snapshots")]
    public static void Build()
    {
        if(!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
        EditorSceneManager.OpenScene(Main);
        if(UnityEngine.Object.FindFirstObjectByType<RoomIntegrationManager>()!=null)
            throw new Exception("Integration already exists. Preserve MainScene edits before explicitly rebuilding snapshots.");
        Directory.CreateDirectory("Builds/Backups/Integration");
        File.Copy(Main,"Builds/Backups/Integration/MainScene-"+DateTime.Now.ToString("yyyyMMdd-HHmmss")+".unity",false);
        UpgradeMaterials("Assets/Rooms/RoomB");UpgradeMaterials("Assets/Rooms/AidaRoom");
        var views=new RoomView[3];
        EditorSceneManager.OpenScene(RoomIntegrationAudit.Sources[0]);
        var memoryPlayer=UnityEngine.Object.FindFirstObjectByType<DesktopExplorer>(FindObjectsInactive.Include);
        views[0]=Capture(memoryPlayer.transform,memoryPlayer.view);
        views[1]=Package(RoomIntegrationAudit.Sources[1],Museum,false);
        views[2]=Package(RoomIntegrationAudit.Sources[2],Stroop,true);
        var scene=EditorSceneManager.OpenScene(Main);
        foreach(var r in scene.GetRootGameObjects())if(r.name=="RoomB"||r.name=="RoomC")UnityEngine.Object.DestroyImmediate(r);
        var explorer=UnityEngine.Object.FindFirstObjectByType<DesktopExplorer>(FindObjectsInactive.Include);
        var player=explorer.gameObject.AddComponent<PlayerController>();player.enabled=false;
        var memory=UnityEngine.Object.FindFirstObjectByType<MemoryRoomController>();
        var manager=new GameObject("Three room navigation").AddComponent<RoomIntegrationManager>();
        manager.explorer=explorer;manager.museumMovement=player;manager.font=memory.statusText.font;
        manager.sharedDaylight=scene.GetRootGameObjects().Single(r=>r.name=="Shared Daylight").GetComponent<Light>();
        manager.rooms=new[]{
            new RoomSlot{title="MEMORY",prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Rooms/Memory/Prefabs/MemoryRoom.prefab"),instance=memory.gameObject,view=views[0]},
            new RoomSlot{title="MUSEUM",prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Museum),offset=new Vector3(40,0,0),view=views[1]},
            new RoomSlot{title="STROOP",prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Stroop),offset=new Vector3(-40,0,0),view=views[2]}
        };
        for(int i=1;i<3;i++)
        {
            var slot=manager.rooms[i];slot.instance=(GameObject)PrefabUtility.InstantiatePrefab(slot.prefab,scene);
            slot.instance.transform.position=slot.offset;slot.instance.SetActive(false);
        }
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Main,true)};
        PlayerSettings.bundleVersion="0.5.0";
        EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        Validate();
        Debug.Log("Three rooms integrated in MainScene; original owner scenes retained.");
    }

    static RoomView Package(string source,string destination,bool stroop)
    {
        var scene=EditorSceneManager.OpenScene(source);
        var transforms=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).ToArray();
        var camera=transforms.Select(t=>t.GetComponent<Camera>()).First(c=>c!=null && c.targetTexture==null);
        var player=stroop?camera.GetComponentInParent<PlayerMovement>().transform:camera.GetComponentInParent<PlayerController>().transform;
        var view=Capture(player,camera);
        UnityEngine.Object.DestroyImmediate(player.gameObject);
        foreach(var e in UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include,FindObjectsSortMode.None))UnityEngine.Object.DestroyImmediate(e.gameObject);
        foreach(var a in UnityEngine.Object.FindObjectsByType<AudioListener>(FindObjectsInactive.Include,FindObjectsSortMode.None))UnityEngine.Object.DestroyImmediate(a);
        foreach(var c in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include,FindObjectsSortMode.None))
        { if(c.targetTexture==null)UnityEngine.Object.DestroyImmediate(c.gameObject);else c.tag="Untagged"; }
        var root=new GameObject(stroop?"Stroop room":"Museum room");
        foreach(var r in scene.GetRootGameObjects())if(r!=root)r.transform.SetParent(root.transform,true);
        root.AddComponent<IntegratedRoom>();
        foreach(var m in root.GetComponentsInChildren<ExperimentManager>(true))m.testMode=false;
        foreach(var d in root.GetComponentsInChildren<DistractorTask>(true))d.testMode=false;
        root.SetActive(false);
        Directory.CreateDirectory(Path.GetDirectoryName(destination));
        PrefabUtility.SaveAsPrefabAsset(root,destination);
        return view;
    }
    static RoomView Capture(Transform player,Camera camera)
    {
        var body=player.GetComponent<CharacterController>();
        return new RoomView{
            position=player.position,rotation=player.rotation,cameraPosition=camera.transform.localPosition,cameraRotation=camera.transform.localRotation,
            height=body.height,center=body.center,radius=body.radius,fieldOfView=camera.fieldOfView,
            ambientMode=RenderSettings.ambientMode,ambientLight=RenderSettings.ambientLight,ambientSky=RenderSettings.ambientSkyColor,
            ambientEquator=RenderSettings.ambientEquatorColor,ambientGround=RenderSettings.ambientGroundColor,ambientIntensity=RenderSettings.ambientIntensity,
            reflectionIntensity=RenderSettings.reflectionIntensity,skybox=RenderSettings.skybox,fog=RenderSettings.fog,fogColor=RenderSettings.fogColor,fogDensity=RenderSettings.fogDensity,
            clearFlags=camera.clearFlags,background=camera.backgroundColor
        };
    }
    static void UpgradeMaterials(string folder)
    {
        foreach(var guid in AssetDatabase.FindAssets("t:Material",new[]{folder}))
        {
            var material=AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
            if(material==null || material.shader==null || !(material.shader.name=="Standard"||material.shader.name=="Standard (Specular setup)"||material.shader.name=="Autodesk Interactive"))continue;
            var color=material.HasProperty("_Color")?material.GetColor("_Color"):Color.white;
            var texture=material.HasProperty("_MainTex")?material.GetTexture("_MainTex"):null;
            var scale=material.GetTextureScale("_MainTex");var offset=material.GetTextureOffset("_MainTex");
            var normal=material.HasProperty("_BumpMap")?material.GetTexture("_BumpMap"):null;
            var emission=material.HasProperty("_EmissionColor")?material.GetColor("_EmissionColor"):Color.black;
            var metallic=material.HasProperty("_Metallic")?material.GetFloat("_Metallic"):0;
            var smoothness=material.HasProperty("_Glossiness")?material.GetFloat("_Glossiness"):.3f;
            material.shader=Shader.Find("Universal Render Pipeline/Lit");
            material.SetColor("_BaseColor",color);material.SetTexture("_BaseMap",texture);material.SetTextureScale("_BaseMap",scale);material.SetTextureOffset("_BaseMap",offset);
            material.SetFloat("_Metallic",metallic);material.SetFloat("_Smoothness",smoothness);
            if(normal!=null){material.SetTexture("_BumpMap",normal);material.EnableKeyword("_NORMALMAP");}
            material.SetColor("_EmissionColor",emission);if(emission.maxColorComponent>0)material.EnableKeyword("_EMISSION");
            EditorUtility.SetDirty(material);
        }
        AssetDatabase.SaveAssets();
    }
    [MenuItem("FXR/Integration/Check three room references")]
    public static void Validate()
    {
        var scene=EditorSceneManager.OpenScene(Main);
        var manager=UnityEngine.Object.FindFirstObjectByType<RoomIntegrationManager>();
        if(manager==null||manager.rooms.Length!=3)throw new Exception("Three room navigation missing.");
        int missing=0;
        foreach(var t in scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true)))
        {
            missing+=GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
            foreach(var renderer in t.GetComponents<Renderer>())
                if(renderer.sharedMaterials.Any(m=>m==null||m.shader==null||m.shader.name=="Hidden/InternalErrorShader"))throw new Exception("Missing material: "+t.name);
        }
        if(missing>0)throw new Exception("Missing integrated scripts: "+missing);
        if(UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length!=1)throw new Exception("Must retain one shared EventSystem.");
        foreach(var slot in manager.rooms)if(slot.prefab==null||slot.instance==null)throw new Exception("Missing room snapshot: "+slot.title);
        if(manager.rooms.Skip(1).Any(s=>s.instance.GetComponentsInChildren<CharacterController>(true).Length>0))throw new Exception("Imported room must use the shared player.");
        Debug.Log("Three room reference validation passed.");
    }
    public static void FinalizeDefaults()
    {
        var scene=EditorSceneManager.OpenScene(Main);
        UnityEngine.Object.FindFirstObjectByType<PlayerModeBootstrap>().desktopOnly=true;
        var manager=UnityEngine.Object.FindFirstObjectByType<RoomIntegrationManager>();
        var root=PrefabUtility.LoadPrefabContents(Stroop);
        try
        {
            foreach(var button in root.GetComponentsInChildren<PhysicalButton>(true))
            {
                string value=button.name=="StartButton"?"START":button.name.Trim().Replace("Button","").ToUpperInvariant();
                string labelName="Integration label "+value;
                if(root.transform.Find(labelName)!=null)continue;
                var label=new GameObject(labelName).AddComponent<TMPro.TextMeshPro>();
                label.transform.SetParent(root.transform,false);
                label.transform.position=button.transform.position+Vector3.up*.13f;
                label.transform.rotation=Quaternion.LookRotation(label.transform.position-new Vector3(-3.19f,1.31f,-5.61f));
                label.font=manager.font;label.fontSize=.18f;label.alignment=TMPro.TextAlignmentOptions.Center;
                label.rectTransform.sizeDelta=new Vector2(.5f,.2f);label.text=value;label.color=Color.white;
            }
            PrefabUtility.SaveAsPrefabAsset(root,Stroop);
        }
        finally{PrefabUtility.UnloadPrefabContents(root);}
        EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Validate();
    }
    [MenuItem("FXR/Integration/Play three rooms")]
    public static void Play()
    {
        if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
        EditorSceneManager.OpenScene(Main);SessionState.SetBool("FXR.FocusMemoryGameView",true);EditorApplication.isPlaying=true;
    }
}
