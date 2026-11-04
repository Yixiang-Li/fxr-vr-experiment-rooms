
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class QuizCSVExporter : MonoBehaviour
{
    private readonly List<string> rows = new List<string>();

    private string sessionId;
    public string SavedPath { get; private set; }
    public bool Interrupted { get; set; }

    public void BeginSession()
    {
        sessionId = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");

        rows.Clear();

        rows.Add(
            "SessionID,QuestionOrder,ExhibitNumber," +
            "Condition,QuestionType,SelectedAnswer," +
            "CorrectAnswer,IsCorrect,ResponseTimeSeconds,Interrupted"
        );
    }

    public void RecordAnswer(
        int questionOrder,
        int exhibitNumber,
        string condition,
        string questionType,
        string selectedAnswer,
        string correctAnswer,
        bool isCorrect,
        float responseTime)
    {
        if (rows.Count == 0)
        {
            BeginSession();
        }

        rows.Add(string.Join(",", new string[]
        {
            Escape(sessionId),
            questionOrder.ToString(),
            exhibitNumber.ToString(),
            Escape(condition),
            Escape(questionType),
            Escape(selectedAnswer),
            Escape(correctAnswer),
            isCorrect ? "1" : "0",
            responseTime.ToString(
                "F3",
                System.Globalization.CultureInfo.InvariantCulture),
            Interrupted ? "1" : "0"
        }));
    }

    public void SaveCSV()
    {
        if (rows.Count <= 1)
        {
            Debug.LogWarning(
                "QuizCSVExporter: No answers to export.");
            return;
        }

        // Save in the Unity project root folder,
        // which is the parent folder of Assets.
        string folder = Path.Combine(
            Application.persistentDataPath,
            "MuseumResults"
        );

        Directory.CreateDirectory(folder);

        string path = Path.Combine(
            folder,
            "participant_" + sessionId + ".csv"
        );

        File.WriteAllLines(
            path,
            rows,
            new UTF8Encoding(true)
        );
        SavedPath = path;

        Debug.Log("CSV SAVED TO: " + path);
    }

    private string Escape(string value)
    {
        if (value == null)
        {
            value = "";
        }

        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}
