using UnityEngine;
using UnityEngine.XR;

namespace FXR.Shared
{
    public sealed class PlayerModeBootstrap : MonoBehaviour
    {
        public GameObject xrRig, desktopRig, simulator;
        [Tooltip("Editor only. Turn on to use Unity's keyboard/mouse XR simulator.")]
        public bool simulateXR;

        void Awake()
        {
            bool simulate = false;
#if UNITY_EDITOR
            simulate = simulateXR;
#endif
            bool useXR = simulate || Application.platform == RuntimePlatform.Android || XRSettings.isDeviceActive;
            xrRig.SetActive(useXR);
            desktopRig.SetActive(!useXR);
            if (simulator != null) simulator.SetActive(simulate);
        }
    }
}
