
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StroopExperiment : MonoBehaviour
{
    private enum InkColor
    {
        Red,
        Green,
        Blue
    }

    [Header("Display")]
    [SerializeField] private GameObject stroopDisplay;
    [SerializeField] private TMP_Text stroopWord;

    [Header("Experiment")]
    [SerializeField, Min(1)] private int totalTrials = 10;

    private readonly List<float> reactionTimes = new List<float>();

    private int currentTrial;
    private int correctAnswers;
    private InkColor expectedAnswer;
    private float trialStartTime;
    private bool isRunning;
    public bool IsRunning => isRunning;
    public bool Completed { get; private set; }
    public int CurrentTrial => currentTrial;
    public int TotalTrials => totalTrials;
    public int CorrectAnswers => correctAnswers;
    public float AverageTime => reactionTimes.Count == 0 ? 0 : SumTimes() / reactionTimes.Count;
    float SumTimes() { float sum=0; foreach(float t in reactionTimes) sum+=t; return sum; }

    private void Awake()
    {
        stroopDisplay.SetActive(false);
    }

    public void StartExperiment()
    {
        if (isRunning || currentTrial > 0)
            return;

        currentTrial = 0;
        correctAnswers = 0;
        reactionTimes.Clear();

        isRunning = true;
        Completed = false;
        stroopDisplay.SetActive(true);

        ShowNextTrial();
    }

    private void ShowNextTrial()
    {
        if (currentTrial >= totalTrials)
        {
            FinishExperiment();
            return;
        }

        InkColor word = (InkColor)UnityEngine.Random.Range(0, 3);
        InkColor ink = (InkColor)UnityEngine.Random.Range(0, 3);

        stroopWord.text = word.ToString().ToUpperInvariant();
        stroopWord.color = GetColor(ink);

        expectedAnswer = ink;
        currentTrial++;
        trialStartTime = Time.time;
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
        if (!isRunning)
            return;

        float reactionTime = Time.time - trialStartTime;
        bool correct = answer == expectedAnswer;

        reactionTimes.Add(reactionTime);

        if (correct)
            correctAnswers++;

        Debug.Log(
            $"Trial {currentTrial}: " +
            $"Correct={correct}, " +
            $"ReactionTime={reactionTime:F3}s"
        );

        ShowNextTrial();
    }

    private void FinishExperiment()
    {
        isRunning = false;
        Completed = true;
        stroopDisplay.SetActive(false);

        float totalTime = 0f;

        foreach (float time in reactionTimes)
            totalTime += time;

        float averageTime = reactionTimes.Count > 0
            ? totalTime / reactionTimes.Count
            : 0f;

        Debug.Log(
            $"Experiment finished. " +
            $"Accuracy: {correctAnswers}/{totalTrials}. " +
            $"Average reaction time: {averageTime:F3}s"
        );
    }

    public void ResetExperiment()
    {
        isRunning = false;
        Completed = false;
        currentTrial = 0;
        correctAnswers = 0;
        reactionTimes.Clear();

        stroopDisplay.SetActive(false);
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
