using UnityEngine;
using UnityEngine.XR;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace FXR.Shared
{
    public sealed class PlayerModeBootstrap : MonoBehaviour
    {
        public GameObject xrRig, desktopRig, simulator;
        [Tooltip("Editor only. Turn on to use Unity's keyboard/mouse XR simulator.")]
        public bool simulateXR;
        [Tooltip("Integrated keyboard/mouse delivery. Owner development scenes may retain optional XR.")]
        public bool desktopOnly;
        bool useXR;

        void Awake()
        {
            bool simulate = false;
#if UNITY_EDITOR
            simulate = simulateXR;
#endif
            useXR = !desktopOnly && (simulate || Application.platform == RuntimePlatform.Android || XRSettings.isDeviceActive);
            xrRig.SetActive(useXR);
            desktopRig.SetActive(!useXR);
            if (simulator != null) simulator.SetActive(simulate);
        }

        void Start()
        {
            var camera = Camera.main;
            foreach (var canvas in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if(canvas.worldCamera == null || canvas.worldCamera.targetTexture == null) canvas.worldCamera = camera;
            if (useXR) return;
            // Standard mouse actions retain both press/release events, including a quick click.
            // The controller simulator and Android retain the existing XR UI input module.
            var events = FindFirstObjectByType<EventSystem>();
            if (events == null) return;
            foreach (var module in events.GetComponents<BaseInputModule>()) module.enabled = false;
            var desktopUI = events.GetComponent<InputSystemUIInputModule>();
            if (desktopUI == null) desktopUI = events.gameObject.AddComponent<InputSystemUIInputModule>();
            // Configure while disabled so every action callback is hooked in one OnEnable pass.
            desktopUI.enabled = false;
            desktopUI.AssignDefaultActions();
            desktopUI.enabled = true;
        }
    }
}
