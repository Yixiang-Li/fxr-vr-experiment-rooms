
using UnityEngine;

public class QuizStationInteraction : MonoBehaviour
{
    [Header("References")]
    public ExperimentManager experimentManager;
    public PlayerController playerController;
    public Transform player;
    public GameObject distractorPanel;

    [Header("Interaction Settings")]
    public float interactionDistance = 2.5f;

    private bool isInteracting = false;

    private void Update()
    {
        if (experimentManager == null || player == null)
        {
            return;
        }

        // QuizStation is available only after observation.
        if (!experimentManager.observationPhaseFinished)
        {
            return;
        }

        if (isInteracting)
        {
            return;
        }

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance <= interactionDistance &&
            Input.GetKeyDown(KeyCode.E))
        {
            StartInteraction();
        }
    }

    private void StartInteraction()
    {
        isInteracting = true;

        // Notify ExperimentManager.
        if (experimentManager != null)
        {
            experimentManager.OnQuizStationStarted();
        }

        // Disable movement and camera control.
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Unlock the mouse for UI interaction.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Show the distractor task interface.
        if (distractorPanel != null)
        {
            distractorPanel.SetActive(true);
        }

        Debug.Log("QuizStation interaction started.");
    }

    private void OnGUI()
    {
        if (experimentManager == null ||
            player == null ||
            !experimentManager.observationPhaseFinished ||
            isInteracting)
        {
            return;
        }

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance > interactionDistance)
        {
            return;
        }

        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 22;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;

        float width = 340f;
        float height = 55f;

        GUI.Box(
            new Rect(
                (Screen.width - width) / 2f,
                Screen.height - height - 40f,
                width,
                height
            ),
            "Press E to start the test",
            style
        );
    }
}
