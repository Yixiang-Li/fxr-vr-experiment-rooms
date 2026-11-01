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
    }
}
