using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using FXR.Memory;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEditor.XR.OpenXR;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace FXR.Editor
{
    public static class QuestReadiness
    {
        [MenuItem("FXR/Quest/Validate and build updated APK")]
        public static void BuildQuest()
        {
            PrototypeBuilder.ConfigureProject();
            PlayerSettings.colorSpace=ColorSpace.Linear;
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
            PlayerSettings.bundleVersion="0.3.0";PlayerSettings.Android.bundleVersionCode=3;
            PlayerSettings.runInBackground=false;
            var mobile=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/Settings/Mobile_RPAsset.asset");
            mobile.supportsHDR=false;mobile.renderScale=1f;mobile.msaaSampleCount=4;
            mobile.supportsCameraDepthTexture=false;mobile.supportsCameraOpaqueTexture=false;
            mobile.maxAdditionalLightsCount=2;mobile.shadowDistance=12;
            mobile.mainLightShadowmapResolution=1024;
            var pipelineSettings=new SerializedObject(mobile);
            pipelineSettings.FindProperty("m_AdditionalLightsRenderingMode").intValue=(int)LightRenderingMode.PerPixel;
            pipelineSettings.FindProperty("m_AdditionalLightShadowsSupported").boolValue=false;
            pipelineSettings.FindProperty("m_SoftShadowsSupported").boolValue=false;
            pipelineSettings.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(mobile);
            var issues=new List<OpenXRFeature.ValidationRule>();
            // Meta Quest's validation predicates inspect the selected group rather
            // than the group passed to GetCurrentValidationIssues. Set it explicitly.
            EditorUserBuildSettings.selectedBuildTargetGroup=BuildTargetGroup.Android;
            var openXR=OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android);
            openXR.latencyOptimization=OpenXRSettings.LatencyOptimization.PrioritizeInputPolling;
            EditorUtility.SetDirty(openXR);
            OpenXRProjectValidation.GetCurrentValidationIssues(issues,BuildTargetGroup.Android);
            var report=new StringBuilder("Quest preparation report — 10 October 2026\nUnity 2022.3.62f3 / OpenXR 1.16.1 / XRI 3.3.1\n");
            report.AppendLine("Android ARM64, IL2CPP, Vulkan, linear color, UnityPlayerActivity, landscape-left.");
            report.AppendLine("4x MSAA, 1.0 render scale, no HDR/post-processing/SSAO, maximum 2 additional pixel lights.");
            report.AppendLine("Final build scene: MainScene only. Development scenes are excluded.");
            if(openXR==null||openXR.renderMode!=OpenXRSettings.RenderMode.SinglePassInstanced
                ||!openXR.GetFeatures<OculusTouchControllerProfile>().Any(x=>x.enabled))throw new Exception("Incomplete OpenXR/controller configuration.");
            foreach(var issue in issues)report.AppendLine((issue.error?"ERROR: ":"WARNING: ")+issue.message);
            report.AppendLine("OpenXR validation: "+issues.Count(x=>x.error)+" error(s), "+issues.Count(x=>!x.error)+" warning(s).");
            var scene=EditorSceneManager.OpenScene("Assets/Shared/Scenes/MainScene.unity",OpenSceneMode.Single);
            var room=scene.GetRootGameObjects().Select(x=>x.GetComponent<MemoryRoomController>()).Single(x=>x!=null);
            foreach(var camera in scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<Camera>(true)))
            {
                camera.allowHDR=false;
                var additional=camera.GetComponent<UniversalAdditionalCameraData>();if(additional!=null)additional.renderPostProcessing=false;
            }
            foreach(bool natural in new[]{false,true})
            {
                room.officeProps.SetActive(!natural);room.natureProps.SetActive(natural);
                var renderers=room.GetComponentsInChildren<MeshRenderer>(false);
                long triangles=renderers.Sum(x=>{var mf=x.GetComponent<MeshFilter>();return mf!=null&&mf.sharedMesh!=null?mf.sharedMesh.triangles.LongLength/3:0;});
                report.AppendLine((natural?"Nature":"Office")+" idle room: "+renderers.Length+" active mesh renderers, "+triangles+" triangles (geometry count, not a headset performance result).");
            }
            room.officeProps.SetActive(true);room.natureProps.SetActive(false);
            EditorSceneManager.SaveScene(scene);
            ClearDynamicFontCaches();AssetDatabase.SaveAssets();
            Directory.CreateDirectory("Builds/Quest");
            File.WriteAllText("Builds/Quest/Readiness.txt",report.ToString());Debug.Log(report.ToString());
            if(issues.Any(x=>x.error))throw new Exception("OpenXR validation errors remain. Read Builds/Quest/Readiness.txt.");
            var build=BuildPipeline.BuildPlayer(new BuildPlayerOptions{
                scenes=new[]{"Assets/Shared/Scenes/MainScene.unity"},locationPathName="Builds/Quest/FXRMemory.apk",
                target=BuildTarget.Android,options=BuildOptions.Development});
            if(build.summary.result!=BuildResult.Succeeded)throw new Exception("Quest build failed: "+build.summary.result);
            long apkBytes=new FileInfo("Builds/Quest/FXRMemory.apk").Length;
            File.AppendAllText("Builds/Quest/Readiness.txt","APK build succeeded: "+apkBytes+" bytes (APK file only; compiler symbols excluded).\nHeadset installation and on-device checks still need a connected, authorized Quest.\n");
            Debug.Log("Updated Quest APK build succeeded: "+apkBytes+" bytes.");
        }

        public static void ClearDynamicFontCaches()
        {
            foreach(string guid in AssetDatabase.FindAssets("t:TMP_FontAsset"))
            {
                var font=AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(AssetDatabase.GUIDToAssetPath(guid));
                if(font.atlasPopulationMode==TMPro.AtlasPopulationMode.Dynamic){font.ClearFontAssetData(true);EditorUtility.SetDirty(font);}
            }
        }
    }
}
