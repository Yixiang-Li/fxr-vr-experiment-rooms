
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

    [Header("Paper Physics")]
    [SerializeField] private Rigidbody paperRigidbody;
    [SerializeField] private float liftHeight = 0.15f;

    private bool hasAnswered;
    private bool hasConsent;
    private Coroutine closeCoroutine;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public bool HasConsent => hasConsent;

    private void Awake()
    {
        if (paperRigidbody == null)
            paperRigidbody = GetComponent<Rigidbody>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (paperRigidbody != null)
        {
            paperRigidbody.isKinematic = true;
            paperRigidbody.useGravity = true;
        }
    }

    private void Update()
    {
        if (hasAnswered || consentPanel.activeSelf)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        if (playerCamera == null)
            return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                PickUpPaper();
                consentPanel.SetActive(true);
            }
        }
    }

    private void PickUpPaper()
    {
        if (paperRigidbody == null)
            return;

        paperRigidbody.isKinematic = true;
        paperRigidbody.velocity = Vector3.zero;
        paperRigidbody.angularVelocity = Vector3.zero;

        transform.position = initialPosition +
                             Vector3.up * liftHeight;
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

        closeCoroutine =
            StartCoroutine(CloseConsentAfterDelay());
    }

    private IEnumerator CloseConsentAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);

        consentPanel.SetActive(false);
        ReleasePaper();

        closeCoroutine = null;
    }

    private void ReleasePaper()
    {
        if (paperRigidbody == null)
            return;

        paperRigidbody.isKinematic = false;
        paperRigidbody.useGravity = true;

        Debug.Log("Paper released: physics enabled.");
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

        if (paperRigidbody != null)
        {
            paperRigidbody.isKinematic = true;
            paperRigidbody.velocity = Vector3.zero;
            paperRigidbody.angularVelocity = Vector3.zero;

            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        Debug.Log("Consent reset.");
    }
}
