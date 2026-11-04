
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class DistractorTask : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText;
    public TMP_Text questionText;
    public TMP_InputField answerInput;
    public Button submitButton;

    [Header("Task Settings")]
    public float taskDuration = 60f;
    public bool testMode = false;
    public float testDuration = 5f;

    [Header("Next Phase")]
    public GameObject memoryQuizPanel;

    private float remainingTime;
    private int correctAnswer;
    private bool isRunning = false;

    private int totalAnswers = 0;
    private int correctAnswers = 0;

    private void Awake()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SubmitAnswer);
        }
    }

    private void OnEnable()
    {
        StartTask();
    }

    private void Update()
    {
        if (!isRunning)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            UpdateTimer();
            FinishTask();
            return;
        }

        UpdateTimer();

        if ((Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame) ||
            (Keyboard.current != null && Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            SubmitAnswer();
        }
    }

    private void StartTask()
    {
        remainingTime = testMode
            ? testDuration
            : taskDuration;

        totalAnswers = 0;
        correctAnswers = 0;
        isRunning = true;

        if (submitButton != null)
            submitButton.interactable = true;

        if (answerInput != null)
            answerInput.interactable = true;

        UpdateTimer();
        GenerateQuestion();

        Debug.Log("Distractor task started.");
    }

    private void GenerateQuestion()
    {
        int a = Random.Range(10, 40);
        int b = Random.Range(1, 20);

        bool addition = Random.value > 0.5f;

        if (addition)
        {
            correctAnswer = a + b;
            questionText.text = a + " + " + b + " = ?";
        }
        else
        {
            correctAnswer = a - b;
            questionText.text = a + " - " + b + " = ?";
        }

        answerInput.text = "";
        answerInput.Select();
        answerInput.ActivateInputField();
    }

    public void SubmitAnswer()
    {
        if (!isRunning || answerInput == null)
            return;

        int userAnswer;

        if (!int.TryParse(answerInput.text, out userAnswer))
            return;

        totalAnswers++;

        if (userAnswer == correctAnswer)
            correctAnswers++;

        Debug.Log(
            "Distractor answer: " + userAnswer +
            " | Correct answer: " + correctAnswer
        );

        GenerateQuestion();
    }

    private void UpdateTimer()
    {
        int seconds = Mathf.CeilToInt(remainingTime);

        timerText.text =
            (seconds / 60).ToString("00") + ":" +
            (seconds % 60).ToString("00");
    }

    private void FinishTask()
    {
        isRunning = false;

        Debug.Log(
            "Distractor task finished. Correct: " +
            correctAnswers + "/" + totalAnswers
        );

        if (submitButton != null)
            submitButton.interactable = false;

        if (answerInput != null)
            answerInput.interactable = false;

        if (questionText != null)
            questionText.text = "Time is up!";

        if (memoryQuizPanel != null)
        {
            memoryQuizPanel.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError(
                "DistractorTask: MemoryQuizPanel is not assigned!"
            );
        }
    }
}
