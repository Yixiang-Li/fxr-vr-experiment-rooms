
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StroopExperiment : MonoBehaviour
{
    private enum InkColor { Red, Green, Blue }
    private enum Phase { Idle, Block1, Transition, Block2, Finished }

    [Header("Display")]
    [SerializeField] private GameObject stroopDisplay;
    [SerializeField] private TMP_Text stroopWord;
    [SerializeField] private TMP_Text messageText;

    [Header("Digital Clock")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField, Min(1f)] private float blockDuration = 10f;

    [Header("Transition")]
    [SerializeField, Min(0f)] private float transitionDuration = 3f;

    [Header("Phone Notifications")]
    [SerializeField] private GameObject phoneScreenGlow;
    [SerializeField] private AudioSource notificationAudio;
    [SerializeField] private float notificationInterval = 2.5f;
    [SerializeField] private float notificationFlashDuration = 0.5f;

    [Header("Sound Effects")]
    [SerializeField] private RoomAudioManager roomAudio;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Start Button")]
    [SerializeField] private PhysicalButton startButton;

    private Phase phase = Phase.Idle;
    private bool hasStarted;

    private float remainingTime;
    private float trialStartTime;
    private float nextNotificationTime;

    private InkColor expectedAnswer;
    private int previousWordIndex = -1;

    private readonly List<float> reactionTimes1 = new List<float>();
    private readonly List<float> reactionTimes2 = new List<float>();

    private int correct1;
    private int correct2;

    private Coroutine transitionCoroutine;
    private Coroutine notificationCoroutine;

    private bool movementWasEnabled;

    public bool IsRunning =>
        phase == Phase.Block1 || phase == Phase.Block2;

    private void Awake()
    {
        if (stroopDisplay != null)
            stroopDisplay.SetActive(false);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        if (phoneScreenGlow != null)
            phoneScreenGlow.SetActive(false);

        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!IsRunning)
            return;

        remainingTime = Mathf.Max(
            0f,
            remainingTime - Time.deltaTime
        );

        UpdateClock();

        if (phase == Phase.Block2 &&
            Time.time >= nextNotificationTime)
        {
            TriggerNotification();

            nextNotificationTime =
                Time.time + Mathf.Max(0.1f, notificationInterval);
        }

        if (remainingTime <= 0f)
            EndCurrentBlock();
    }

    public void StartExperiment()
    {
        if (hasStarted)
            return;

        hasStarted = true;
        previousWordIndex = -1;

        reactionTimes1.Clear();
        reactionTimes2.Clear();

        correct1 = 0;
        correct2 = 0;

        if (playerMovement != null)
        {
            movementWasEnabled = playerMovement.enabled;
            playerMovement.enabled = false;
        }

        StartBlock(Phase.Block1);
    }

    private void StartBlock(Phase newPhase)
    {
        phase = newPhase;
        remainingTime = blockDuration;

        if (stroopDisplay != null)
            stroopDisplay.SetActive(true);

        if (stroopWord != null)
            stroopWord.gameObject.SetActive(true);

        if (messageText != null)
            messageText.gameObject.SetActive(false);

        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        if (phoneScreenGlow != null)
            phoneScreenGlow.SetActive(false);

        UpdateClock();

        if (phase == Phase.Block2)
        {
            nextNotificationTime =
                Time.time + Mathf.Max(0.1f, notificationInterval);
        }

        ShowNextTrial();
    }

    private void ShowNextTrial()
    {
        if (!IsRunning || stroopWord == null)
            return;

        // Never repeat the previous word.
        int wordIndex;

        if (previousWordIndex == -1)
        {
            wordIndex = Random.Range(0, 3);
        }
        else
        {
            int offset = Random.Range(1, 3);
            wordIndex = (previousWordIndex + offset) % 3;
        }

        previousWordIndex = wordIndex;

        InkColor word = (InkColor)wordIndex;

        // Ink color never matches the word.
        int inkOffset = Random.Range(1, 3);
        int inkIndex = (wordIndex + inkOffset) % 3;

        InkColor ink = (InkColor)inkIndex;

        stroopWord.text = word.ToString().ToUpperInvariant();
        stroopWord.color = GetColor(ink);

        expectedAnswer = ink;
        trialStartTime = Time.time;

        Debug.Log(
            $"Stroop trial: Word={word}, Ink={ink}, Expected={expectedAnswer}"
        );
    }

    public void SubmitRed()
    {
        SubmitAnswer(InkColor.Red);
    }

    public void SubmitGreen()
    {
        SubmitAnswer(InkColor.Green);
    }

    public void SubmitBlue()
    {
        SubmitAnswer(InkColor.Blue);
    }

    private void SubmitAnswer(InkColor answer)
    {
        if (!IsRunning)
            return;

        float reactionTime = Time.time - trialStartTime;
        bool correct = answer == expectedAnswer;

        if (phase == Phase.Block1)
        {
            reactionTimes1.Add(reactionTime);

            if (correct)
                correct1++;
        }
        else
        {
            reactionTimes2.Add(reactionTime);

            if (correct)
                correct2++;
        }

        Debug.Log(
            $"{phase}: Correct={correct}, RT={reactionTime:F3}s"
        );

        ShowNextTrial();
    }

    private void EndCurrentBlock()
    {
        UpdateClock();

        // Play timer sound at the end of BOTH blocks.
        if (roomAudio != null)
            roomAudio.PlayTimer();

        if (notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
            notificationCoroutine = null;
        }

        if (phoneScreenGlow != null)
            phoneScreenGlow.SetActive(false);

        if (phase == Phase.Block1)
        {
            phase = Phase.Transition;

            transitionCoroutine =
                StartCoroutine(TransitionToBlock2());
        }
        else if (phase == Phase.Block2)
        {
            FinishExperiment();
        }
    }

    private IEnumerator TransitionToBlock2()
    {
        ShowMessage(
            "PART 1 COMPLETE\n\nGet ready for Part 2"
        );

        yield return new WaitForSeconds(transitionDuration);

        transitionCoroutine = null;

        StartBlock(Phase.Block2);
    }

    private void ShowMessage(string text)
    {
        if (stroopDisplay != null)
            stroopDisplay.SetActive(true);

        if (stroopWord != null)
            stroopWord.gameObject.SetActive(false);

        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = text;
        }
    }

    private void TriggerNotification()
    {
        if (phoneScreenGlow == null &&
            notificationAudio == null)
            return;

        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        notificationCoroutine =
            StartCoroutine(FlashPhone());
    }

    private IEnumerator FlashPhone()
    {
        if (phoneScreenGlow != null)
            phoneScreenGlow.SetActive(true);

        if (notificationAudio != null &&
            notificationAudio.clip != null)
        {
            notificationAudio.PlayOneShot(notificationAudio.clip);
        }

        yield return new WaitForSeconds(
            Mathf.Max(0.05f, notificationFlashDuration)
        );

        if (phoneScreenGlow != null)
            phoneScreenGlow.SetActive(false);

        notificationCoroutine = null;
    }

    private void FinishExperiment()
    {
        phase = Phase.Finished;
        if (roomAudio != null)
            roomAudio.PlayExperimentEnding();

        ShowMessage(
            "EXPERIMENT COMPLETE\n\n" +
            "BLOCK 1 - NO DISTRACTION\n" +
            $"Answers: {reactionTimes1.Count}\n" +
            $"Accuracy: {Accuracy(correct1, reactionTimes1.Count):F1}%\n" +
            $"Avg RT: {AverageRT(reactionTimes1):F2}s\n\n" +
            "BLOCK 2 - DISTRACTION\n" +
            $"Answers: {reactionTimes2.Count}\n" +
            $"Accuracy: {Accuracy(correct2, reactionTimes2.Count):F1}%\n" +
            $"Avg RT: {AverageRT(reactionTimes2):F2}s"
        );

        if (playerMovement != null)
            playerMovement.enabled = movementWasEnabled;

        if (startButton != null)
            startButton.ReleaseStartVisual();
    }

    private void UpdateClock()
    {
        if (countdownText == null)
            return;

        int seconds = Mathf.CeilToInt(remainingTime);

        countdownText.text =
            $"{seconds / 60:00}:{seconds % 60:00}";
    }

    private float Accuracy(int correct, int total)
    {
        return total > 0
            ? 100f * correct / total
            : 0f;
    }

    private float AverageRT(List<float> times)
    {
        if (times.Count == 0)
            return 0f;

        float sum = 0f;

        foreach (float time in times)
            sum += time;

        return sum / times.Count;
    }

    public void ResetExperiment()
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        transitionCoroutine = null;
        notificationCoroutine = null;

        if (notificationAudio != null)
            notificationAudio.Stop();

        if (playerMovement != null && hasStarted)
            playerMovement.enabled = movementWasEnabled;

        phase = Phase.Idle;
        hasStarted = false;

        previousWordIndex = -1;
        remainingTime = blockDuration;

        reactionTimes1.Clear();
        reactionTimes2.Clear();

        correct1 = 0;
        correct2 = 0;

        if (stroopDisplay != null)
            stroopDisplay.SetActive(false);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        if (phoneScreenGlow != null)
            phoneScreenGlow.SetActive(false);

        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    private Color GetColor(InkColor color)
    {
        switch (color)
        {
            case InkColor.Red:
                return new Color(0.8f, 0.15f, 0.18f);

            case InkColor.Green:
                return new Color(0.08f, 0.52f, 0.27f);

            case InkColor.Blue:
                return new Color(0.12f, 0.38f, 0.82f);

            default:
                return Color.black;
        }
    }
}
