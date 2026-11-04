using UnityEngine;
using UnityEngine.UI;

namespace FXR.Integration
{
    // External player references are assigned before activating an imported room.
    // The owners' complete source scenes remain independently runnable.
    public sealed class IntegratedRoom : MonoBehaviour
    {
        public void Bind(Transform player, Camera camera, PlayerController movement)
        {
            foreach(var c in GetComponentsInChildren<ExhibitInteraction>(true))
            { c.player=player; c.playerCamera=camera; c.playerController=movement; }
            foreach(var c in GetComponentsInChildren<ExperimentManager>(true))
            { c.playerController=movement; c.testMode=false; }
            foreach(var c in GetComponentsInChildren<QuizStationInteraction>(true))
            { c.player=player; c.playerController=movement; }
            foreach(var c in GetComponentsInChildren<WelcomeScreen>(true)) c.playerController=movement;
            foreach(var c in GetComponentsInChildren<ConsentPaperInteraction>(true)) c.ConfigureCamera(camera);
            var explorer=player.GetComponent<FXR.Shared.DesktopExplorer>();
            var cameraOffset=camera.transform.localPosition;
            foreach(var c in GetComponentsInChildren<PhysicalButton>(true))
                c.ConfigurePlayer(player,position=>FocusStroopTask(explorer,position,cameraOffset));
            foreach(var c in GetComponentsInChildren<StroopExperiment>(true)) c.ConfigureSharedMovement(player.GetComponent<FXR.Shared.DesktopExplorer>());
            foreach(var c in GetComponentsInChildren<RoomAudioManager>(true)) c.ConfigureCamera(camera);
            foreach(var c in GetComponentsInChildren<DistractorTask>(true)) c.testMode=false;
            foreach(var c in GetComponentsInChildren<Canvas>(true))
                if(c.worldCamera!=null && c.worldCamera.targetTexture!=null)
                {
                    c.worldCamera.eventMask=0;
                    // Monitor pixels are display-only. Their off-screen UI must
                    // not consume clicks intended for the paper or desk controls.
                    foreach(var raycaster in c.GetComponents<GraphicRaycaster>())raycaster.enabled=false;
                }
                else
                {
                    if(c.renderMode==RenderMode.ScreenSpaceOverlay){c.renderMode=RenderMode.ScreenSpaceCamera;c.planeDistance=.4f;}
                    c.worldCamera=camera;
                }
        }
        public void FocusStroopEntry(FXR.Shared.DesktopExplorer explorer)
        {
            var paper=GetComponentInChildren<ConsentPaperInteraction>();
            Vector3 focus=paper.GetComponent<Collider>().bounds.center;
            foreach(var renderer in GetComponentsInChildren<Renderer>())
                foreach(var material in renderer.sharedMaterials)
                    if(material!=null)
                    {
                        var texture=material.HasProperty("_BaseMap")?material.GetTexture("_BaseMap"):
                            material.HasProperty("_MainTex")?material.GetTexture("_MainTex"):null;
                        if(texture is RenderTexture)focus=Vector3.Lerp(focus,renderer.bounds.center,.4f);
                    }
            Vector3 position=explorer.transform.position;
            var offset=explorer.view.transform.localPosition;
            var look=Quaternion.Inverse(explorer.transform.rotation)*Quaternion.LookRotation(focus-explorer.view.transform.position,Vector3.up);
            explorer.ConfigureView(position,explorer.transform.rotation,offset,look,explorer.view.fieldOfView);
        }
        void FocusStroopTask(FXR.Shared.DesktopExplorer explorer,Transform position,Vector3 cameraOffset)
        {
            Vector3 buttons=Vector3.zero;int count=0;
            foreach(var button in GetComponentsInChildren<PhysicalButton>())
                if(button.name.Trim()!="StartButton") {buttons+=button.GetComponent<Collider>().bounds.center;count++;}
            buttons/=Mathf.Max(1,count);
            Vector3 focus=buttons+Vector3.up*.4f;
            foreach(var renderer in GetComponentsInChildren<Renderer>())
                foreach(var material in renderer.sharedMaterials)
                    if(material!=null)
                    {
                        var texture=material.HasProperty("_BaseMap")?material.GetTexture("_BaseMap"):
                            material.HasProperty("_MainTex")?material.GetTexture("_MainTex"):null;
                        if(texture is RenderTexture)focus=Vector3.Lerp(buttons,renderer.bounds.center,.65f);
                    }
            Vector3 eye=position.position+position.rotation*cameraOffset;
            var localLook=Quaternion.Inverse(position.rotation)*Quaternion.LookRotation(focus-eye,Vector3.up);
            // Keep DesktopExplorer's stored pitch consistent when movement is
            // restored, so the results and response buttons stay in view.
            explorer.ConfigureView(position.position,position.rotation,cameraOffset,Quaternion.Euler(localLook.eulerAngles.x,0,0),explorer.view.fieldOfView);
        }
    }
}
