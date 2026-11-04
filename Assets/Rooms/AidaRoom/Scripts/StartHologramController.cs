
using UnityEngine;

public class StartHologramController : MonoBehaviour
{
    [SerializeField] private GameObject hologram;
    [SerializeField] private ConsentPaperInteraction consent;

    private bool started;

    private void Update()
    {
        if (hologram == null || consent == null)
            return;

        bool shouldShow = consent.HasConsent && !started;

        if (hologram.activeSelf != shouldShow)
            hologram.SetActive(shouldShow);
    }

    public void OnStartPressed()
    {
        started = true;

        if (hologram != null)
            hologram.SetActive(false);
    }

    public void OnResetPressed()
    {
        started = false;
    }
}
