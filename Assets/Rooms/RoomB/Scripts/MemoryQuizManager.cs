
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemoryQuizManager : MonoBehaviour
{
    public enum QuestionType
    {
        ImageRecognition,
        DetailMemory
    }

    [Serializable]
    public class QuizQuestion
    {
        [Header("Exhibit")]
        [Tooltip("1-based index in ExperimentManager.exhibits")]
        public int exhibitNumber = 1;

        public QuestionType questionType;

        [TextArea(2, 3)]
        public string questionText;

        [Header("Image Recognition")]
        public Sprite correctImage;
        public Sprite similarImage;

        [Header("Detail Memory")]
        public string correctAnswer;
        public string wrongAnswer1;
        public string wrongAnswer2;
        public string wrongAnswer3;
    }

    [Header("Experiment")]
    public ExperimentManager experimentManager;

    [Header("Common UI")]
    public GameObject imageOptions;
    public GameObject textOptions;
    public TMP_Text questionText;
    public TMP_Text progressText;
    public Button nextButton;

    [Header("Image Question UI")]
    public Button imageButtonA;
    public Button imageButtonB;
    public Image imageA;
    public Image imageB;

    [Header("Text Question UI")]
    public Button[] textButtons = new Button[4];
    public TMP_Text[] textButtonLabels = new TMP_Text[4];

    [Header("Selection Border")]
    public Color selectionBorderColor =
        new Color(0.25f, 0.50f, 0.90f, 1f);

    [Min(1f)]
    public float selectionBorderWidth = 3f;

    [Header("Results Screen")]
    public GameObject resultsPanel;
    public TMP_Text accuracyValue;
    public TMP_Text correctCountText;

    [Header("CSV Export")]
    public QuizCSVExporter csvExporter;

    private float questionStartTime;

    [Header("Questions")]
    public List<QuizQuestion> questions =
        new List<QuizQuestion>();

    private List<QuizQuestion> quizOrder;

    private int currentQuestionIndex;
    private int correctOptionIndex;
    private int selectedOptionIndex = -1;

    private int photoCorrect;
    private int photoTotal;
    private int observeCorrect;
    private int observeTotal;

    private bool quizFinished;
    public bool QuizFinished => quizFinished;
    public void CompensatePause(float seconds) { questionStartTime += seconds; }

    private void Awake()
    {
        if (imageButtonA != null)
            imageButtonA.onClick.AddListener(
                () => SelectAnswer(0));

        if (imageButtonB != null)
            imageButtonB.onClick.AddListener(
                () => SelectAnswer(1));

        for (int i = 0; i < textButtons.Length; i++)
        {
            int index = i;

            if (textButtons[i] != null)
                textButtons[i].onClick.AddListener(
                    () => SelectAnswer(index));
        }

        if (nextButton != null)
            nextButton.onClick.AddListener(NextQuestion);
    }

    private void OnEnable()
    {
        StartQuiz();
    }

    private void StartQuiz()
    {
        if (experimentManager == null ||
            experimentManager.exhibits == null)
        {
            Debug.LogError(
                "MemoryQuizManager: ExperimentManager missing.");
            return;
        }

        if (questions == null || questions.Count == 0)
        {
            Debug.LogError(
                "MemoryQuizManager: No questions configured.");
            return;
        }

        quizOrder = new List<QuizQuestion>(questions);
        Shuffle(quizOrder);

        currentQuestionIndex = 0;

        photoCorrect = 0;
        photoTotal = 0;
        observeCorrect = 0;
        observeTotal = 0;

        quizFinished = false;
        selectedOptionIndex = -1;

        if (csvExporter != null)
            csvExporter.BeginSession();

        if (resultsPanel != null)
            resultsPanel.SetActive(false);

        if (nextButton != null)
            nextButton.gameObject.SetActive(true);

        ShowQuestion();
    }

    private void ShowQuestion()
    {
        QuizQuestion question =
            quizOrder[currentQuestionIndex];

        selectedOptionIndex = -1;
        questionStartTime = Time.realtimeSinceStartup;

        if (nextButton != null)
            nextButton.interactable = false;

        if (questionText != null)
            questionText.text = question.questionText;

        if (progressText != null)
        {
            progressText.text =
                "Question " + (currentQuestionIndex + 1) +
                " / " + quizOrder.Count;
        }

        bool isImageQuestion =
            question.questionType ==
            QuestionType.ImageRecognition;

        if (imageOptions != null)
            imageOptions.SetActive(isImageQuestion);

        if (textOptions != null)
            textOptions.SetActive(!isImageQuestion);

        if (isImageQuestion)
            ShowImageQuestion(question);
        else
            ShowTextQuestion(question);

        UpdateSelectionVisuals();
    }

    private void ShowImageQuestion(QuizQuestion question)
    {
        correctOptionIndex =
            UnityEngine.Random.Range(0, 2);

        if (imageA != null)
        {
            imageA.sprite = correctOptionIndex == 0
                ? question.correctImage
                : question.similarImage;
        }

        if (imageB != null)
        {
            imageB.sprite = correctOptionIndex == 1
                ? question.correctImage
                : question.similarImage;
        }
    }

    private void ShowTextQuestion(QuizQuestion question)
    {
        List<string> options = new List<string>
        {
            question.correctAnswer,
            question.wrongAnswer1,
            question.wrongAnswer2,
            question.wrongAnswer3
        };

        Shuffle(options);

        correctOptionIndex =
            options.IndexOf(question.correctAnswer);

        string[] letters = { "A. ", "B. ", "C. ", "D. " };

        for (int i = 0;
             i < 4 && i < textButtonLabels.Length;
             i++)
        {
            if (textButtonLabels[i] != null)
                textButtonLabels[i].text =
                    letters[i] + options[i];
        }
    }

    private void SelectAnswer(int index)
    {
        if (quizFinished || quizOrder == null)
            return;

        selectedOptionIndex = index;

        if (nextButton != null)
            nextButton.interactable = true;

        UpdateSelectionVisuals();

        Debug.Log("Selected option: " + (index + 1));
    }

    private void UpdateSelectionVisuals()
    {
        if (quizOrder == null ||
            currentQuestionIndex >= quizOrder.Count)
            return;

        bool isImageQuestion =
            quizOrder[currentQuestionIndex].questionType ==
            QuestionType.ImageRecognition;

        SetSelectionBorder(
            imageA != null ? imageA.rectTransform : null,
            isImageQuestion && selectedOptionIndex == 0
        );

        SetSelectionBorder(
            imageB != null ? imageB.rectTransform : null,
            isImageQuestion && selectedOptionIndex == 1
        );

        for (int i = 0; i < textButtons.Length; i++)
        {
            RectTransform target =
                textButtons[i] != null
                    ? textButtons[i].GetComponent<RectTransform>()
                    : null;

            SetSelectionBorder(
                target,
                !isImageQuestion && selectedOptionIndex == i
            );
        }
    }

    private void SetSelectionBorder(
        RectTransform target,
        bool selected)
    {
        if (target == null)
            return;

        Transform existing =
            target.Find("RuntimeSelectionBorder");

        GameObject borderRoot;

        if (existing == null)
        {
            borderRoot = new GameObject(
                "RuntimeSelectionBorder",
                typeof(RectTransform)
            );

            borderRoot.transform.SetParent(target, false);

            RectTransform rootRect =
                borderRoot.GetComponent<RectTransform>();

            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            rootRect.localScale = Vector3.one;
            rootRect.localRotation = Quaternion.identity;

            CreateBorderLine(
                borderRoot.transform,
                "Top",
                new Vector2(0f, 1f),
                Vector2.one,
                new Vector2(0f, -selectionBorderWidth),
                Vector2.zero
            );

            CreateBorderLine(
                borderRoot.transform,
                "Bottom",
                Vector2.zero,
                new Vector2(1f, 0f),
                Vector2.zero,
                new Vector2(0f, selectionBorderWidth)
            );

            CreateBorderLine(
                borderRoot.transform,
                "Left",
                Vector2.zero,
                new Vector2(0f, 1f),
                Vector2.zero,
                new Vector2(selectionBorderWidth, 0f)
            );

            CreateBorderLine(
                borderRoot.transform,
                "Right",
                new Vector2(1f, 0f),
                Vector2.one,
                new Vector2(-selectionBorderWidth, 0f),
                Vector2.zero
            );

            borderRoot.transform.SetAsLastSibling();
        }
        else
        {
            borderRoot = existing.gameObject;
        }

        borderRoot.SetActive(selected);
    }

    private void CreateBorderLine(
        Transform parent,
        string lineName,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax)
    {
        GameObject line = new GameObject(
            lineName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        line.transform.SetParent(parent, false);

        RectTransform rect =
            line.GetComponent<RectTransform>();

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;

        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;

        Image lineImage = line.GetComponent<Image>();

        lineImage.color = selectionBorderColor;
        lineImage.raycastTarget = false;
    }

    private void NextQuestion()
    {
        if (quizFinished || selectedOptionIndex < 0)
            return;

        QuizQuestion question =
            quizOrder[currentQuestionIndex];

        int exhibitIndex =
            question.exhibitNumber - 1;

        if (exhibitIndex < 0 ||
            exhibitIndex >= experimentManager.exhibits.Length ||
            experimentManager.exhibits[exhibitIndex] == null)
        {
            Debug.LogError(
                "Invalid exhibit number: " +
                question.exhibitNumber);
            return;
        }

        bool isCorrect =
            selectedOptionIndex == correctOptionIndex;

        float responseTime = Time.realtimeSinceStartup - questionStartTime;

        string selectedAnswer;
        string correctAnswer;

        if (question.questionType == QuestionType.ImageRecognition)
        {
            selectedAnswer = selectedOptionIndex == 0 ? "A" : "B";
            correctAnswer = correctOptionIndex == 0 ? "A" : "B";
        }
        else
        {
            // Read the shuffled answer shown on the selected button.
            // The first three characters are the "A. " / "B. " prefix.
            string displayed = textButtonLabels[selectedOptionIndex] != null
                ? textButtonLabels[selectedOptionIndex].text
                : "";

            selectedAnswer = displayed.Length >= 3
                ? displayed.Substring(3)
                : displayed;

            correctAnswer = question.correctAnswer;
        }

        ExhibitInteraction.TrialCondition condition =
            experimentManager.exhibits[exhibitIndex].condition;

        if (condition ==
            ExhibitInteraction.TrialCondition.Photo)
        {
            photoTotal++;

            if (isCorrect)
                photoCorrect++;
        }
        else
        {
            observeTotal++;

            if (isCorrect)
                observeCorrect++;
        }

        if (csvExporter != null)
        {
            csvExporter.RecordAnswer(
                currentQuestionIndex + 1,
                question.exhibitNumber,
                condition.ToString(),
                question.questionType.ToString(),
                selectedAnswer,
                correctAnswer,
                isCorrect,
                responseTime
            );
        }

        Debug.Log(
            "Exhibit " + question.exhibitNumber +
            " | Type: " + question.questionType +
            " | Condition: " + condition +
            " | Correct: " + isCorrect
        );

        currentQuestionIndex++;

        if (currentQuestionIndex >= quizOrder.Count)
            FinishQuiz();
        else
            ShowQuestion();
    }

    private void FinishQuiz()
    {
        quizFinished = true;
        selectedOptionIndex = -1;

        if (csvExporter != null)
            csvExporter.SaveCSV();

        UpdateSelectionVisuals();

        int totalCorrect = photoCorrect + observeCorrect;
        int totalQuestions = photoTotal + observeTotal;

        float totalAccuracy =
            totalQuestions > 0
                ? 100f * totalCorrect / totalQuestions
                : 0f;

        float photoAccuracy =
            photoTotal > 0
                ? 100f * photoCorrect / photoTotal
                : 0f;

        float observeAccuracy =
            observeTotal > 0
                ? 100f * observeCorrect / observeTotal
                : 0f;

        // Update the participant-facing results.
        if (accuracyValue != null)
            accuracyValue.text =
                totalAccuracy.ToString("F0") + "%";

        if (correctCountText != null)
        {
            correctCountText.text =
                totalCorrect + " out of " +
                totalQuestions + " correct answers";
        }

        // Keep the condition comparison in the Console.
        Debug.Log(
            "MEMORY QUIZ RESULTS\n" +
            "Overall: " + totalCorrect + "/" +
            totalQuestions + " (" +
            totalAccuracy.ToString("F1") + "%)\n" +
            "Photo: " + photoCorrect + "/" +
            photoTotal + " (" +
            photoAccuracy.ToString("F1") + "%)\n" +
            "Observe: " + observeCorrect + "/" +
            observeTotal + " (" +
            observeAccuracy.ToString("F1") + "%)"
        );

        // Display results before hiding the quiz.
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "MemoryQuizManager: ResultsPanel is not assigned.");

            if (imageOptions != null)
                imageOptions.SetActive(false);

            if (textOptions != null)
                textOptions.SetActive(false);

            if (nextButton != null)
                nextButton.gameObject.SetActive(false);

            if (questionText != null)
            {
                questionText.text =
                    "Experiment Complete!\n\n" +
                    totalAccuracy.ToString("F0") + "%";
            }

            if (progressText != null)
                progressText.text = "Completed";
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);

            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
