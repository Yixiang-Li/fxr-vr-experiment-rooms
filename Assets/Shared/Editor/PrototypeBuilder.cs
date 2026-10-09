using System;
using System.IO;
using System.Linq;
using FXR.Memory;
using FXR.Shared;
using TMPro;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace FXR.Editor
{
    public static class PrototypeBuilder
    {
        const string ScenePath = "Assets/Shared/Scenes/MainScene.unity";
        const string RoomPath = "Assets/Rooms/Memory/Prefabs/MemoryRoom.prefab";
        const string Samples = "Assets/Samples/XR Interaction Toolkit/3.3.2/";
        static TMP_FontAsset font;
        static Material wall, floor, wood, dark, teal, white, gold, green;

        [MenuItem("FXR/Rebuild generated prototype")]
        public static void BuildPrototype()
        {
            if (!Application.isBatchMode && File.Exists(ScenePath)
                && !EditorUtility.DisplayDialog("Rebuild prototype", "This regenerates MainScene and MemoryRoom.prefab. Commit team edits first.", "Rebuild", "Cancel")) return;
            ConfigureProject();
            ImportTextResources();
            wall = Material("Wall", new Color(0.72f,0.78f,0.82f));
            floor = Material("Floor", new Color(0.24f,0.29f,0.32f));
            wood = Material("Wood", new Color(0.42f,0.26f,0.16f));
            dark = Material("CardBack", new Color(0.08f,0.17f,0.2f));
            teal = Material("Accent", new Color(0.12f,0.52f,0.5f));
            white = Material("CardFace", new Color(0.95f,0.93f,0.84f));
            gold = Material("Tray", new Color(0.61f,0.42f,0.22f));
            green = Material("Leaves", new Color(0.18f,0.45f,0.25f));
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.54f,0.58f,0.64f);
            var sun = new GameObject("Shared Daylight").AddComponent<Light>();
            sun.type = LightType.Directional; sun.intensity = 1.1f;
            sun.transform.rotation = Quaternion.Euler(48,-25,0);
            sun.shadows = LightShadows.Soft;
            new GameObject("XR Interaction Manager").AddComponent<XRInteractionManager>();
            var eventSystem = new GameObject("Shared Event System", typeof(EventSystem), typeof(XRUIInputModule));
            eventSystem.GetComponent<XRUIInputModule>().enableMouseInput = true;

            GameObject memory = BuildMemoryRoom();
            PrefabUtility.SaveAsPrefabAssetAndConnect(memory, RoomPath, InteractionMode.AutomatedAction);
            BuildEntry();
            BuildPlayers();
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            Debug.Log("FXR prototype created: " + ScenePath);
        }

        static void ImportTextResources()
        {
            string[] found = AssetDatabase.FindAssets("LiberationSans SDF t:TMP_FontAsset");
            if (found.Length == 0)
            {
                var package = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(TextMeshPro).Assembly);
                AssetDatabase.ImportPackage(package.resolvedPath + "/Package Resources/TMP Essential Resources.unitypackage", false);
                AssetDatabase.Refresh();
                found = AssetDatabase.FindAssets("LiberationSans SDF t:TMP_FontAsset");
            }
            font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(found[0]));
        }

        static GameObject BuildMemoryRoom()
        {
            var root = new GameObject("MemoryRoom");
            var room = root.AddComponent<MemoryRoomController>();
            Cube("Floor",root.transform,new Vector3(0,-0.12f,2),new Vector3(8,0.24f,8),floor);
            var walls = new[] {
                Cube("Back Wall",root.transform,new Vector3(0,1.8f,6),new Vector3(8,3.6f,0.15f),wall),
                Cube("Left Wall",root.transform,new Vector3(-4,1.8f,2),new Vector3(0.15f,3.6f,8),wall),
                Cube("Right Wall",root.transform,new Vector3(4,1.8f,2),new Vector3(0.15f,3.6f,8),wall)
            };
            room.walls = walls.Select(w => w.GetComponent<Renderer>()).ToArray();
            Cube("Front Left",root.transform,new Vector3(-2.65f,1.8f,-2),new Vector3(2.7f,3.6f,.15f),wall);
            Cube("Front Right",root.transform,new Vector3(2.65f,1.8f,-2),new Vector3(2.7f,3.6f,.15f),wall);
            Cube("Door Header",root.transform,new Vector3(0,3.2f,-2),new Vector3(2.6f,.8f,.15f),teal);
            Text3D("Room title",root.transform,"MEMORY / ENVIRONMENT",new Vector3(-.9f,2.83f,2.4f),2.1f,.2f,1f,Color.white);
            Text3D("Room subtitle",root.transform,"Same task. Two atmospheres.",new Vector3(-.9f,2.68f,2.39f),2.1f,.15f,.6f,new Color(.2f,.28f,.3f));
            Cube("Table",root.transform,new Vector3(-.9f,.82f,2.45f),new Vector3(2.35f,.12f,1.2f),wood);
            foreach (float x in new[] {-1.9f,.1f}) foreach (float z in new[] {2f,2.85f})
                Cube("Table leg",root.transform,new Vector3(x,.4f,z),new Vector3(.08f,.8f,.08f),dark);
            Cube("Memory board",root.transform,new Vector3(-.9f,1.91f,2.42f),new Vector3(1.62f,1.47f,.08f),dark);
            Cube("Collection tray base",root.transform,new Vector3(-.9f,.91f,2.28f),new Vector3(1.72f,.04f,.65f),gold);
            Cube("Tray front",root.transform,new Vector3(-.9f,1.0f,1.98f),new Vector3(1.72f,.18f,.035f),gold);
            Cube("Tray left",root.transform,new Vector3(-1.74f,1.0f,2.28f),new Vector3(.035f,.18f,.65f),gold);
            Cube("Tray right",root.transform,new Vector3(-.06f,1.0f,2.28f),new Vector3(.035f,.18f,.65f),gold);
            room.cards = new MemoryCard[12];
            for(int i=0;i<12;i++)
            {
                var cardObject = Cube("Card " + (i+1).ToString("00"),root.transform,
                    new Vector3(-1.47f+(i%4)*.38f,2.36f-(i/4)*.43f,2.32f),new Vector3(.3f,.35f,.05f),dark);
                // Restore unit scale so card physics and symbol dimensions use metres.
                cardObject.transform.localScale = Vector3.one;
                cardObject.GetComponent<MeshFilter>().sharedMesh = BoxMesh(new Vector3(.3f,.35f,.05f));
                cardObject.GetComponent<BoxCollider>().size = new Vector3(.3f,.35f,.05f);
                var body=cardObject.AddComponent<Rigidbody>(); body.isKinematic=true; body.mass=.12f;
                cardObject.AddComponent<XRSimpleInteractable>();
                var card=cardObject.AddComponent<MemoryCard>();
                card.index=i; card.room=room; card.cardRenderer=cardObject.GetComponent<Renderer>();
                card.faceDownMaterial=dark; card.faceUpMaterial=white;
                var face = new GameObject("Symbols"); face.transform.SetParent(cardObject.transform,false);
                face.transform.localPosition=new Vector3(0,.015f,-.029f);
                card.front=face; card.symbols=new GameObject[6];
                for(int pair=0;pair<6;pair++)
                {
                    var symbol=new GameObject("Symbol "+pair,typeof(MeshFilter),typeof(MeshRenderer));
                    symbol.transform.SetParent(face.transform,false);
                    symbol.GetComponent<MeshFilter>().sharedMesh=SymbolMesh(pair);
                    symbol.GetComponent<Renderer>().sharedMaterial=Material("Symbol"+pair,
                        Color.HSVToRGB(pair/6f,.65f,.7f),true);
                    symbol.SetActive(pair==i/2); card.symbols[pair]=symbol;
                }
                face.SetActive(false);
                Text3D("Card number",cardObject.transform,(i+1).ToString("00"),new Vector3(0,-.13f,-.03f),.22f,.05f,.35f,new Color(.55f,.65f,.65f));
                room.cards[i]=card;
            }
            room.officeProps = BuildOffice(root.transform);
            room.natureProps = BuildNature(root.transform); room.natureProps.SetActive(false);
            room.officeAudio=Audio("Office ambience",root.transform,"Office",new Vector3(-2.8f,1.2f,4.4f));
            room.natureAudio=Audio("Nature ambience",root.transform,"Nature",new Vector3(2.6f,2.1f,4.4f));
            var fixture = Cube("Ceiling lamp",root.transform,new Vector3(0,3.3f,2),new Vector3(2,.06f,.65f),white);
            room.roomLight = new GameObject("Mood light").AddComponent<Light>();
            room.roomLight.transform.SetParent(root.transform,false);
            room.roomLight.transform.localPosition=new Vector3(0,3.1f,2);
            room.roomLight.type=LightType.Point; room.roomLight.range=8; room.roomLight.intensity=2.1f;
            BuildPanel(root.transform,room);
            return root;
        }

        static GameObject BuildOffice(Transform parent)
        {
            var group=new GameObject("Office props"); group.transform.SetParent(parent,false);
            Cube("Side desk",group.transform,new Vector3(-2.8f,.75f,4.65f),new Vector3(1.7f,.15f,.8f),wall);
            Cube("Monitor",group.transform,new Vector3(-2.8f,1.28f,4.67f),new Vector3(.95f,.55f,.06f),dark);
            Cube("Screen",group.transform,new Vector3(-2.8f,1.28f,4.62f),new Vector3(.84f,.45f,.01f),teal);
            Cube("Monitor stand",group.transform,new Vector3(-2.8f,.98f,4.65f),new Vector3(.08f,.35f,.08f),dark);
            Cube("Keyboard",group.transform,new Vector3(-2.8f,.855f,4.36f),new Vector3(.7f,.035f,.23f),dark);
            for(int i=0;i<5;i++)
                Cube("Paper",group.transform,new Vector3(-3.35f,.86f+i*.013f,4.6f),new Vector3(.35f,.013f,.42f),white);
            Cube("Phone",group.transform,new Vector3(-2.05f,.91f,4.56f),new Vector3(.18f,.16f,.3f),dark);
            for(int i=0;i<4;i++)
                Cube("Office wall panel",group.transform,new Vector3(3.89f,1.75f,1+i*.95f),new Vector3(.04f,1.3f,.65f),Material("Office panels",new Color(.4f,.5f,.59f)));
            return group;
        }

        static GameObject BuildNature(Transform parent)
        {
            var group=new GameObject("Nature props"); group.transform.SetParent(parent,false);
            for(int i=0;i<6;i++)
            {
                float x=i%2==0?-2.9f:2.9f, z=.4f+(i/2)*2.15f;
                Primitive("Pot",group.transform,PrimitiveType.Cylinder,new Vector3(x,.23f,z),new Vector3(.43f,.23f,.43f),wood);
                Primitive("Stem",group.transform,PrimitiveType.Cylinder,new Vector3(x,.9f,z),new Vector3(.04f,.58f,.04f),gold);
                for(int j=0;j<4;j++)
                {
                    var leaf=Primitive("Leaf",group.transform,PrimitiveType.Sphere,new Vector3(x+(j%2==0?.2f:-.2f),.7f+j*.24f,z),new Vector3(.64f,.26f,.42f),green);
                    leaf.transform.localRotation=Quaternion.Euler(0,j*30,j%2==0?25:-25);
                }
            }
            Cube("Green view",group.transform,new Vector3(0,1.55f,5.88f),new Vector3(3.6f,1.4f,.03f),Material("Nature view",new Color(.32f,.58f,.43f)));
            return group;
        }

        static void BuildPanel(Transform parent,MemoryRoomController room)
        {
            var panel=new GameObject("Experiment panel",typeof(RectTransform),typeof(Canvas),typeof(TrackedDeviceGraphicRaycaster),typeof(GraphicRaycaster));
            panel.transform.SetParent(parent,false); panel.transform.localPosition=new Vector3(1.35f,1.85f,2.25f);
            panel.transform.localScale=Vector3.one*.00165f;
            panel.GetComponent<Canvas>().renderMode=RenderMode.WorldSpace;
            panel.GetComponent<RectTransform>().sizeDelta=new Vector2(780,1000);
            var background=new GameObject("Background",typeof(RectTransform),typeof(Image));
            background.transform.SetParent(panel.transform,false);
            var rect=background.GetComponent<RectTransform>();rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.sizeDelta=Vector2.zero;
            background.GetComponent<Image>().color=new Color(.06f,.12f,.15f,.98f);
            UIString("Header",panel.transform,"MEMORY LAB",new Vector2(0,435),new Vector2(700,70),38,Color.white);
            room.statusText=UIString("Instructions",panel.transform,"",new Vector2(0,290),new Vector2(700,200),25,Color.white);
            room.actionButton=UIButton(panel.transform,"Start",new Vector2(0,130),new Vector2(700,70),teal.color,out room.actionText);
            UnityEventTools.AddPersistentListener(room.actionButton.onClick,room.StartOrContinue);
            room.orderButton=UIButton(panel.transform,"Order",new Vector2(0,45),new Vector2(700,55),new Color(.22f,.29f,.32f),out room.orderText);
            UnityEventTools.AddPersistentListener(room.orderButton.onClick,room.ToggleOrder);
            var office=UIButton(panel.transform,"PREVIEW OFFICE",new Vector2(-180,-25),new Vector2(340,55),new Color(.3f,.35f,.4f),out _);
            UnityEventTools.AddPersistentListener(office.onClick,room.PreviewOffice);
            var nature=UIButton(panel.transform,"PREVIEW NATURE",new Vector2(180,-25),new Vector2(340,55),new Color(.24f,.4f,.3f),out _);
            UnityEventTools.AddPersistentListener(nature.onClick,room.PreviewNature);
            room.ratingPanel=new GameObject("Effort rating",typeof(RectTransform)); room.ratingPanel.transform.SetParent(panel.transform,false);
            for(int i=1;i<=5;i++)
            {
                var button=UIButton(room.ratingPanel.transform,i.ToString(),new Vector2((i-3)*125,-115),new Vector2(110,65),new Color(.38f,.31f,.23f),out _);
                UnityEventTools.AddIntPersistentListener(button.onClick,room.RateEffort,i);
            }
            room.ratingPanel.SetActive(false);
            room.resultText=UIString("Results",panel.transform,"",new Vector2(0,-285),new Vector2(700,245),24,new Color(.74f,.9f,.85f));
            var reset=UIButton(panel.transform,"RESET ROOM",new Vector2(0,-435),new Vector2(700,55),new Color(.45f,.24f,.2f),out _);
            UnityEventTools.AddPersistentListener(reset.onClick,room.ResetRoom);
        }

        static void BuildEntry()
        {
            var root=new GameObject("Shared Entry - integration owner only");
            Cube("Lobby floor",root.transform,new Vector3(0,-.12f,-4),new Vector3(8,.24f,4),floor);
            Cube("Lobby back wall",root.transform,new Vector3(0,1.7f,-6),new Vector3(8,3.4f,.15f),wall);
            var sign=Text3D("Shared entrance sign",root.transform,"FXR EXPERIMENT ROOMS",new Vector3(0,2.5f,-5.88f),6,.6f,4,Color.white);
            sign.transform.localRotation=Quaternion.Euler(0,180,0);
            foreach(var slot in new[]{("RoomB",-4f), ("RoomC",4f)})
            {
                var marker=new GameObject(slot.Item1+" - teammate prefab slot");marker.transform.SetParent(root.transform,false);
                marker.transform.localPosition=new Vector3(slot.Item2,0,-4);
                Cube("Entry marker",marker.transform,new Vector3(0,.01f,0),new Vector3(1.4f,.02f,1.8f),teal);
            }
        }

        static void BuildPlayers()
        {
            var root=new GameObject("Shared Player Setup");
            var xrPrefab=AssetDatabase.LoadAssetAtPath<GameObject>(Samples+"Starter Assets/Prefabs/XR Origin (XR Rig).prefab");
            var rig=(GameObject)PrefabUtility.InstantiatePrefab(xrPrefab);rig.name="Shared XR Rig";rig.transform.SetParent(root.transform);
            rig.transform.position=new Vector3(0,0,-.7f);
            foreach(var move in rig.GetComponentsInChildren<ContinuousMoveProvider>(true)) move.moveSpeed=1.4f;
            var simulatorPrefab=AssetDatabase.LoadAssetAtPath<GameObject>(Samples+"XR Interaction Simulator/XR Interaction Simulator.prefab");
            var simulator=(GameObject)PrefabUtility.InstantiatePrefab(simulatorPrefab);simulator.transform.SetParent(root.transform);simulator.SetActive(false);
            var desktop=new GameObject("Desktop Preview",typeof(CharacterController),typeof(DesktopExplorer));desktop.transform.SetParent(root.transform,false);
            desktop.transform.position=new Vector3(0,0,-.7f);
            var body=desktop.GetComponent<CharacterController>();body.height=1.7f;body.center=new Vector3(0,.85f,0);body.radius=.2f;
            var camera=new GameObject("Desktop Camera",typeof(Camera),typeof(AudioListener));camera.tag="MainCamera";
            camera.transform.SetParent(desktop.transform,false);camera.transform.localPosition=new Vector3(0,1.65f,0);
            var view=camera.GetComponent<Camera>();view.nearClipPlane=.05f;view.farClipPlane=100;view.fieldOfView=65;view.backgroundColor=new Color(.12f,.16f,.18f);
            desktop.GetComponent<DesktopExplorer>().view=view;
            var mode=root.AddComponent<PlayerModeBootstrap>();mode.xrRig=rig;mode.desktopRig=desktop;mode.simulator=simulator;
            rig.SetActive(false);
        }

        static Material Material(string name,Color color,bool unlit=false)
        {
            string path="Assets/Rooms/Memory/Materials/"+name+".mat";
            var material=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(material==null){material=new Material(Shader.Find(unlit?"Universal Render Pipeline/Unlit":"Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(material,path);}
            material.color=color;EditorUtility.SetDirty(material);return material;
        }

        static GameObject Cube(string name,Transform parent,Vector3 position,Vector3 scale,Material material)
            => Primitive(name,parent,PrimitiveType.Cube,position,scale,material);
        static GameObject Primitive(string name,Transform parent,PrimitiveType type,Vector3 position,Vector3 scale,Material material)
        {
            var obj=GameObject.CreatePrimitive(type);obj.name=name;obj.transform.SetParent(parent,false);
            obj.transform.localPosition=position;obj.transform.localScale=scale;obj.GetComponent<Renderer>().sharedMaterial=material;return obj;
        }

        static Mesh BoxMesh(Vector3 size)
        {
            const string path="Assets/Rooms/Memory/Materials/CardMesh.asset";
            var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(mesh!=null)return mesh;
            var primitive=GameObject.CreatePrimitive(PrimitiveType.Cube);mesh=UnityEngine.Object.Instantiate(primitive.GetComponent<MeshFilter>().sharedMesh);
            var vertices=mesh.vertices;for(int i=0;i<vertices.Length;i++)vertices[i]=Vector3.Scale(vertices[i],size);mesh.vertices=vertices;mesh.RecalculateBounds();
            UnityEngine.Object.DestroyImmediate(primitive);AssetDatabase.CreateAsset(mesh,path);return mesh;
        }

        static Mesh SymbolMesh(int symbol)
        {
            string path="Assets/Rooms/Memory/Materials/Symbol"+symbol+".asset";
            var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(mesh!=null)return mesh;
            int n=new[]{24,3,4,10,6,12}[symbol];var points=new Vector3[n+1];var triangles=new int[n*3];
            for(int i=0;i<n;i++)
            {
                float angle=(90f+i*360f/n)*Mathf.Deg2Rad;
                float radius=(symbol==3 && i%2==1)? .042f : (symbol==5 && i%3==1)?.045f:.09f;
                points[i+1]=new Vector3(Mathf.Cos(angle)*radius,Mathf.Sin(angle)*radius,0);
                triangles[i*3]=0;triangles[i*3+1]=(i+1)%n+1;triangles[i*3+2]=i+1;
            }
            mesh=new Mesh{name="Memory symbol "+symbol,vertices=points,triangles=triangles};mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,path);return mesh;
        }

        static TextMeshPro Text3D(string name,Transform parent,string text,Vector3 position,float width,float height,float size,Color color)
        {
            var label=new GameObject(name).AddComponent<TextMeshPro>();label.transform.SetParent(parent,false);label.transform.localPosition=position;
            label.rectTransform.sizeDelta=new Vector2(width,height);label.font=font;label.text=text;label.fontSize=size;label.color=color;label.alignment=TextAlignmentOptions.Center;
            return label;
        }

        static TextMeshProUGUI UIString(string name,Transform parent,string text,Vector2 position,Vector2 size,float fontSize,Color color)
        {
            var label=new GameObject(name,typeof(RectTransform)).AddComponent<TextMeshProUGUI>();label.transform.SetParent(parent,false);
            label.rectTransform.anchoredPosition=position;label.rectTransform.sizeDelta=size;label.font=font;label.text=text;label.fontSize=fontSize;label.color=color;
            label.alignment=TextAlignmentOptions.Center;label.raycastTarget=false;return label;
        }

        static Button UIButton(Transform parent,string text,Vector2 position,Vector2 size,Color color,out TMP_Text label)
        {
            var obj=new GameObject(text,typeof(RectTransform),typeof(Image),typeof(Button));obj.transform.SetParent(parent,false);
            var rect=obj.GetComponent<RectTransform>();rect.anchoredPosition=position;rect.sizeDelta=size;obj.GetComponent<Image>().color=color;
            label=UIString("Label",obj.transform,text,Vector2.zero,size-Vector2.one*12,25,Color.white);
            var button=obj.GetComponent<Button>();var colors=button.colors;colors.highlightedColor=new Color(1.2f,1.2f,1.2f);colors.pressedColor=new Color(.7f,.7f,.7f);button.colors=colors;return button;
        }

        static AudioSource Audio(string name,Transform parent,string clip,Vector3 position)
        {
            var source=new GameObject(name).AddComponent<AudioSource>();source.transform.SetParent(parent,false);source.transform.localPosition=position;
            source.clip=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Rooms/Memory/Audio/"+clip+".wav");source.loop=true;source.playOnAwake=false;source.spatialBlend=1;source.volume=.4f;
            source.minDistance=2;source.maxDistance=10;return source;
        }

        [MenuItem("FXR/Configure project for Quest")]
        public static void ConfigureProject()
        {
            EditorSettings.serializationMode=SerializationMode.ForceText;
            UnityEditor.VersionControlSettings.mode="Visible Meta Files";
            PlayerSettings.companyName="FXRGroup";PlayerSettings.productName="FXR Experiment Rooms";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,"fr.fxr.memoryrooms");
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel29;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android,false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,new[]{GraphicsDeviceType.Vulkan});
            var mobile=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/Mobile_RPAsset.asset");
            if(mobile!=null){mobile.supportsCameraDepthTexture=false;mobile.supportsCameraOpaqueTexture=false;mobile.msaaSampleCount=4;GraphicsSettings.defaultRenderPipeline=mobile;QualitySettings.renderPipeline=mobile;}
            XRGeneralSettingsPerBuildTarget settings;
            if(!EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey,out settings))
            {
                Directory.CreateDirectory("Assets/XR/Settings");AssetDatabase.Refresh();
                settings=ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
                AssetDatabase.CreateAsset(settings,"Assets/XR/Settings/XRGeneralSettingsPerBuildTarget.asset");
                EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey,settings,true);
            }
            if(!settings.HasManagerSettingsForBuildTarget(BuildTargetGroup.Android))settings.CreateDefaultManagerSettingsForBuildTarget(BuildTargetGroup.Android);
            settings.SettingsForBuildTarget(BuildTargetGroup.Android).InitManagerOnStart=true;
            bool assigned=XRPackageMetadataStore.AssignLoader(settings.ManagerSettingsForBuildTarget(BuildTargetGroup.Android),"UnityEngine.XR.OpenXR.OpenXRLoader",BuildTargetGroup.Android);
            if(!assigned)throw new Exception("Could not configure OpenXR loader for Android.");
            UnityEditor.XR.OpenXR.Features.FeatureHelpers.RefreshFeatures(BuildTargetGroup.Android);
            var packageSettings = AssetDatabase.LoadAssetAtPath<ScriptableObject>("Assets/XR/Settings/OpenXRPackageSettings.asset") as IPackageSettings;
            var openXR = packageSettings?.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
            if(openXR!=null)
            {
                openXR.renderMode=OpenXRSettings.RenderMode.SinglePassInstanced;
                foreach(var feature in openXR.GetFeatures())
                    if(feature is OculusTouchControllerProfile || feature.GetType().Name == "MetaQuestFeature") { feature.enabled=true; EditorUtility.SetDirty(feature); }
                EditorUtility.SetDirty(openXR);
            }
            EditorUtility.SetDirty(settings);AssetDatabase.SaveAssets();
        }

        public static void BuildMacPreview()
        {
            PlayerSettings.fullScreenMode=FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=800;
            var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="Builds/Mac/FXRPreview.app",target=BuildTarget.StandaloneOSX,options=BuildOptions.Development});
            if(result.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("Mac preview build failed: "+result.summary.result);
        }

        public static void FixPreviewPanel()
        {
            var root = PrefabUtility.LoadPrefabContents(RoomPath);
            var panel = root.transform.Find("Experiment panel");
            if (panel.GetComponent<GraphicRaycaster>() == null) panel.gameObject.AddComponent<GraphicRaycaster>();
            var title = root.transform.Find("Room title").GetComponent<TextMeshPro>();
            title.transform.localPosition = new Vector3(-.9f,2.83f,2.4f);
            title.rectTransform.sizeDelta = new Vector2(2.1f,.2f); title.fontSize=1f;
            var subtitle = root.transform.Find("Room subtitle").GetComponent<TextMeshPro>();
            subtitle.transform.localPosition = new Vector3(-.9f,2.68f,2.39f);
            subtitle.rectTransform.sizeDelta = new Vector2(2.1f,.15f); subtitle.fontSize=.6f;
            PrefabUtility.SaveAsPrefabAsset(root,RoomPath);
            PrefabUtility.UnloadPrefabContents(root);
            AssetDatabase.SaveAssets();
        }

        public static void FixAndBuildMacPreview()
        {
            FixPreviewPanel();
            BuildMacPreview();
        }

        public static void BuildQuest()
        {
            ConfigureProject();
            var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},locationPathName="Builds/Quest/FXRMemory.apk",target=BuildTarget.Android,options=BuildOptions.Development});
            if(result.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("Quest build failed: "+result.summary.result);
        }
    }
}
