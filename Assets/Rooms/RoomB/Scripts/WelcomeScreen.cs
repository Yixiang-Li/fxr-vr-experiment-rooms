
using UnityEngine;

public class WelcomeScreen : MonoBehaviour
{
    [Header("UI")]
    public GameObject welcomePanel;

    [Header("Player")]
    public PlayerController playerController;

    [Header("Room Entry")]
    public Transform roomSpawnPoint;

    [Header("Experiment")]
    public ExperimentManager experimentManager;
    public void StartExperiment()
    {
        if (welcomePanel == null ||
            playerController == null ||
            roomSpawnPoint == null)
        {
            Debug.LogWarning(
                "WelcomeScreen: Some references are missing!"
            );
            return;
        }

        // Move player into Room2
        playerController.TeleportTo(roomSpawnPoint);

        // Hide welcome instructions
        welcomePanel.SetActive(false);

        // Enable player movement and mouse look
        playerController.StartExperiment();

        if (experimentManager != null)
        {
            experimentManager.StartExperiment();
        }

        Debug.Log("Experiment started! Player entered Room2.");
    }
}
