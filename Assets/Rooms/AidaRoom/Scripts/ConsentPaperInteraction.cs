
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConsentPaperInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject consentPanel;

    [Header("Consent Buttons")]
    [SerializeField] private Button consentButton;
    [SerializeField] private Button declineButton;

    [Header("Settings")]
    [SerializeField] private float closeDelay = 10f;

    private bool hasAnswered;
    private bool hasConsent;
    private Coroutine closeCoroutine;

    public bool HasConsent => hasConsent;
    public bool Answered => hasAnswered;
    public bool PanelOpen => consentPanel.activeSelf;
    public void ConfigureCamera(Camera camera) { playerCamera = camera; }

    private void Update()
    {
        if (hasAnswered || consentPanel.activeSelf)
            return;

        if (DeskPointer.PressedOn(this, playerCamera))
            consentPanel.SetActive(true);
    }

    public void AcceptConsent()
    {
        RecordAnswer(true);
    }

    public void DeclineConsent()
    {
        RecordAnswer(false);
    }

    private void RecordAnswer(bool accepted)
    {
        if (hasAnswered)
            return;

        hasAnswered = true;
        hasConsent = accepted;

        Button selectedButton = accepted
            ? consentButton
            : declineButton;

        Image buttonImage = selectedButton.GetComponent<Image>();

        buttonImage.color = accepted
            ? new Color(0.25f, 0.7f, 0.35f)
            : new Color(0.85f, 0.3f, 0.3f);

        consentButton.interactable = false;
        declineButton.interactable = false;

        Debug.Log("Consent recorded: " +
                  (accepted ? "Accepted" : "Declined"));

        closeCoroutine = StartCoroutine(CloseConsentAfterDelay());
    }

    private IEnumerator CloseConsentAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);

        consentPanel.SetActive(false);
        closeCoroutine = null;
    }

    public void ResetConsent()
    {
        if (closeCoroutine != null)
        {
            StopCoroutine(closeCoroutine);
            closeCoroutine = null;
        }

        hasAnswered = false;
        hasConsent = false;

        consentButton.interactable = true;
        declineButton.interactable = true;

        consentButton.GetComponent<Image>().color = Color.white;
        declineButton.GetComponent<Image>().color = Color.white;

        consentPanel.SetActive(false);

        Debug.Log("Consent reset.");
    }
}
