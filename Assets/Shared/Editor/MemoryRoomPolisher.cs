using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using FXR.Memory;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace FXR.Editor
{
    // Upgrades only the owner's development root, then integrates it into the existing MainScene.
    public static class MemoryRoomPolisher
    {
        const string Base = "Assets/Rooms/Memory/";
        const string Main = "Assets/Shared/Scenes/MainScene.unity";
        const string Prefab = Base + "Prefabs/MemoryRoom.prefab";
        static TMP_FontAsset font;
        static Material ivory, charcoal, oak, metal, paper, cyan, amber, leafA, leafB, leafC, clay, stone, water, skyOffice, skyNature;

        [MenuItem("FXR/Polish and integrate Memory Room")]
        public static void PolishAndIntegrate()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Directory.CreateDirectory("Builds/Backups");
            File.Copy(DevelopmentSceneBuilder.MemoryScene, "Builds/Backups/MemoryDev-before-polish.unity", true);
            Directory.CreateDirectory(Base + "Art/Materials");
            Directory.CreateDirectory(Base + "Art/Meshes");
            Directory.CreateDirectory(Base + "Art/Textures");
            AssetDatabase.Refresh();
            MemorySourceAssets.Prepare();
            font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(
                AssetDatabase.FindAssets("LiberationSans SDF t:TMP_FontAsset")[0]));
            MakeMaterials();
            var scene = EditorSceneManager.OpenScene(DevelopmentSceneBuilder.MemoryScene, OpenSceneMode.Single);
            var root = scene.GetRootGameObjects().Single(x => x.GetComponent<MemoryRoomController>() != null);
            var room = root.GetComponent<MemoryRoomController>();
            if (PrefabUtility.IsPartOfPrefabInstance(root)) throw new Exception("Polish the unpacked owner scene, not an integration instance.");
            Remove(root.transform, "Office props", "Nature props", "Experiment panel", "Office ambience", "Nature ambience", "Ceiling lamp", "Room finish", "Soundscape");
            if (room.atmosphere != null) UnityEngine.Object.DestroyImmediate(room.atmosphere);
            room.atmosphere = root.AddComponent<MemoryAtmosphere>();
            var finish = Group("Room finish", root.transform);
            UpgradeTask(room, finish);
            BuildArchitecture(root.transform, finish);
            room.officeProps = BuildOffice(root.transform);
            room.natureProps = BuildNature(root.transform);
            BuildSound(room);
            BuildPanel(root.transform, room);
            room.natureProps.SetActive(false);
            room.officeProps.SetActive(true);
            EditorSceneManager.SaveScene(scene);
            PrefabUtility.SaveAsPrefabAsset(root, Prefab);
            AssetDatabase.SaveAssets();
            // Keep all shared entry/players and teammate slots as they are; prefab GUID is preserved.
            var main = EditorSceneManager.OpenScene(Main, OpenSceneMode.Single);
            var instance = main.GetRootGameObjects().Single(x => x.GetComponent<MemoryRoomController>() != null);
            if (PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(instance) != Prefab)
                throw new Exception("MainScene memory root is not the expected prefab; do not replace team work.");
            EditorSceneManager.SaveScene(main);
            AssetDatabase.SaveAssets();
            ValidatePolishedRoom();
            EditorSceneManager.OpenScene(DevelopmentSceneBuilder.MemoryScene, OpenSceneMode.Single);
            Debug.Log("Memory Room polished in its owner scene and integrated into existing MainScene.");
        }

        static void MakeMaterials()
        {
            ivory = Mat("Porcelain", new Color(.82f, .82f, .77f), .25f);
            charcoal = Mat("Graphite", new Color(.085f, .12f, .13f), .2f);
            oak = MemorySourceAssets.Surface("wood_table_001",Vector2.one);
            metal = Mat("Brushed metal", new Color(.3f, .36f, .38f), .5f); metal.SetFloat("_Metallic", .25f);
            paper = Mat("Paper", new Color(.94f, .94f, .88f), .05f);
            cyan = Mat("Screen glow", new Color(.24f, .65f, .79f), 0, null, true);
            amber = Mat("Warm glow", new Color(.93f, .8f, .53f), 0, null, true);
            leafA = Mat("Leaves deep", new Color(.12f, .3f, .18f), .12f);
            leafB = Mat("Leaves sage", new Color(.25f, .46f, .24f), .12f);
            leafC = Mat("Leaves light", new Color(.4f, .57f, .28f), .12f);
            clay = Mat("Terracotta", new Color(.61f, .35f, .24f), .1f);
            stone = Mat("River stones", new Color(.47f, .5f, .45f), .12f);
            water = Mat("Water", new Color(.19f, .43f, .43f), .7f);
            skyOffice = Mat("Office sky", new Color(.65f, .8f, .88f), 0, null, true);
            skyNature = Mat("Garden sky", new Color(.76f, .86f, .8f), 0, null, true);
        }

        static void UpgradeTask(MemoryRoomController room, Transform finish)
        {
            var root = room.transform;
            root.Find("Table").GetComponent<Renderer>().sharedMaterial = oak;
            root.Find("Floor").GetComponent<Renderer>().sharedMaterial = MemorySourceAssets.Surface("floor_tiles_06",new Vector2(4,4));
            root.Find("Memory board").GetComponent<Renderer>().sharedMaterial = charcoal;
            foreach (var renderer in room.walls) renderer.sharedMaterial = MemorySourceAssets.Surface("painted_plaster_wall",new Vector2(3,2));
            foreach (string name in new[] { "Front Left", "Front Right" }) root.Find(name).GetComponent<Renderer>().sharedMaterial = room.walls[0].sharedMaterial;
            root.Find("Door Header").GetComponent<Renderer>().sharedMaterial = oak;
            var title = root.Find("Room title").GetComponent<TextMeshPro>();
            title.text = "MEMORY / ENVIRONMENT"; title.fontSize = .9f; title.color = new Color(.88f, .94f, .91f);
            title.transform.localPosition = new Vector3(-.9f, 2.94f, 2.35f);
            var subtitle = root.Find("Room subtitle").GetComponent<TextMeshPro>();
            subtitle.text = "Find the pair. Notice the atmosphere."; subtitle.fontSize = .49f; subtitle.color = new Color(.62f,.77f,.72f);
            subtitle.transform.localPosition = new Vector3(-.9f, 2.78f, 2.34f);
            Cube("Task title plaque",finish,new Vector3(-.9f,2.86f,2.41f),new Vector3(2.32f,.4f,.07f),charcoal);
            // Constant task surface, symbol contrast and key light in both conditions.
            room.roomLight.transform.localPosition = new Vector3(-.5f, 3.15f, .6f);
            room.roomLight.transform.localRotation = Quaternion.Euler(35, 0, 0);
            room.roomLight.type = LightType.Spot; room.roomLight.spotAngle = 100;
            room.roomLight.range = 5; room.roomLight.intensity = 3.8f;
            room.roomLight.color = new Color(1, .98f, .93f); room.roomLight.shadows = LightShadows.None;
            var back = Mat("Card back refined", new Color(.1f, .2f, .22f), .15f);
            var front = Mat("Card face refined", new Color(.96f, .95f, .88f), 0, null, true);
            var focus = Mat("Card focus", new Color(.73f, .87f, .74f), 0, null, true);
            foreach (var card in room.cards)
            {
                card.faceDownMaterial = back; card.faceUpMaterial = front;
                card.cardRenderer.sharedMaterial = back;
                Remove(card.transform, "Hover frame", "Card back mark");
                var hover = Group("Hover frame", card.transform);
                Cube("Outline top", hover, new Vector3(0,.182f,-.032f), new Vector3(.318f,.012f,.007f), focus);
                Cube("Outline bottom", hover, new Vector3(0,-.182f,-.032f), new Vector3(.318f,.012f,.007f), focus);
                Cube("Outline left", hover, new Vector3(-.155f,0,-.032f), new Vector3(.012f,.365f,.007f), focus);
                Cube("Outline right", hover, new Vector3(.155f,0,-.032f), new Vector3(.012f,.365f,.007f), focus);
                card.hoverFrame = hover.gameObject; hover.gameObject.SetActive(false);
                var mark = Group("Card back mark", card.transform);
                card.backDecoration = mark.gameObject;
                Cube("Center line", mark, new Vector3(0,.014f,-.03f), new Vector3(.034f,.064f,.002f), metal);
                var label = card.transform.Find("Card number").GetComponent<TextMeshPro>();
                label.color = new Color(.66f, .79f, .78f); label.fontSize = .34f;
            }
            Cube("Board top rim", finish, new Vector3(-.9f,2.64f,2.38f), new Vector3(1.74f,.04f,.13f), oak);
            foreach (float x in new[] { -1.75f, -.05f }) Cube("Board side rim", finish, new Vector3(x,1.91f,2.38f), new Vector3(.045f,1.46f,.13f), oak);
            Cube("Board lower rim", finish, new Vector3(-.9f,1.18f,2.38f), new Vector3(1.74f,.035f,.13f), oak);
            Cube("Task rug", finish, new Vector3(0,.004f,.55f), new Vector3(3.65f,.009f,3.3f), Mat("Woven rug", new Color(.31f,.38f,.37f), .05f, Grain("Woven rug", false)));
            Cube("Standing marker", finish, new Vector3(-.05f,.013f,.25f), new Vector3(.72f,.006f,.022f), amber);
            Text("Floor hint", finish, "STAND HERE", new Vector3(-.05f,.025f,.04f), 1.15f,.15f,.48f,new Color(.8f,.86f,.8f)).transform.localRotation = Quaternion.Euler(90,0,0);
            Cube("Console cabinet", finish, new Vector3(1.29f,.52f,2.48f), new Vector3(1.2f,1.04f,.42f), oak);
            Cube("Console plinth", finish, new Vector3(1.29f,.04f,2.48f), new Vector3(1.24f,.08f,.45f), charcoal);
            Combine(finish, "Common finish");
        }

        static void BuildArchitecture(Transform root, Transform finish)
        {
            Cube("Ceiling", finish, new Vector3(0,3.58f,2), new Vector3(8,.12f,8), ivory);
            foreach (float x in new[] { -3.94f, 3.94f })
                Cube("Wall base rail", finish, new Vector3(x,.095f,2), new Vector3(.07f,.19f,7.9f), oak);
            Cube("Back base rail", finish, new Vector3(0,.095f,5.9f), new Vector3(7.9f,.19f,.08f), oak);
            foreach (float x in new[] { -2.4f, 0f, 2.4f })
            {
                Cube("Ceiling fixture", finish, new Vector3(x,3.45f,2), new Vector3(1.45f,.075f,.45f), metal);
                Cube("Diffuser", finish, new Vector3(x,3.4f,2), new Vector3(1.36f,.018f,.38f), paper);
            }
            Combine(finish, "Common finish");
        }

        static GameObject BuildOffice(Transform parent)
        {
            var root = Group("Office props", parent);
            var blue = Mat("Office felt", new Color(.28f,.37f,.43f), .05f);
            var screen = Mat("Screen dark", new Color(.055f,.13f,.18f), 0, null, true);
            BuildWindow(root, false);
            for (int i = 0; i < 3; i++)
            {
                float x = -3.92f;
                Cube("Acoustic tile",root,new Vector3(x,1.72f,.5f+i*1.35f),new Vector3(.065f,1.35f,1.02f),blue);
                for (int j = 0; j < 6; j++) Cube("Tile seam",root,new Vector3(x+.038f,1.2f+j*.19f,.5f+i*1.35f),new Vector3(.008f,.018f,.98f),metal);
            }
            Desk(root, new Vector3(-2.5f,0,4.65f), "Workstation A", screen);
            Desk(root, new Vector3(2.5f,0,4.65f), "Workstation B", screen);
            Chair(root,new Vector3(-2.5f,0,3.62f),blue);
            Chair(root,new Vector3(2.5f,0,3.62f),blue);
            // Shelves, files, printer and wall clock make the office read as an occupied workspace.
            Cube("Storage cabinet",root,new Vector3(3.58f,.74f,1.15f),new Vector3(.65f,1.48f,1.6f),ivory);
            for(int i=0;i<3;i++)
            {
                Cube("Cabinet door",root,new Vector3(3.235f,.74f,.6f+i*.5f),new Vector3(.025f,1.34f,.46f),blue);
                Cube("Cabinet handle",root,new Vector3(3.212f,.95f,.77f+i*.5f),new Vector3(.016f,.15f,.025f),metal);
            }
            Cube("Printer",root,new Vector3(3.56f,1.63f,.95f),new Vector3(.51f,.3f,.55f),charcoal);
            Cube("Printer feed",root,new Vector3(3.56f,1.796f,.95f),new Vector3(.44f,.02f,.38f),paper);
            MemorySourceAssets.Place("wall_clock",root,new Vector3(3.83f,2.42f,3.8f),.48f,-90);
            var notice = Cube("Notice board",root,new Vector3(-3.9f,2.15f,4.92f),new Vector3(.045f,1.2f,1.1f),charcoal);
            for(int i=0;i<6;i++) Cube("Pinned note",root,new Vector3(-3.87f,1.85f+(i/3)*.47f,4.56f+(i%3)*.32f),new Vector3(.008f,.33f,.25f),i%2==0?paper:amber);
            for(int i=0;i<8;i++) Cube("Ceiling acoustic baffle",root,new Vector3(-3.6f+i*1.02f,3.28f,4.2f),new Vector3(.055f,.3f,2.25f),blue);
            PointLight("Office fill",root,new Vector3(0,2.8f,4.7f),new Color(.7f,.85f,1),2f,4.6f);
            Combine(root,"Office architecture and furniture");
            return root.gameObject;
        }

        static void Desk(Transform root, Vector3 p, string name, Material screen)
        {
            var group=Group(name,root); group.localPosition=p;
            MemorySourceAssets.Place("metal_office_desk",group,Vector3.zero,.825f,0,true);
            MemorySourceAssets.Place("desk_lamp_arm_01",group,new Vector3(.58f,.828f,.25f),.5f,160);
            Cube("Monitor frame",group,new Vector3(0,1.22f,.19f),new Vector3(.85f,.48f,.045f),charcoal);
            Cube("Display",group,new Vector3(0,1.22f,.164f),new Vector3(.79f,.418f,.012f),screen);
            Cube("Monitor neck",group,new Vector3(0,.94f,.19f),new Vector3(.055f,.22f,.055f),metal);
            Cube("Monitor foot",group,new Vector3(0,.844f,.14f),new Vector3(.3f,.03f,.18f),metal);
            Cube("Screen header",group,new Vector3(0,1.4f,.156f),new Vector3(.75f,.04f,.004f),cyan);
            for(int i=0;i<5;i++)
            {
                Cube("Screen line",group,new Vector3(-.17f,1.3f-i*.046f,.155f),new Vector3(.27f+.027f*(i%3),.012f,.003f),i%3==0?cyan:metal);
                Cube("Screen chart",group,new Vector3(.1f+i*.042f,1.1f+i*.027f,.155f),new Vector3(.027f,.04f+i*.037f,.003f),cyan);
            }
            Cube("Keyboard body",group,new Vector3(0,.84f,-.2f),new Vector3(.53f,.025f,.19f),charcoal);
            for(int row=0;row<3;row++) for(int key=0;key<12;key++)
                Cube("Key",group,new Vector3(-.235f+key*.042f,.856f,-.255f+row*.049f),new Vector3(.032f,.01f,.033f),metal);
            Cube("Mouse mat",group,new Vector3(.49f,.833f,-.2f),new Vector3(.23f,.008f,.26f),charcoal);
            Sphere("Mouse",group,new Vector3(.49f,.854f,-.2f),new Vector3(.07f,.035f,.11f),metal);
            for(int i=0;i<5;i++) Cube("Document",group,new Vector3(-.68f,.84f+i*.006f,.15f),new Vector3(.25f,.005f,.32f),paper);
            Cylinder("Mug",group,new Vector3(.67f,.895f,.13f),new Vector3(.09f,.069f,.09f),ivory);
            Cylinder("Coffee",group,new Vector3(.67f,.966f,.13f),new Vector3(.075f,.001f,.075f),charcoal);
            var files = new[]{cyan,blueFile(),ivory};
            for(int i=0;i<3;i++) Cube("Binder",group,new Vector3(-.62f+i*.056f,1.07f,.32f),new Vector3(.046f,.39f,.23f),files[i]);
            Cube("Desk phone",group,new Vector3(.69f,.869f,-.25f),new Vector3(.17f,.06f,.19f),charcoal);
            Cube("Phone handset",group,new Vector3(.69f,.913f,-.22f),new Vector3(.205f,.04f,.06f),metal);
        }
        static Material blueFile() => Mat("Binder blue",new Color(.2f,.32f,.42f),.15f);

        static void Chair(Transform root, Vector3 p, Material upholstery)
        {
            MemorySourceAssets.Place("modern_arm_chair_01",root,p,1.02f,180,true);
        }

        static GameObject BuildNature(Transform parent)
        {
            var root=Group("Nature props",parent);
            BuildWindow(root,true);
            for(int i=0;i<23;i++)
                Cube("Oak wall slat",root,new Vector3(3.89f,1.72f,-.6f+i*.25f),new Vector3(.07f,2.9f,.09f),oak);
            for(int i=0;i<8;i++) Cube("Ceiling timber",root,new Vector3(-3.55f+i*1.02f,3.28f,4.2f),new Vector3(.07f,.24f,2.25f),oak);
            MemorySourceAssets.Place("potted_plant_02",root,new Vector3(-3.02f,0,.4f),1.45f,0,true);
            MemorySourceAssets.Place("potted_plant_02",root,new Vector3(2.98f,0,.15f),1.25f,85,true);
            MemorySourceAssets.Place("potted_plant_01",root,new Vector3(-3.15f,0,3.72f),1.9f,20,true);
            MemorySourceAssets.Place("potted_plant_02",root,new Vector3(3.1f,0,4.25f),1.85f,150,true);
            Cube("Garden bed rim",root,new Vector3(0,.2f,5.12f),new Vector3(4.2f,.4f,1.15f),stone,true);
            Cube("Soil",root,new Vector3(0,.409f,5.12f),new Vector3(4.04f,.025f,1.02f),Mat("Garden soil",new Color(.22f,.25f,.18f),.05f));
            MemorySourceAssets.Place("potted_plant_04",root,new Vector3(.15f,.42f,5.15f),1.05f,95);
            MemorySourceAssets.Place("potted_plant_04",root,new Vector3(1.62f,.42f,5.1f),.95f,190);
            MemorySourceAssets.Place("potted_plant_04",root,new Vector3(-1.3f,.42f,5.1f),.82f,20);
            for(int i=0;i<36;i++)
            {
                float x=-1.95f+(i%12)*.35f,z=4.77f+(i/12)*.29f;
                var rock=Sphere("Pebble",root,new Vector3(x,.45f,z),new Vector3(.14f+.04f*(i%3),.07f,.11f),stone);
                rock.localRotation=Quaternion.Euler(i*13,i*29,0);
            }
            Cube("Water feature base",root,new Vector3(-2.9f,.15f,4.93f),new Vector3(.85f,.3f,.9f),stone,true);
            Cube("Water surface",root,new Vector3(-2.9f,.308f,4.93f),new Vector3(.73f,.012f,.78f),water);
            var ripple=Mat("Water highlights",new Color(.47f,.66f,.59f),0,null,true);
            for(int i=0;i<4;i++) Cube("Water ripple",root,new Vector3(-2.95f+i*.09f,.318f,4.75f+i*.095f),new Vector3(.18f,.002f,.012f),ripple);
            Cube("Fountain spout",root,new Vector3(-2.9f,.56f,5.28f),new Vector3(.055f,.5f,.07f),oak);
            Cube("Falling water",root,new Vector3(-2.9f,.45f,5.245f),new Vector3(.04f,.29f,.01f),ripple);
            Cube("Oak bench",root,new Vector3(-3.55f,.44f,2.04f),new Vector3(.49f,.08f,1.45f),oak,true);
            foreach(float z in new[]{1.46f,2.6f}) Cube("Bench support",root,new Vector3(-3.55f,.21f,z),new Vector3(.35f,.42f,.08f),charcoal);
            PointLight("Garden fill",root,new Vector3(0,2.8f,4.7f),new Color(1f,.88f,.65f),2f,4.6f);
            Combine(root,"Garden architecture and planting");
            return root.gameObject;
        }

        static void Plant(Transform root, Vector3 p, float height, int variation)
        {
            var group=Group("Broadleaf plant",root);group.localPosition=p;
            var profile=new[]{new Vector2(0,0),new Vector2(.22f,0),new Vector2(.3f,.42f),new Vector2(.33f,.44f),new Vector2(.33f,.49f),new Vector2(.27f,.49f),new Vector2(.24f,.41f)};
            MeshObject("Tapered ceramic planter",group,Lathe(profile,24),variation%2==0?clay:ivory);
            Cylinder("Soil",group,new Vector3(0,.445f,0),new Vector3(.5f,.01f,.5f),charcoal);
            Between("Plant stem",group,new Vector3(0,.44f,0),new Vector3(.04f,height,0),.018f,leafA);
            for(int i=0;i<17;i++)
            {
                float angle=(i*137.5f+variation*31)*Mathf.Deg2Rad;
                float h=.58f+i*(height-.62f)/17;
                Vector3 a=new Vector3(.02f,h,0);
                Vector3 dir=new Vector3(Mathf.Cos(angle),.4f+Mathf.Sin(i)*.18f,Mathf.Sin(angle)).normalized;
                Leaf(group,a,dir,.53f+(i%3)*.09f,.21f,new[]{leafA,leafB,leafC}[i%3]);
            }
        }

        static void Tree(Transform root, Vector3 p, float h, int seed)
        {
            var group=Group("Indoor garden tree",root);group.localPosition=p;
            Between("Tree trunk",group,Vector3.zero,new Vector3(.11f,h*.76f,0),.045f,oak);
            for(int i=0;i<9;i++)
            {
                float a=(i*137+seed*23)*Mathf.Deg2Rad;
                Vector3 start=new Vector3(.06f,h*.4f+i*h*.045f,0);
                Vector3 end=start+new Vector3(Mathf.Cos(a)*.45f,.22f,Mathf.Sin(a)*.45f);
                Between("Tree branch",group,start,end,.013f,oak);
                for(int j=0;j<7;j++)
                    Leaf(group,end+Vector3.up*(j%3)*.05f,new Vector3(Mathf.Cos(a+j*1.9f),.38f,Mathf.Sin(a+j*1.9f)).normalized,.36f,.15f,new[]{leafA,leafB,leafC}[(i+j)%3]);
            }
        }

        static void BuildWindow(Transform root, bool nature)
        {
            var backdrop=Mat(nature?"Photographed forest":"Photographed city",Color.white,0,MemorySourceAssets.Window(nature),true);
            var quad=new Mesh{
                vertices=new[]{new Vector3(-2.975f,.86f,5.84f),new Vector3(2.975f,.86f,5.84f),new Vector3(2.975f,3.24f,5.84f),new Vector3(-2.975f,3.24f,5.84f)},
                uv=new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up},triangles=new[]{0,2,1,0,3,2}};
            quad.RecalculateNormals();MeshObject("Photographed window view",root,quad,backdrop);
            foreach(float x in new[]{-3.03f,0,3.03f}) Cube("Window mullion",root,new Vector3(x,2.05f,5.73f),new Vector3(.065f,2.55f,.11f),nature?oak:metal);
            foreach(float y in new[]{.78f,3.32f}) Cube("Window frame",root,new Vector3(0,y,5.73f),new Vector3(6.13f,.07f,.11f),nature?oak:metal);
            Cube("Window sill",root,new Vector3(0,.735f,5.62f),new Vector3(6.2f,.065f,.38f),nature?oak:ivory);
            if(!nature) for(int i=0;i<10;i++)
                Cube("Venetian blind",root,new Vector3(0,3.18f-i*.075f,5.67f),new Vector3(5.96f,.034f,.08f),ivory);
        }

        static void BuildSound(MemoryRoomController room)
        {
            var root=Group("Soundscape",room.transform);
            room.officeAudio=Audio("Recorded office ambience",root,"Office",new Vector3(0,2.1f,4.6f),.8f);
            room.officeAudio.spatialBlend=0;
            var keys=Audio("Recorded keyboard",root,"OfficeDetail",new Vector3(-2.5f,1.1f,4.5f),.8f);
            room.natureAudio=Audio("Recorded forest birds",root,"Nature",new Vector3(0,2.1f,5.6f),.8f);
            room.natureAudio.spatialBlend=0;
            var stream=Audio("Recorded running water",root,"NatureDetail",new Vector3(-2.9f,.55f,4.9f),.8f);
            room.atmosphere.officeSources=new[]{room.officeAudio,keys};
            room.atmosphere.natureSources=new[]{room.natureAudio,stream};
            var feedback=new GameObject("Neutral task feedback").AddComponent<AudioSource>();
            feedback.transform.SetParent(root,false);feedback.transform.localPosition=new Vector3(-.9f,1.9f,2.25f);
            feedback.playOnAwake=false;feedback.spatialBlend=1;feedback.minDistance=2;feedback.maxDistance=8;feedback.volume=.6f;
            room.atmosphere.feedbackSource=feedback;
            room.atmosphere.flipClip=Clip("Flip");room.atmosphere.matchClip=Clip("Match");room.atmosphere.mismatchClip=Clip("Mismatch");
        }
        static AudioClip Clip(string name) => AssetDatabase.LoadAssetAtPath<AudioClip>(Base+"Audio/"+name+".wav");
        static AudioSource Audio(string name,Transform parent,string clip,Vector3 p,float level)
        {
            var source=new GameObject(name).AddComponent<AudioSource>();source.transform.SetParent(parent,false);source.transform.localPosition=p;
            source.clip=Clip(clip);if(source.clip==null)throw new Exception("Missing room audio: "+clip);
            source.loop=true;source.playOnAwake=false;source.spatialBlend=1;source.volume=level;source.rolloffMode=AudioRolloffMode.Linear;
            source.minDistance=3;source.maxDistance=12;source.dopplerLevel=0;return source;
        }

        static void BuildPanel(Transform parent,MemoryRoomController room)
        {
            var go=new GameObject("Experiment panel",typeof(RectTransform),typeof(Canvas),typeof(TrackedDeviceGraphicRaycaster),typeof(GraphicRaycaster));
            go.transform.SetParent(parent,false);go.transform.localPosition=new Vector3(1.29f,1.84f,2.18f);go.transform.localScale=Vector3.one*.0016f;
            var canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;go.GetComponent<RectTransform>().sizeDelta=new Vector2(800,1120);
            var panel=go.transform;
            Rect("Panel background",panel,Vector2.zero,new Vector2(800,1120),new Color(.045f,.095f,.11f,1));
            Rect("Accent rail",panel,new Vector2(-395,0),new Vector2(10,1120),new Color(.48f,.72f,.64f));
            Label("Header",panel,"MEMORY LAB",new Vector2(0,486),new Vector2(700,70),48,Color.white);
            room.phaseText=Label("Phase",panel,"PREVIEW / OFFICE",new Vector2(0,417),new Vector2(700,50),27,new Color(.6f,.78f,.74f));
            Rect("Header divider",panel,new Vector2(0,377),new Vector2(700,2),new Color(.23f,.35f,.36f));
            room.statusText=Label("Instructions",panel,"<b>Find 6 matching pairs.</b>\nSelect two cards to reveal their symbols.\nYou have 3 minutes in each environment.\nAfter each round, rate your mental effort.",new Vector2(0,262),new Vector2(700,200),32,Color.white);
            room.feedbackText=Label("Interaction hint",panel,"VR: point + trigger  /  Desktop: left click\nMove: thumbstick  /  Desktop: WASD + right drag",new Vector2(0,103),new Vector2(700,85),25,new Color(.69f,.8f,.79f));
            room.timerFill=Bar("Time",panel,new Vector2(0,171),new Color(.44f,.78f,.72f));
            room.progressFill=Bar("Pairs",panel,new Vector2(0,33),new Color(.78f,.84f,.57f));
            room.timerFill.transform.parent.gameObject.SetActive(false);room.progressFill.transform.parent.gameObject.SetActive(false);
            room.actionButton=Button(panel,"START ROUND 1",new Vector2(0,-10),new Vector2(700,85),new Color(.18f,.44f,.39f),out room.actionText);
            UnityEventTools.AddPersistentListener(room.actionButton.onClick,room.StartOrContinue);
            room.setupPanel=UIGroup("Setup controls",panel);
            room.orderButton=Button(room.setupPanel.transform,"OFFICE  >  NATURE  ·  CHANGE ORDER",new Vector2(0,-126),new Vector2(700,72),new Color(.18f,.26f,.28f),out room.orderText);
            UnityEventTools.AddPersistentListener(room.orderButton.onClick,room.ToggleOrder);
            room.officePreviewButton=Button(room.setupPanel.transform,"VIEW OFFICE",new Vector2(-181,-215),new Vector2(338,72),new Color(.22f,.3f,.35f),out _);
            UnityEventTools.AddPersistentListener(room.officePreviewButton.onClick,room.PreviewOffice);
            room.naturePreviewButton=Button(room.setupPanel.transform,"VIEW NATURE",new Vector2(181,-215),new Vector2(338,72),new Color(.22f,.35f,.27f),out _);
            UnityEventTools.AddPersistentListener(room.naturePreviewButton.onClick,room.PreviewNature);
            room.soundButton=Button(room.setupPanel.transform,"SOUND ON",new Vector2(0,-305),new Vector2(700,65),new Color(.16f,.23f,.25f),out room.soundText);
            UnityEventTools.AddPersistentListener(room.soundButton.onClick,room.ToggleSound);
            Label("Setup note",room.setupPanel.transform,"Preview is optional. The order above controls round 1.\nSound and order stay fixed once the session starts.",new Vector2(0,-393),new Vector2(700,70),24,new Color(.62f,.72f,.72f));
            room.ratingPanel=UIGroup("Effort rating",panel);
            Label("Rating title",room.ratingPanel.transform,"MENTAL EFFORT",new Vector2(0,0),new Vector2(700,50),32,Color.white);
            for(int i=1;i<=5;i++)
            {
                var button=Button(room.ratingPanel.transform,i.ToString(),new Vector2((i-3)*143,-97),new Vector2(128,98),new Color(.27f,.38f,.36f),out var label);
                label.fontSize=42;
                UnityEventTools.AddIntPersistentListener(button.onClick,room.RateEffort,i);
            }
            Label("Rating anchors",room.ratingPanel.transform,"1 = very low                       5 = very high",new Vector2(0,-182),new Vector2(700,60),28,new Color(.75f,.84f,.81f));
            Label("Rating note",room.ratingPanel.transform,"Your own impression, without a right or wrong answer.",new Vector2(0,-280),new Vector2(700,90),27,new Color(.65f,.76f,.74f));
            room.ratingPanel.SetActive(false);
            room.resultsPanel=UIGroup("Final results",panel);
            room.resultText=Label("Results",room.resultsPanel.transform,"",new Vector2(0,-205),new Vector2(700,360),30,new Color(.79f,.89f,.84f));
            room.resultsPanel.SetActive(false);
            Rect("Footer divider",panel,new Vector2(0,-457),new Vector2(700,2),new Color(.23f,.35f,.36f));
            var reset=Button(panel,"RESET SESSION",new Vector2(0,-510),new Vector2(700,68),new Color(.27f,.2f,.19f),out _);
            UnityEventTools.AddPersistentListener(reset.onClick,room.RequestReset);
            room.resetConfirmation=UIGroup("Reset confirmation",panel);
            // Opaque overlay catches rays/clicks before controls behind it.
            Rect("Modal blocker",room.resetConfirmation.transform,Vector2.zero,new Vector2(800,1120),new Color(.045f,.095f,.11f,1));
            Label("Reset heading",room.resetConfirmation.transform,"Restart this session?",new Vector2(0,180),new Vector2(700,130),45,Color.white);
            Label("Reset explanation",room.resetConfirmation.transform,"The current rounds and ratings will be cleared.\nAlready saved sessions stay on this device.\nThe timer is paused while this message is open.",new Vector2(0,20),new Vector2(700,200),32,new Color(.76f,.84f,.82f));
            var cancel=Button(room.resetConfirmation.transform,"KEEP PLAYING",new Vector2(0,-165),new Vector2(700,85),new Color(.18f,.44f,.39f),out _);
            UnityEventTools.AddPersistentListener(cancel.onClick,room.CancelReset);
            var confirm=Button(room.resetConfirmation.transform,"YES, RESET",new Vector2(0,-275),new Vector2(700,75),new Color(.39f,.24f,.21f),out _);
            UnityEventTools.AddPersistentListener(confirm.onClick,room.ResetRoom);
            room.resetConfirmation.SetActive(false);
        }

        static Image Bar(string name,Transform parent,Vector2 p,Color color)
        {
            var back=Rect(name+" track",parent,p,new Vector2(700,9),new Color(.15f,.24f,.25f));
            var image=Rect(name+" fill",back.transform,Vector2.zero,new Vector2(700,9),color);
            image.sprite=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            image.type=Image.Type.Filled;image.fillMethod=Image.FillMethod.Horizontal;image.fillOrigin=0;image.fillAmount=0;return image;
        }
        static GameObject UIGroup(string name,Transform parent)
        {
            var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);
            go.GetComponent<RectTransform>().sizeDelta=new Vector2(800,1120);return go;
        }
        static Image Rect(string name,Transform parent,Vector2 p,Vector2 size,Color color)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);
            go.GetComponent<RectTransform>().anchoredPosition=p;go.GetComponent<RectTransform>().sizeDelta=size;
            var image=go.GetComponent<Image>();image.color=color;return image;
        }
        static TextMeshProUGUI Label(string name,Transform parent,string text,Vector2 p,Vector2 size,float fontSize,Color color)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(TextMeshProUGUI));go.transform.SetParent(parent,false);
            var label=go.GetComponent<TextMeshProUGUI>();label.rectTransform.anchoredPosition=p;label.rectTransform.sizeDelta=size;
            label.font=font;label.text=text;label.fontSize=fontSize;label.color=color;label.alignment=TextAlignmentOptions.Center;
            label.raycastTarget=false;label.enableWordWrapping=true;return label;
        }
        static Button Button(Transform parent,string text,Vector2 p,Vector2 size,Color color,out TMP_Text label)
        {
            var image=Rect(text,parent,p,size,color);var button=image.gameObject.AddComponent<Button>();
            label=Label("Label",image.transform,text,Vector2.zero,size-new Vector2(24,12),29,Color.white);
            var colors=button.colors;colors.highlightedColor=new Color(1.22f,1.22f,1.22f);colors.pressedColor=new Color(.72f,.85f,.8f);colors.disabledColor=new Color(.42f,.47f,.47f);button.colors=colors;
            var navigation=button.navigation;navigation.mode=Navigation.Mode.None;button.navigation=navigation;return button;
        }

        static Transform Group(string name,Transform parent) {var go=new GameObject(name);go.transform.SetParent(parent,false);return go.transform;}
        static void Remove(Transform parent,params string[] names) {foreach(string name in names){var child=parent.Find(name);if(child!=null)UnityEngine.Object.DestroyImmediate(child.gameObject);}}
        static Transform Cube(string name,Transform parent,Vector3 p,Vector3 scale,Material material,bool collider=false) => Primitive(name,parent,PrimitiveType.Cube,p,scale,material,collider);
        static Transform Cylinder(string name,Transform parent,Vector3 p,Vector3 scale,Material material) => Primitive(name,parent,PrimitiveType.Cylinder,p,scale,material,false);
        static Transform Sphere(string name,Transform parent,Vector3 p,Vector3 scale,Material material) => Primitive(name,parent,PrimitiveType.Sphere,p,scale,material,false);
        static Transform Primitive(string name,Transform parent,PrimitiveType type,Vector3 p,Vector3 scale,Material material,bool collider)
        {
            var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(parent,false);go.transform.localPosition=p;go.transform.localScale=scale;
            var renderer=go.GetComponent<Renderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;
            if(!collider)UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());return go.transform;
        }
        static void Between(string name,Transform parent,Vector3 a,Vector3 b,float radius,Material mat)
        {
            var t=Cylinder(name,parent,(a+b)*.5f,new Vector3(radius*2,(b-a).magnitude*.5f,radius*2),mat);
            t.localRotation=Quaternion.FromToRotation(Vector3.up,b-a);
        }
        static void PointLight(string name,Transform parent,Vector3 position,Color color,float intensity,float range)
        {
            var light=new GameObject(name).AddComponent<Light>();light.transform.SetParent(parent,false);light.transform.localPosition=position;
            light.type=LightType.Point;light.color=color;light.intensity=intensity;light.range=range;light.shadows=LightShadows.None;
        }
        static TextMeshPro Text(string name,Transform parent,string text,Vector3 p,float width,float height,float size,Color color)
        {
            var go=new GameObject(name,typeof(TextMeshPro));go.transform.SetParent(parent,false);go.transform.localPosition=p;
            var label=go.GetComponent<TextMeshPro>();label.font=font;label.text=text;label.fontSize=size;label.color=color;label.alignment=TextAlignmentOptions.Center;label.rectTransform.sizeDelta=new Vector2(width,height);return label;
        }
        static void MeshObject(string name,Transform parent,Mesh mesh,Material mat)
        {
            var go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(parent,false);
            go.GetComponent<MeshFilter>().sharedMesh=mesh;go.GetComponent<MeshRenderer>().sharedMaterial=mat;go.GetComponent<MeshRenderer>().shadowCastingMode=ShadowCastingMode.Off;
        }
        static Mesh Lathe(Vector2[] profile,int segments)
        {
            var v=new Vector3[profile.Length*(segments+1)];var uv=new Vector2[v.Length];var triangles=new List<int>();
            for(int y=0;y<profile.Length;y++) for(int i=0;i<=segments;i++)
            {
                float a=i*Mathf.PI*2/segments;int k=y*(segments+1)+i;
                v[k]=new Vector3(Mathf.Cos(a)*profile[y].x,profile[y].y,Mathf.Sin(a)*profile[y].x);uv[k]=new Vector2((float)i/segments,(float)y/profile.Length);
                if(y>0&&i>0){int b=k-segments-1;triangles.AddRange(new[]{k,b,k-1,k-1,b,b-1});}
            }
            var mesh=new Mesh{vertices=v,uv=uv,triangles=triangles.ToArray()};mesh.RecalculateNormals();return mesh;
        }
        static void Leaf(Transform root,Vector3 start,Vector3 direction,float length,float width,Material mat)
        {
            var side=Vector3.Cross(direction,Vector3.up).normalized;var v=new Vector3[21];var uv=new Vector2[21];var indices=new List<int>();
            for(int row=0;row<7;row++) for(int col=0;col<3;col++)
            {
                float t=row/6f,s=col-1;
                v[row*3+col]=start+direction*(t*length)+Vector3.up*(Mathf.Sin(t*Mathf.PI)*.11f+s*s*.02f)+side*(s*Mathf.Sin(t*Mathf.PI)*width);
                uv[row*3+col]=new Vector2(col/2f,t);
                if(row>0&&col>0){int k=row*3+col;indices.AddRange(new[]{k,k-3,k-1,k-1,k-3,k-4,k-1,k-3,k,k-4,k-3,k-1});}
            }
            var normal=Vector3.Cross(side,direction).normalized;
            var mesh=new Mesh{vertices=v,uv=uv,triangles=indices.ToArray(),normals=Enumerable.Repeat(normal,v.Length).ToArray()};MeshObject("Sculpted leaf",root,mesh,mat);
        }
        static Material Mat(string name,Color color,float smoothness,Texture2D texture=null,bool unlit=false)
        {
            string path=Base+"Art/Materials/"+name+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);
            var shader=Shader.Find(unlit?"Universal Render Pipeline/Unlit":"Universal Render Pipeline/Lit");
            if(material==null){material=new Material(shader);AssetDatabase.CreateAsset(material,path);}material.shader=shader;material.color=color;
            if(material.HasProperty("_Smoothness"))material.SetFloat("_Smoothness",smoothness);
            if(texture!=null){material.SetTexture("_BaseMap",texture);material.SetTextureScale("_BaseMap",name=="Limestone floor"?new Vector2(6,6):Vector2.one);}
            EditorUtility.SetDirty(material);return material;
        }
        static Texture2D Grain(string name,bool tile)
        {
            string path=Base+"Art/Textures/"+name+".png";
            if(!File.Exists(path))
            {
                var texture=new Texture2D(256,256,TextureFormat.RGB24,false);var random=new System.Random(412);
                for(int y=0;y<256;y++)for(int x=0;x<256;x++)
                {
                    float n=(float)random.NextDouble()*.03f;
                    float value=tile?(x<3||y<3?.7f:.92f+n):.84f+.055f*Mathf.Sin(y*.19f+Mathf.Sin(x*.036f)*2)+n;
                    texture.SetPixel(x,y,new Color(value,value,value));
                }
                texture.Apply();File.WriteAllBytes(path,texture.EncodeToPNG());UnityEngine.Object.DestroyImmediate(texture);AssetDatabase.ImportAsset(path);
                var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.wrapMode=TextureWrapMode.Repeat;importer.mipmapEnabled=true;importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
        static void Combine(Transform root,string name)
        {
            // One mesh per material and environment, rather than a draw call for every keyboard key or leaf.
            var filters=root.GetComponentsInChildren<MeshFilter>(true).Where(x=>x.GetComponent<MeshRenderer>()!=null&&x.GetComponent<TextMeshPro>()==null&&!PrefabUtility.IsPartOfPrefabInstance(x)).ToArray();
            foreach(var group in filters.GroupBy(x=>x.GetComponent<MeshRenderer>().sharedMaterial))
            {
                var parts=group.Select(x=>new CombineInstance{mesh=x.sharedMesh,transform=root.worldToLocalMatrix*x.transform.localToWorldMatrix}).ToArray();
                var mesh=new Mesh{name=name+" "+group.Key.name,indexFormat=IndexFormat.UInt32};mesh.CombineMeshes(parts,true,true);mesh.RecalculateBounds();
                string path=Base+"Art/Meshes/"+name+" "+group.Key.name+".asset";
                var existing=AssetDatabase.LoadAssetAtPath<Mesh>(path);
                if(existing==null)AssetDatabase.CreateAsset(mesh,path);else{EditorUtility.CopySerialized(mesh,existing);UnityEngine.Object.DestroyImmediate(mesh);mesh=existing;EditorUtility.SetDirty(mesh);}
                foreach(var filter in group){UnityEngine.Object.DestroyImmediate(filter.GetComponent<MeshRenderer>());UnityEngine.Object.DestroyImmediate(filter);}
                MeshObject(name+" / "+group.Key.name,root,mesh,group.Key);
            }
        }

        public static void ValidatePolishedRoom()
        {
            foreach(string path in new[]{DevelopmentSceneBuilder.MemoryScene,Main})
            {
                var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Single);
                var room=scene.GetRootGameObjects().Select(x=>x.GetComponent<MemoryRoomController>()).Single(x=>x!=null);
                var components=room.GetComponentsInChildren<Component>(true);
                if(components.Any(x=>x==null))throw new Exception("Missing script in "+path);
                if(room.cards.Length!=12||room.atmosphere==null||room.resetConfirmation==null||room.setupPanel==null)throw new Exception("Incomplete polished room references.");
                if(room.atmosphere.officeSources.Concat(room.atmosphere.natureSources).Any(x=>x==null||x.clip==null))throw new Exception("Missing ambience audio.");
                if(room.GetComponentsInChildren<MeshRenderer>(true).Any(x=>x.sharedMaterials.Any(m=>m==null)))throw new Exception("Missing polished material.");
                Debug.Log("Polished room validation passed: "+path+"; renderers including both conditions="+room.GetComponentsInChildren<Renderer>(true).Length);
            }
            DevelopmentSceneBuilder.ValidateScenes();
        }
    }
}
