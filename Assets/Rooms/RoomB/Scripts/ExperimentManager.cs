
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ExperimentManager : MonoBehaviour
{
    [Header("Exhibits in Visiting Order")]
    public ExhibitInteraction[] exhibits;

    [Header("Memory Quiz Reference")]
    public MemoryQuizManager memoryQuizManager;

    [Header("Experiment Progress")]
    public int currentExhibitIndex = 0;
    public bool experimentStarted = false;
    public bool observationPhaseFinished = false;

    [Header("Exhibit Guidance")]
    public GameObject guideArrow;
    public Transform[] guidePoints;

    [Header("Quiz Station Guidance")]
    public Transform quizStationGuidePoint;

    [Header("Quiz Screen")]
    public TMP_Text quizScreenText;

    [Header("Test Settings")]
    public bool testMode = false;
    public float testObservationDuration = 3f;

    [Header("Distractor Task")]
    public GameObject distractorPanel;
    public PlayerController playerController;

    [Header("Experiment Ending")]
    public GameObject resultsPanel;
    public Transform exitPoint;

    private bool quizStationStarted = false;
    private bool freeExplorationMode = false;
    private bool experimentCompleted = false;

    private void Start()
    {
        if (exhibits == null || exhibits.Length == 0)
        {
            Debug.LogError(
                "ExperimentManager: No exhibits assigned!"
            );
            return;
        }

        AssignBalancedConditions();

        if (guideArrow != null)
        {
            guideArrow.SetActive(false);
        }

        if (distractorPanel != null)
        {
            distractorPanel.SetActive(false);
        }

        UpdateQuizScreen(
            "MEMORY EXPERIMENT\n\n" +
            "Complete the exhibition first"
        );

        Debug.Log("ExperimentManager initialized.");
    }

    private void AssignBalancedConditions()
    {
        if (memoryQuizManager == null ||
            memoryQuizManager.questions == null)
        {
            Debug.LogError(
                "ExperimentManager: MemoryQuizManager is not assigned. " +
                "Cannot balance Photo/Observe conditions."
            );
            return;
        }

        List<int> imageExhibits = new List<int>();
        List<int> detailExhibits = new List<int>();

        HashSet<int> assignedExhibits = new HashSet<int>();

        foreach (MemoryQuizManager.QuizQuestion question
                 in memoryQuizManager.questions)
        {
            int exhibitIndex = question.exhibitNumber - 1;

            if (exhibitIndex < 0 ||
                exhibitIndex >= exhibits.Length ||
                exhibits[exhibitIndex] == null)
            {
                Debug.LogError(
                    "Invalid exhibit number in Memory Quiz: " +
                    question.exhibitNumber
                );
                return;
            }

            if (!assignedExhibits.Add(exhibitIndex))
            {
                Debug.LogError(
                    "Duplicate quiz question for Exhibit " +
                    question.exhibitNumber +
                    ". Each exhibit must have exactly one question."
                );
                return;
            }

            if (question.questionType ==
                MemoryQuizManager.QuestionType.ImageRecognition)
            {
                imageExhibits.Add(exhibitIndex);
            }
            else
            {
                detailExhibits.Add(exhibitIndex);
            }
        }

        if (assignedExhibits.Count != exhibits.Length)
        {
            Debug.LogError(
                "ExperimentManager: Every exhibit must have " +
                "exactly one quiz question. Found " +
                assignedExhibits.Count + " questions for " +
                exhibits.Length + " exhibits."
            );
            return;
        }

        if (imageExhibits.Count % 2 != 0 ||
            detailExhibits.Count % 2 != 0)
        {
            Debug.LogError(
                "ExperimentManager: Both question types " +
                "must have an even number of exhibits."
            );
            return;
        }

        Shuffle(imageExhibits);
        Shuffle(detailExhibits);

        AssignHalfPhotoHalfObserve(imageExhibits);
        AssignHalfPhotoHalfObserve(detailExhibits);

        int photoImage = imageExhibits.Count / 2;
        int observeImage = imageExhibits.Count / 2;

        int photoDetail = detailExhibits.Count / 2;
        int observeDetail = detailExhibits.Count / 2;

        Debug.Log(
            "BALANCED EXPERIMENT CONDITIONS\n" +
            "Photo: " +
            photoImage + " ImageRecognition, " +
            photoDetail + " DetailMemory\n" +
            "Observe: " +
            observeImage + " ImageRecognition, " +
            observeDetail + " DetailMemory"
        );

        for (int i = 0; i < exhibits.Length; i++)
        {
            Debug.Log(
                "Exhibit " + (i + 1) +
                " -> " + exhibits[i].condition
            );
        }
    }

    private void AssignHalfPhotoHalfObserve(
        List<int> exhibitIndices)
    {
        int photoCount = exhibitIndices.Count / 2;

        for (int i = 0; i < exhibitIndices.Count; i++)
        {
            int exhibitIndex = exhibitIndices[i];

            exhibits[exhibitIndex].condition =
                i < photoCount
                    ? ExhibitInteraction.TrialCondition.Photo
                    : ExhibitInteraction.TrialCondition.Observe;
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void StartExperiment()
    {
        experimentStarted = true;
        experimentCompleted = false;
        freeExplorationMode = false;

        currentExhibitIndex = 0;
        observationPhaseFinished = false;
        quizStationStarted = false;

        UpdateGuideArrow();

        UpdateQuizScreen(
            "MEMORY EXPERIMENT\n\n" +
            "Complete the exhibition first"
        );

        Debug.Log("Observation phase started.");
    }

    public float GetObservationDuration(float normalDuration)
    {
        if (testMode)
        {
            return Mathf.Max(
                0.1f,
                testObservationDuration
            );
        }

        return normalDuration;
    }

    public bool CanInteract(ExhibitInteraction exhibit)
    {
        if (!experimentStarted ||
            experimentCompleted ||
            freeExplorationMode ||
            observationPhaseFinished)
        {
            return false;
        }

        if (exhibits == null ||
            currentExhibitIndex >= exhibits.Length)
        {
            return false;
        }

        return exhibits[currentExhibitIndex] == exhibit;
    }

    public void OnExhibitObservationStarted()
    {
        if (guideArrow != null)
        {
            guideArrow.SetActive(false);
        }
    }

    public void OnExhibitCompleted(ExhibitInteraction exhibit)
    {
        if (!CanInteract(exhibit))
        {
            return;
        }

        Debug.Log(
            "Completed exhibit " +
            (currentExhibitIndex + 1) +
            "/" + exhibits.Length +
            " | Condition: " + exhibit.condition
        );

        currentExhibitIndex++;

        if (currentExhibitIndex >= exhibits.Length)
        {
            observationPhaseFinished = true;

            Debug.Log("All exhibits completed!");

            StartDistractorPhase();
        }
        else
        {
            Debug.Log(
                "Next exhibit: " +
                (currentExhibitIndex + 1) +
                "/" + exhibits.Length
            );

            UpdateGuideArrow();
        }
    }

    private void StartDistractorPhase()
    {
        foreach (ExhibitInteraction exhibit in exhibits)
        {
            if (exhibit != null)
            {
                exhibit.gameObject.SetActive(false);
            }
        }

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (distractorPanel != null)
        {
            distractorPanel.SetActive(false);
        }

        if (guideArrow != null)
        {
            if (quizStationGuidePoint != null)
            {
                guideArrow.transform.position =
                    quizStationGuidePoint.position;

                guideArrow.transform.rotation =
                    quizStationGuidePoint.rotation;

                guideArrow.SetActive(true);
            }
            else
            {
                guideArrow.SetActive(false);

                Debug.LogWarning(
                    "QuizStationGuidePoint is not assigned!"
                );
            }
        }

        UpdateQuizScreen(
            "MEMORY EXPERIMENT\n\n" +
            "Observation Complete\n\n" +
            "PRESS E TO CONTINUE"
        );

        Debug.Log(
            "Observation complete. " +
            "Approach QuizStation to continue."
        );
    }

    public void OnQuizStationStarted()
    {
        quizStationStarted = true;

        if (guideArrow != null)
        {
            guideArrow.SetActive(false);
        }

        UpdateQuizScreen(
            "DISTRACTOR TASK\n\n" +
            "Solve the calculations"
        );

        Debug.Log("QuizStation test started.");
    }

    // Called by the Confirm & Exit button
    public void ConfirmAndExit()
    {
        EndExperiment();

        if (exitPoint != null && playerController != null)
        {
            playerController.TeleportTo(exitPoint);
            Debug.Log("Player moved outside the exhibition.");
        }
        else
        {
            Debug.LogWarning(
                "Exit Point or Player Controller is missing!"
            );
        }
    }

    // Called by the Explore Again button
    public void ExploreAgain()
    {
        EndExperiment();

        freeExplorationMode = true;

        UpdateQuizScreen(
            "NOW YOU CAN EXPLORE FREELY\n\n" +
            "Feel free to look around and leave whenever you're ready."
        );

        Debug.Log(
            "Free exploration mode started. " +
            "Participants can explore without experiment tasks."
        );
    }

    private void EndExperiment()
    {
        experimentCompleted = true;
        experimentStarted = false;

        if (resultsPanel != null)
        {
            resultsPanel.SetActive(false);
        }

        if (distractorPanel != null)
        {
            distractorPanel.SetActive(false);
        }

        if (guideArrow != null)
        {
            guideArrow.SetActive(false);
        }

        // Restore exhibit objects hidden after observation.
        foreach (ExhibitInteraction exhibit in exhibits)
        {
            if (exhibit != null)
            {
                exhibit.gameObject.SetActive(true);
            }
        }

        // Show a welcome message for free exploration.
        UpdateQuizScreen(
            "NOW YOU CAN EXPLORE FREELY\n\n" +
            "Feel free to look around and leave whenever you're ready."
        );

        if (playerController != null)
        {
            playerController.EndObservation();
            playerController.enabled = true;
            playerController.StartExperiment();
        }

        Debug.Log("Experiment ended.");
    }

    private void UpdateQuizScreen(string message)
    {
        if (quizScreenText != null)
        {
            quizScreenText.text = message;
        }
    }

    private void UpdateGuideArrow()
    {
        if (guideArrow == null)
        {
            return;
        }

        if (!experimentStarted ||
            observationPhaseFinished ||
            guidePoints == null ||
            currentExhibitIndex >= guidePoints.Length)
        {
            guideArrow.SetActive(false);
            return;
        }

        Transform target = guidePoints[currentExhibitIndex];

        if (target == null)
        {
            guideArrow.SetActive(false);
            return;
        }

        guideArrow.transform.position = target.position;
        guideArrow.transform.rotation = target.rotation;

        guideArrow.SetActive(true);
    }

    private void OnGUI()
    {
        if (!experimentStarted ||
            experimentCompleted ||
            freeExplorationMode)
        {
            return;
        }

        if (observationPhaseFinished)
        {
            if (quizStationStarted)
            {
                return;
            }

            GUIStyle messageStyle =
                new GUIStyle(GUI.skin.box);

            messageStyle.fontSize = 22;
            messageStyle.alignment =
                TextAnchor.MiddleCenter;

            messageStyle.wordWrap = true;
            messageStyle.normal.textColor =
                Color.white;

            GUI.Box(
                new Rect(
                    (Screen.width - 600f) / 2f,
                    30f,
                    600f,
                    90f
                ),
                "Observation complete!\n" +
                "Please proceed to the Quiz Station.",
                messageStyle
            );

            return;
        }

        string progress =
            "Exhibit " +
            (currentExhibitIndex + 1) +
            " / " + exhibits.Length;

        GUIStyle style =
            new GUIStyle(GUI.skin.label);

        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        GUI.Label(
            new Rect(20, 20, 300, 40),
            progress,
            style
        );
    }
}
