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
        bool useXR;

        void Awake()
        {
            bool simulate = false;
#if UNITY_EDITOR
            simulate = simulateXR;
#endif
            useXR = simulate || Application.platform == RuntimePlatform.Android || XRSettings.isDeviceActive;
            xrRig.SetActive(useXR);
            desktopRig.SetActive(!useXR);
            if (simulator != null) simulator.SetActive(simulate);
        }

        void Start()
        {
            var camera = Camera.main;
            foreach (var canvas in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                canvas.worldCamera = camera;
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
