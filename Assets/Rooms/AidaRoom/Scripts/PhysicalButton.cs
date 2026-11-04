
using System.Collections;
using UnityEngine;

public class PhysicalButton : MonoBehaviour
{
    public enum ButtonType { Start, Red, Green, Blue }

    [Header("References")]
    [SerializeField] private ButtonType buttonType;
    [SerializeField] private StroopExperiment experiment;
    [SerializeField] private ConsentPaperInteraction consent;
    [SerializeField] private StartHologramController hologramController;
    [SerializeField] private RoomAudioManager roomAudio;

    [Header("Player Positioning")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform stroopPlayerPosition;

    [Header("Animation")]
    [SerializeField] private float pressDepth = 0.015f;
    [SerializeField] private float pressDuration = 0.20f;

    [Header("Visuals")]
    [SerializeField] private Color glowColor = Color.cyan;
    [SerializeField] private float glowIntensity = 0.5f;

    private Vector3 originalPosition;
    private Material buttonMaterial;
    private Color originalColor;
    private bool isAnimating;
    private bool startActivated;

    private void Awake()
    {
        originalPosition = transform.localPosition;

        Renderer buttonRenderer = GetComponent<Renderer>();

        if (buttonRenderer != null)
        {
            buttonMaterial = buttonRenderer.material;
            originalColor = buttonMaterial.color;
            SetGlow(false);
        }
    }

    private void OnMouseDown()
    {
        if (isAnimating || experiment == null)
            return;

        if (buttonType == ButtonType.Start)
        {
            if (startActivated || consent == null || !consent.HasConsent)
                return;

            startActivated = true;

            // Play START sound
            if (roomAudio != null)
                roomAudio.PlayStart();

            if (hologramController != null)
                hologramController.OnStartPressed();

            StartCoroutine(AnimatePress(true));
            PositionPlayer();
            experiment.StartExperiment();
        }
        else
        {
            if (!experiment.IsRunning)
                return;

            StartCoroutine(AnimatePress(false));

            switch (buttonType)
            {
                case ButtonType.Red:
                    experiment.SubmitRed();
                    break;

                case ButtonType.Green:
                    experiment.SubmitGreen();
                    break;

                case ButtonType.Blue:
                    experiment.SubmitBlue();
                    break;
            }
        }
    }

    private void PositionPlayer()
    {
        if (player == null || stroopPlayerPosition == null)
            return;

        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.position = stroopPlayerPosition.position;
        player.rotation = stroopPlayerPosition.rotation;

        if (controller != null)
            controller.enabled = true;
    }

    private IEnumerator AnimatePress(bool stayPressed)
    {
        isAnimating = true;
        SetGlow(true);

        Vector3 pressedPosition =
            originalPosition + Vector3.down * pressDepth;

        float half = Mathf.Max(0.01f, pressDuration / 2f);
        float elapsed = 0f;

        while (elapsed < half)
        {
            elapsed += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(
                originalPosition,
                pressedPosition,
                Mathf.Clamp01(elapsed / half)
            );

            yield return null;
        }

        transform.localPosition = pressedPosition;

        if (!stayPressed)
        {
            elapsed = 0f;

            while (elapsed < half)
            {
                elapsed += Time.deltaTime;

                transform.localPosition = Vector3.Lerp(
                    pressedPosition,
                    originalPosition,
                    Mathf.Clamp01(elapsed / half)
                );

                yield return null;
            }

            transform.localPosition = originalPosition;
            SetGlow(false);
            isAnimating = false;
        }
    }

    private void SetGlow(bool active)
    {
        if (buttonMaterial == null)
            return;

        if (active)
        {
            buttonMaterial.color =
                Color.Lerp(originalColor, glowColor, 0.35f);

            if (buttonMaterial.HasProperty("_EmissionColor"))
            {
                buttonMaterial.EnableKeyword("_EMISSION");
                buttonMaterial.SetColor(
                    "_EmissionColor",
                    glowColor * glowIntensity
                );
            }
        }
        else
        {
            buttonMaterial.color = originalColor;

            if (buttonMaterial.HasProperty("_EmissionColor"))
            {
                buttonMaterial.SetColor("_EmissionColor", Color.black);
                buttonMaterial.DisableKeyword("_EMISSION");
            }
        }
    }

    public void ReleaseStartVisual()
    {
        if (buttonType != ButtonType.Start)
            return;

        StopAllCoroutines();
        transform.localPosition = originalPosition;
        SetGlow(false);
        isAnimating = false;
    }

    public void ResetButton()
    {
        StopAllCoroutines();

        transform.localPosition = originalPosition;
        SetGlow(false);

        isAnimating = false;
        startActivated = false;
    }
}
