using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace FXR.Editor
{
    // The downloaded source models are nested FBX prefabs. Source GUIDs stay stable.
    public static class MemorySourceAssets
    {
        const string Root="Assets/Rooms/Memory/ThirdParty/PolyHaven/";
        [Serializable] public class Part { public string name,baseColor;public bool alphaClip; }
        [Serializable] public class Source { public string id;public Part[] materials; }

        public static void Prepare()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach(string clip in new[]{"Office","OfficeDetail","Nature","NatureDetail"})
            {
                var audio=AssetImporter.GetAtPath("Assets/Rooms/Memory/Audio/"+clip+".wav") as AudioImporter;
                var settings=audio.defaultSampleSettings;settings.loadType=AudioClipLoadType.Streaming;
                settings.compressionFormat=AudioCompressionFormat.Vorbis;settings.quality=.8f;
                settings.sampleRateSetting=AudioSampleRateSetting.PreserveSampleRate;
                audio.defaultSampleSettings=settings;audio.forceToMono=true;audio.SaveAndReimport();
            }
            foreach(var file in Directory.GetFiles(Root,"source.json",SearchOption.AllDirectories))
            {
                string folder=Path.GetDirectoryName(file).Replace('\\','/');
                var source=JsonUtility.FromJson<Source>(File.ReadAllText(file));
                if(source.materials==null)continue;
                foreach(var part in source.materials)PrepareMaterial(folder,part);
                string modelPath=folder+"/"+source.id+".fbx";
                var importer=AssetImporter.GetAtPath(modelPath) as ModelImporter;
                if(importer==null)continue;
                importer.isReadable=true;importer.importAnimation=false;
                importer.meshCompression=ModelImporterMeshCompression.Low;
                importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;
                importer.SaveAndReimport();
                var materials=AssetDatabase.LoadAllAssetsAtPath(modelPath).OfType<Material>().ToArray();
                var model=AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
                var names=model.GetComponentsInChildren<MeshRenderer>(true).SelectMany(x=>x.sharedMaterials)
                    .Where(x=>x!=null).Select(x=>x.name).Concat(materials.Select(x=>x.name))
                    .Concat(importer.GetExternalObjectMap().Keys.Where(x=>x.type==typeof(Material)).Select(x=>x.name)).Distinct().ToArray();
                foreach(string name in names)
                {
                    string lower=name.ToLowerInvariant();
                    var part=source.materials.FirstOrDefault(p=>lower.Contains(p.name.ToLowerInvariant())
                        || p.name=="leaves"&&lower.Contains("leaf")) ?? source.materials[0];
                    var material=AssetDatabase.LoadAssetAtPath<Material>(folder+"/"+part.name+".mat");
                    if(lower.EndsWith("_glass"))material=PrepareGlass(folder);
                    importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material),name),material);
                }
                importer.SaveAndReimport();
                model=AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
                long triangles=model.GetComponentsInChildren<MeshFilter>(true).Sum(x=>x.sharedMesh.triangles.LongLength/3);
                Debug.Log("CC0 model prepared: "+source.id+" | "+triangles+" triangles | materials: "+string.Join(", ",names));
                PrepareOptimizedModel(model,folder,source.id);
            }
            AssetDatabase.SaveAssets();
        }

        static Material PrepareGlass(string folder)
        {
            string path=folder+"/glass.mat";
            var material=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(material==null){material=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(material,path);}
            material.SetColor("_BaseColor",new Color(.9f,.96f,1,.06f));
            material.SetFloat("_Surface",1);material.SetFloat("_Blend",0);
            material.SetFloat("_SrcBlend",(int)BlendMode.SrcAlpha);material.SetFloat("_DstBlend",(int)BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_ZWrite",0);material.SetFloat("_Smoothness",.95f);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");material.renderQueue=3000;
            material.SetShaderPassEnabled("ShadowCaster",false);EditorUtility.SetDirty(material);return material;
        }

        static void PrepareOptimizedModel(GameObject source,string folder,string id)
        {
            // Preserve every material/submesh and UV while combining scattered leaves.
            var groups=new Dictionary<Material,List<CombineInstance>>();
            foreach(var filter in source.GetComponentsInChildren<MeshFilter>(true))
            {
                var renderer=filter.GetComponent<MeshRenderer>();if(renderer==null)continue;
                for(int sub=0;sub<filter.sharedMesh.subMeshCount;sub++)
                {
                    var mat=renderer.sharedMaterials[Mathf.Min(sub,renderer.sharedMaterials.Length-1)];
                    if(!groups.ContainsKey(mat))groups[mat]=new List<CombineInstance>();
                    groups[mat].Add(new CombineInstance{mesh=filter.sharedMesh,subMeshIndex=sub,
                        transform=source.transform.worldToLocalMatrix*filter.transform.localToWorldMatrix});
                }
            }
            var root=new GameObject(id+" — grouped for Quest");
            try
            {
                foreach(var pair in groups)
                {
                    var mesh=new Mesh{name=id+" "+pair.Key.name,indexFormat=IndexFormat.UInt32};
                    mesh.CombineMeshes(pair.Value.ToArray(),true,true);mesh.RecalculateBounds();
                    string path=folder+"/"+pair.Key.name+"_Grouped.asset";
                    var saved=AssetDatabase.LoadAssetAtPath<Mesh>(path);
                    if(saved==null)AssetDatabase.CreateAsset(mesh,path);
                    else{EditorUtility.CopySerialized(mesh,saved);UnityEngine.Object.DestroyImmediate(mesh);mesh=saved;EditorUtility.SetDirty(mesh);}
                    var part=new GameObject(pair.Key.name,typeof(MeshFilter),typeof(MeshRenderer));
                    part.transform.SetParent(root.transform,false);part.GetComponent<MeshFilter>().sharedMesh=mesh;
                    part.GetComponent<MeshRenderer>().sharedMaterial=pair.Key;
                }
                PrefabUtility.SaveAsPrefabAsset(root,folder+"/"+id+"_Quest.prefab");
                Debug.Log("Grouped source model: "+id+" → "+groups.Count+" mesh renderers; geometry and materials retained.");
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }

        static void Texture(string path,bool linear=false,bool normal=false,bool alpha=false)
        {
            var importer=AssetImporter.GetAtPath(path) as TextureImporter;
            if(importer==null)return;
            importer.textureType=normal?TextureImporterType.NormalMap:TextureImporterType.Default;
            importer.sRGBTexture=!linear&&!normal;importer.alphaIsTransparency=alpha;
            importer.mipmapEnabled=true;importer.maxTextureSize=1024;importer.anisoLevel=4;
            importer.filterMode=FilterMode.Trilinear;
            var android=importer.GetPlatformTextureSettings("Android");android.overridden=true;
            android.maxTextureSize=1024;android.format=TextureImporterFormat.ASTC_6x6;
            importer.SetPlatformTextureSettings(android);importer.SaveAndReimport();
        }

        static void PrepareMaterial(string folder,Part part)
        {
            string stem=folder+"/"+part.name;
            Texture(folder+"/"+part.baseColor,alpha:part.alphaClip);
            Texture(stem+"_Normal.jpg",normal:true);
            Texture(stem+"_MetallicSmoothness.png",linear:true);
            Texture(stem+"_Occlusion.jpg",linear:true);
            var material=AssetDatabase.LoadAssetAtPath<Material>(stem+".mat");
            if(material==null){material=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(material,stem+".mat");}
            material.SetColor("_BaseColor",Color.white);
            material.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(folder+"/"+part.baseColor));
            material.SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>(stem+"_Normal.jpg"));
            material.SetTexture("_MetallicGlossMap",AssetDatabase.LoadAssetAtPath<Texture2D>(stem+"_MetallicSmoothness.png"));
            material.SetTexture("_OcclusionMap",AssetDatabase.LoadAssetAtPath<Texture2D>(stem+"_Occlusion.jpg"));
            material.SetFloat("_BumpScale",.8f);material.SetFloat("_Smoothness",.85f);
            material.SetFloat("_Metallic",1);material.SetFloat("_OcclusionStrength",.7f);
            material.EnableKeyword("_NORMALMAP");material.EnableKeyword("_METALLICSPECGLOSSMAP");
            if(material.GetTexture("_OcclusionMap")!=null)material.EnableKeyword("_OCCLUSIONMAP");
            material.SetFloat("_AlphaClip",part.alphaClip?1:0);material.SetFloat("_Cutoff",.45f);
            material.SetFloat("_Cull",part.alphaClip?(int)CullMode.Off:(int)CullMode.Back);
            if(part.alphaClip)material.EnableKeyword("_ALPHATEST_ON");else material.DisableKeyword("_ALPHATEST_ON");
            material.renderQueue=part.alphaClip?2450:2000;material.enableInstancing=true;
            EditorUtility.SetDirty(material);
        }

        public static Material Surface(string id,Vector2 tiling)
        {
            var material=AssetDatabase.LoadAssetAtPath<Material>(Root+id+"/main.mat");
            if(material==null)throw new Exception("Missing prepared CC0 surface: "+id);
            material.SetTextureScale("_BaseMap",tiling);EditorUtility.SetDirty(material);return material;
        }

        public static Texture2D Window(bool nature)
        {
            string path=nature?"forest_slope/GardenWindow.jpg":"potsdamer_platz/OfficeWindow.jpg";
            var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+path);
            var importer=(TextureImporter)AssetImporter.GetAtPath(Root+path);
            importer.maxTextureSize=2048;importer.mipmapEnabled=true;importer.wrapMode=TextureWrapMode.Clamp;importer.SaveAndReimport();
            return texture;
        }

        public static Transform Place(string id,Transform parent,Vector3 location,float height,float yaw=0,bool collision=false)
        {
            var wrapper=new GameObject("CC0 / "+id).transform;wrapper.SetParent(parent,false);wrapper.localPosition=location;
            var model=AssetDatabase.LoadAssetAtPath<GameObject>(Root+id+"/"+id+"_Quest.prefab");
            if(model==null)throw new Exception("Missing source model: "+id);
            var instance=(GameObject)PrefabUtility.InstantiatePrefab(model,wrapper);
            instance.transform.localPosition=Vector3.zero;instance.transform.localRotation=Quaternion.Euler(0,yaw,0);
            var renderers=instance.GetComponentsInChildren<MeshRenderer>(true);
            var bounds=LocalBounds(wrapper,renderers);
            instance.transform.localScale*=height/Mathf.Max(.001f,bounds.size.y);
            bounds=LocalBounds(wrapper,renderers);
            instance.transform.localPosition-=new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
            if(collision)
            {
                bounds=LocalBounds(wrapper,renderers);var box=wrapper.gameObject.AddComponent<BoxCollider>();box.center=bounds.center;box.size=bounds.size;
            }
            return wrapper;
        }

        static Bounds LocalBounds(Transform relative,MeshRenderer[] renderers)
        {
            Bounds bounds=new Bounds();bool first=true;
            foreach(var renderer in renderers)
            {
                var box=renderer.bounds;
                for(int i=0;i<8;i++)
                {
                    var point=relative.InverseTransformPoint(box.center+Vector3.Scale(box.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1)));
                    if(first){bounds=new Bounds(point,Vector3.zero);first=false;}else bounds.Encapsulate(point);
                }
            }
            return bounds;
        }
    }
}
