
using System.Collections;
using UnityEngine;

public class ResetButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StroopExperiment experiment;
    [SerializeField] private PhysicalButton startButton;
    [SerializeField] private PhysicalButton redButton;
    [SerializeField] private PhysicalButton greenButton;
    [SerializeField] private PhysicalButton blueButton;
    [SerializeField] private StartHologramController hologramController;
    [SerializeField] private RoomAudioManager roomAudio;

    [Header("Animation")]
    [SerializeField] private float pressDepth = 0.015f;
    [SerializeField] private float pressDuration = 0.20f;

    private Vector3 originalPosition;
    private bool isAnimating;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    private void Update() { if (DeskPointer.PressedOn(this)) Press(); }

    public void Press()
    {
        if (isAnimating)
            return;

        if (roomAudio != null) roomAudio.ResetExperimentAudio();

        // Play RESET sound
        if (roomAudio != null)
            roomAudio.PlayReset();

        if (experiment != null)
            experiment.ResetExperiment();

        if (startButton != null)
            startButton.ResetButton();

        if (redButton != null)
            redButton.ResetButton();

        if (greenButton != null)
            greenButton.ResetButton();

        if (blueButton != null)
            blueButton.ResetButton();

        if (hologramController != null)
            hologramController.OnResetPressed();

        StartCoroutine(AnimatePress());

        Debug.Log("Experiment reset!");
    }

    private IEnumerator AnimatePress()
    {
        isAnimating = true;

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
        isAnimating = false;
    }
}
