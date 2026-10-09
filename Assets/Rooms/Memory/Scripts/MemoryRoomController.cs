using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FXR.Memory
{
    public sealed class MemoryRoomController : MonoBehaviour
    {
        [Header("Task")]
        public float roundSeconds = 180f;
        public MemoryCard[] cards;
        public bool natureFirst;
        [Header("Environment")]
        public GameObject officeProps, natureProps;
        public AudioSource officeAudio, natureAudio;
        public Light roomLight;
        public Renderer[] walls;
        [Header("World-space UI")]
        public TMP_Text statusText, resultText, actionText, orderText;
        public Button actionButton, orderButton;
        public GameObject ratingPanel;

        public MemorySession Session { get; private set; }
        int firstCard = -1;
        string savedFile;

        void Awake() { Session = new MemorySession(roundSeconds); }
        void Start() { ResetRoom(); }

        void Update()
        {
            var before = Session.Phase;
            Session.Tick(Time.deltaTime);
            if (before != Session.Phase) UpdateInterface();
            if (Session.Phase == SessionPhase.Running)
                statusText.text = $"{Session.Condition.ToString().ToUpperInvariant()}  /  ROUND {Session.RoundIndex + 1} OF 2\n"
                    + $"{Mathf.CeilToInt(Session.RemainingSeconds)} s remaining\n"
                    + $"{Session.MatchedPairs} / 6 pairs    {Session.Mismatches} mistakes";
        }

        public void StartOrContinue()
        {
            if (Session.Phase == SessionPhase.Idle || Session.Phase == SessionPhase.Completed)
            {
                savedFile = null;
                Session.Begin(natureFirst, Environment.TickCount);
            }
            else if (Session.Phase == SessionPhase.ReadyForNextRound) Session.Continue();
            else return;
            StopAllCoroutines();
            firstCard = -1;
            ResetCards();
            ApplyCondition(Session.Condition);
            UpdateInterface();
        }

        public void ToggleOrder()
        {
            if (Session.Phase != SessionPhase.Idle) return;
            natureFirst = !natureFirst;
            UpdateInterface();
        }

        public void PreviewOffice() { Preview(EnvironmentCondition.Office); }
        public void PreviewNature() { Preview(EnvironmentCondition.Nature); }
        void Preview(EnvironmentCondition condition)
        {
            if (Session.Phase == SessionPhase.Idle || Session.Phase == SessionPhase.Completed)
                ApplyCondition(condition);
        }

        public void SelectCard(int index)
        {
            var outcome = Session.Choose(index);
            if (outcome == CardSelection.Ignored) return;
            cards[index].ShowFace(true);
            if (outcome == CardSelection.FirstCard) { firstCard = index; return; }
            StartCoroutine(ResolvePair(firstCard, index, outcome == CardSelection.Match));
        }

        IEnumerator ResolvePair(int a, int b, bool match)
        {
            yield return new WaitForSeconds(match ? 0.45f : 0.85f);
            // A timeout or Reset can end a reveal before the delay finishes.
            if (Session.Phase != SessionPhase.Running) yield break;
            Session.ResolveSelection();
            if (match) { cards[a].DropMatched(); cards[b].DropMatched(); }
            else { cards[a].ShowFace(false); cards[b].ShowFace(false); }
            firstCard = -1;
            UpdateInterface();
        }

        public void RateEffort(int score)
        {
            Session.Rate(score);
            if (Session.Phase == SessionPhase.Completed) SaveResults();
            UpdateInterface();
        }

        public void ResetRoom()
        {
            StopAllCoroutines();
            Session.Reset();
            firstCard = -1;
            savedFile = null;
            ResetCards();
            ApplyCondition(EnvironmentCondition.Office);
            UpdateInterface();
        }

        void ResetCards()
        {
            for (int i = 0; i < cards.Length; i++) cards[i].ResetCard(Session.PairIds[i]);
        }

        void ApplyCondition(EnvironmentCondition condition)
        {
            bool natural = condition == EnvironmentCondition.Nature;
            officeProps.SetActive(!natural);
            natureProps.SetActive(natural);
            officeAudio.Stop(); natureAudio.Stop();
            if (natural) natureAudio.Play(); else officeAudio.Play();
            roomLight.color = natural ? new Color(1f, 0.9f, 0.72f) : new Color(0.72f, 0.84f, 1f);
            roomLight.intensity = natural ? 1.8f : 2.1f;
            foreach (var wall in walls)
                wall.material.color = natural ? new Color(0.82f, 0.86f, 0.76f) : new Color(0.69f, 0.75f, 0.81f);
        }

        void UpdateInterface()
        {
            ratingPanel.SetActive(Session.Phase == SessionPhase.Rating);
            actionButton.interactable = Session.Phase == SessionPhase.Idle
                || Session.Phase == SessionPhase.ReadyForNextRound || Session.Phase == SessionPhase.Completed;
            actionText.text = Session.Phase == SessionPhase.ReadyForNextRound ? "NEXT ENVIRONMENT"
                : Session.Phase == SessionPhase.Completed ? "NEW SESSION" : "START SESSION";
            orderButton.interactable = Session.Phase == SessionPhase.Idle;
            orderText.text = natureFirst ? "ORDER: NATURE > OFFICE" : "ORDER: OFFICE > NATURE";
            if (Session.Phase == SessionPhase.Idle)
                statusText.text = $"MEMORY / ENVIRONMENT\nFind 6 matching pairs in {Mathf.CeilToInt(roundSeconds)} seconds.\nSelect two cards with your controller ray.\nRate mental effort after each round.";
            else if (Session.Phase == SessionPhase.Rating)
                statusText.text = "ROUND FINISHED\nHow mentally demanding was that task?\n1 = very low    5 = very high";
            else if (Session.Phase == SessionPhase.ReadyForNextRound)
                statusText.text = "FIRST ROUND RECORDED\nContinue with a new card arrangement\nin the other environment.";
            else if (Session.Phase == SessionPhase.Completed)
                statusText.text = "SESSION COMPLETE\nCompare your two rounds below.\nReset clears the current session.";

            var text = new StringBuilder();
            foreach (var result in Session.Results)
                text.AppendLine($"{result.condition}: {(result.completed ? "complete" : "timeout")} | {result.elapsedSeconds:0.0}s\n"
                    + $"{result.matchedPairs}/6 pairs, {result.mismatches} mistakes, effort {(result.mentalEffort == 0 ? "pending" : result.mentalEffort.ToString())}/5");
            if (savedFile != null) text.AppendLine("Anonymous results saved locally (CSV).");
            resultText.text = text.ToString();
        }

        void SaveResults()
        {
            try
            {
                string folder = Path.Combine(Application.persistentDataPath, "MemoryResults");
                Directory.CreateDirectory(folder);
                savedFile = Path.Combine(folder, $"session-{DateTime.UtcNow:yyyyMMdd-HHmmssfff}.csv");
                var csv = new StringBuilder("round,condition,completed,elapsed_seconds,matched_pairs,card_flips,pair_attempts,mismatches,mental_effort_1_to_5\n");
                for (int i = 0; i < Session.Results.Count; i++)
                {
                    var r = Session.Results[i];
                    csv.AppendLine($"{i + 1},{r.condition},{r.completed},{r.elapsedSeconds.ToString("F3", CultureInfo.InvariantCulture)},{r.matchedPairs},{r.cardFlips},{r.pairAttempts},{r.mismatches},{r.mentalEffort}");
                }
                File.WriteAllText(savedFile, csv.ToString());
                Debug.Log("Memory session exported: " + savedFile);
            }
            catch (Exception error) { savedFile = null; Debug.LogWarning("Could not export results: " + error.Message); }
        }
    }
}
