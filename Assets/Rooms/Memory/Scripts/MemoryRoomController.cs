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
        public GameObject practiceArea;
        public AudioSource officeAudio, natureAudio;
        public Light roomLight;
        public Light environmentLight;
        public Renderer[] walls;
        public MemoryAtmosphere atmosphere;
        [Header("World-space UI")]
        public TMP_Text statusText, resultText, actionText, orderText;
        public TMP_Text phaseText, feedbackText, soundText;
        public Button actionButton, orderButton, officePreviewButton, naturePreviewButton, soundButton;
        public GameObject ratingPanel, setupPanel, resultsPanel, resetConfirmation;
        public Image progressFill, timerFill;

        public MemorySession Session { get; private set; }
        public bool ResetPending => resetConfirmation != null && resetConfirmation.activeSelf;
        public bool DesktopInput { get; private set; }
        public bool IsPaused => participantPaused || applicationPaused || focusPaused;
        public string SavedResultsFile => savedFile;
        public event Action ResetPerformed;
        int firstCard = -1;
        string savedFile, exportError, feedback = "";
        EnvironmentCondition displayedCondition;
        bool applicationPaused;
        bool participantPaused, focusPaused;
        readonly bool[] interruptedRounds = new bool[2];
        public bool HasInterruption => interruptedRounds[0] || interruptedRounds[1];

        void OnApplicationPause(bool paused)
        {
            applicationPaused = paused;
            if (paused && Session != null && Session.Phase == SessionPhase.Running)
                interruptedRounds[Session.RoundIndex] = true;
        }

        void OnApplicationFocus(bool focused)
        {
            // Batch test runners have no foreground window. Native desktop apps
            // pause on focus loss so time is never spent in another application.
            if (Application.isBatchMode) return;
            SetWindowFocus(focused);
        }

        public void SetWindowFocus(bool focused)
        {
            focusPaused = !focused;
            if (!focused && Session != null && Session.Phase == SessionPhase.Running)
                interruptedRounds[Session.RoundIndex] = true;
        }

        public void SetDesktopInput()
        {
            DesktopInput = true;
            UpdateInterface();
        }

        public void TogglePause()
        {
            if (Session.Phase != SessionPhase.Running || ResetPending) return;
            participantPaused = !participantPaused;
            if (participantPaused) interruptedRounds[Session.RoundIndex] = true;
            UpdateInterface();
        }

        void Awake() { Session = new MemorySession(roundSeconds); }
        void Start() { ResetRoom(); }

        void Update()
        {
            var before = Session.Phase;
            if (!ResetPending && !IsPaused) Session.Tick(Time.deltaTime);
            if (before != Session.Phase)
            {
                StopAllCoroutines();
                UpdateInterface();
            }
            if (Session.Phase == SessionPhase.Running) UpdateRunningStatus();
        }

        public bool CanSelectCard(int index) => Session != null && Session.Phase == SessionPhase.Running
            && !ResetPending && !IsPaused && !Session.SelectionPending && index >= 0 && index < cards.Length
            && index != firstCard && !Session.Matched[index];

        public void StartOrContinue()
        {
            if (ResetPending || IsPaused) return;
            if (Session.Phase == SessionPhase.Completed) { RequestReset(); return; }
            if (Session.Phase == SessionPhase.Idle)
            {
                savedFile = exportError = null;
                Array.Clear(interruptedRounds, 0, interruptedRounds.Length);
                Session.Begin(natureFirst, Environment.TickCount);
            }
            else if (Session.Phase == SessionPhase.ReadyForNextRound) Session.Continue();
            else return;
            StopAllCoroutines();
            ResetPracticeObjects();
            firstCard = -1;
            feedback = "Select a card, then find its matching symbol.";
            ResetCards();
            ApplyCondition(Session.Condition, true);
            UpdateInterface();
        }

        public void ToggleOrder()
        {
            if (Session.Phase != SessionPhase.Idle || ResetPending) return;
            natureFirst = !natureFirst;
            ApplyCondition(natureFirst ? EnvironmentCondition.Nature : EnvironmentCondition.Office);
            UpdateInterface();
        }

        public void ToggleSound()
        {
            if (Session.Phase != SessionPhase.Idle || ResetPending || atmosphere == null) return;
            atmosphere.soundEnabled = !atmosphere.soundEnabled;
            UpdateInterface();
        }

        public void PreviewOffice() { Preview(EnvironmentCondition.Office); }
        public void PreviewNature() { Preview(EnvironmentCondition.Nature); }
        void Preview(EnvironmentCondition condition)
        {
            if (Session.Phase == SessionPhase.Idle && !ResetPending)
            {
                ApplyCondition(condition);
                UpdateInterface();
            }
        }

        public void SelectCard(int index)
        {
            if (ResetPending || IsPaused) return;
            var outcome = Session.Choose(index);
            if (outcome == CardSelection.Ignored) return;
            cards[index].ShowFace(true);
            atmosphere?.Feedback();
            if (outcome == CardSelection.FirstCard)
            {
                firstCard = index;
                feedback = "Now select a second card.";
                UpdateInterface();
                return;
            }
            feedback = "Comparing the two symbols...";
            UpdateInterface();
            StartCoroutine(ResolvePair(firstCard, index, outcome == CardSelection.Match));
        }

        IEnumerator ResolvePair(int a, int b, bool match)
        {
            float remaining = match ? .45f : .85f;
            while (remaining > 0)
            {
                if (!ResetPending && !IsPaused) remaining -= Time.deltaTime;
                yield return null;
            }
            if (Session.Phase != SessionPhase.Running) yield break;
            Session.ResolveSelection();
            if (match) { cards[a].DropMatched(); cards[b].DropMatched(); }
            else { cards[a].ShowFace(false); cards[b].ShowFace(false); }
            atmosphere?.Feedback(match);
            feedback = match ? "Pair found. Choose another card." : "Different symbols. Try another pair.";
            firstCard = -1;
            UpdateInterface();
        }

        public void RateEffort(int score)
        {
            // Repeated controller clicks must not write duplicate CSV files.
            if (ResetPending || Session.Phase != SessionPhase.Rating || score < 1 || score > 5) return;
            Session.Rate(score);
            if (Session.Phase == SessionPhase.Completed) SaveResults();
            UpdateInterface();
        }

        public void RequestReset()
        {
            if (Session.Phase == SessionPhase.Idle || resetConfirmation == null) { ResetRoom(); return; }
            if (Session.Phase == SessionPhase.Running) interruptedRounds[Session.RoundIndex] = true;
            resetConfirmation.SetActive(true);
            UpdateInterface();
        }

        public void CancelReset()
        {
            if (resetConfirmation != null) resetConfirmation.SetActive(false);
            UpdateInterface();
        }

        public void ResetRoom()
        {
            StopAllCoroutines();
            if (resetConfirmation != null) resetConfirmation.SetActive(false);
            Session.Reset();
            participantPaused = false;
            Array.Clear(interruptedRounds, 0, interruptedRounds.Length);
            firstCard = -1;
            savedFile = exportError = null;
            feedback = "";
            ResetCards();
            ResetPracticeObjects();
            ApplyCondition(natureFirst ? EnvironmentCondition.Nature : EnvironmentCondition.Office, true);
            UpdateInterface();
            ResetPerformed?.Invoke();
        }

        void ResetPracticeObjects()
        {
            foreach (var item in GetComponentsInChildren<MemoryPracticeObject>(true)) item.ResetObject();
        }

        void ResetCards()
        {
            for (int i = 0; i < cards.Length; i++) cards[i].ResetCard(Session.PairIds[i]);
        }

        void ApplyCondition(EnvironmentCondition condition, bool immediate = false)
        {
            displayedCondition = condition;
            bool natural = condition == EnvironmentCondition.Nature;
            officeProps.SetActive(!natural);
            natureProps.SetActive(natural);
            if (environmentLight != null)
            {
                // This short-range accent illuminates the background furniture,
                // while the task light, board and camera remain standardized.
                environmentLight.color = natural ? new Color(.78f, .94f, .65f) : new Color(.65f, .8f, 1f);
                environmentLight.intensity = natural ? 1.3f : 1.1f;
            }
            if (atmosphere != null) atmosphere.SetCondition(natural, immediate);
            else
            {
                officeAudio.Stop(); natureAudio.Stop();
                if (natural) natureAudio.Play(); else officeAudio.Play();
                roomLight.color = natural ? new Color(1f, .9f, .72f) : new Color(.72f, .84f, 1f);
                roomLight.intensity = natural ? 1.8f : 2.1f;
            }
            // The polished room uses separate environment finishes. The task light stays identical.
        }

        void UpdateRunningStatus()
        {
            int seconds = Mathf.CeilToInt(Session.RemainingSeconds);
            statusText.text = $"<size=52><b>{seconds / 60:00}:{seconds % 60:00}</b></size> remaining\n"
                + $"<b>{Session.MatchedPairs} / 6</b> pairs found   ·   {Session.Mismatches} mismatches";
            if (progressFill != null) progressFill.fillAmount = Session.MatchedPairs / 6f;
            if (timerFill != null)
            {
                timerFill.fillAmount = Session.RemainingSeconds / roundSeconds;
                timerFill.color = seconds <= 30 ? new Color(.95f, .69f, .35f) : new Color(.44f, .78f, .72f);
            }
        }

        void UpdateInterface()
        {
            bool idle = Session.Phase == SessionPhase.Idle;
            bool running = Session.Phase == SessionPhase.Running;
            bool rating = Session.Phase == SessionPhase.Rating;
            bool done = Session.Phase == SessionPhase.Completed;
            if (practiceArea != null) practiceArea.SetActive(idle || done);
            ratingPanel.SetActive(rating);
            if (setupPanel != null) setupPanel.SetActive(idle);
            if (resultsPanel != null) resultsPanel.SetActive(done);
            bool canAct = idle || Session.Phase == SessionPhase.ReadyForNextRound || done;
            actionButton.gameObject.SetActive(canAct);
            actionButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, done ? 80 : -10);
            actionButton.interactable = canAct && !ResetPending;
            actionText.text = Session.Phase == SessionPhase.ReadyForNextRound ? "START ROUND 2"
                : done ? "NEW SESSION" : "START ROUND 1";
            orderButton.interactable = idle && !ResetPending;
            if (officePreviewButton != null) officePreviewButton.interactable = idle && !ResetPending;
            if (naturePreviewButton != null) naturePreviewButton.interactable = idle && !ResetPending;
            if (soundButton != null) soundButton.interactable = idle && !ResetPending;
            if (soundText != null) soundText.text = atmosphere != null && atmosphere.soundEnabled ? "SOUND ON" : "SOUND OFF";
            orderText.text = natureFirst ? "NATURE  >  OFFICE  ·  CHANGE ORDER" : "OFFICE  >  NATURE  ·  CHANGE ORDER";
            if (phaseText != null) phaseText.text = idle ? $"PREVIEW / {displayedCondition.ToString().ToUpperInvariant()}"
                : done ? "BOTH ROUNDS RECORDED"
                : $"ROUND {Session.RoundIndex + 1} / 2   ·   {Session.Condition.ToString().ToUpperInvariant()}";
            if (idle)
                statusText.text = "<b>Find 6 matching pairs.</b>\nSelect two cards to reveal their symbols.\nYou have 3 minutes in each environment.\nAfter each round, rate your mental effort.";
            else if (rating)
                statusText.text = $"<b>{(Session.Results[Session.RoundIndex].completed ? "All 6 pairs found." : "Time is up.")}</b>\nHow much mental effort did that round take?\nChoose a rating below to continue.";
            else if (Session.Phase == SessionPhase.ReadyForNextRound)
                statusText.text = $"<b>Your rating is recorded.</b>\nNext: {(natureFirst ? "Office" : "Nature")}.\nThe same task, with a new card arrangement.\nStart when you are ready.";
            else if (done)
                statusText.text = "<b>Session complete.</b>\nYour two rounds are shown below.";
            if (running) UpdateRunningStatus();
            if (feedbackText != null) feedbackText.text = running ? feedback
                : idle ? (DesktopInput ? "Left click: cards and buttons\nF: task view  /  SPACE: start the round"
                    : "Point and trigger to select.\nMove with the controller's 2D input.") : "";
            if (progressFill != null) progressFill.transform.parent.gameObject.SetActive(running);
            if (timerFill != null) timerFill.transform.parent.gameObject.SetActive(running);
            // Withhold round-one scores until both ratings are collected, reducing comparison cues.
            var text = new StringBuilder();
            if (done)
            {
                for (int i = 0; i < Session.Results.Count; i++)
                {
                    var result = Session.Results[i];
                    string elapsed = result.elapsedSeconds.ToString("0.0", CultureInfo.InvariantCulture);
                    text.AppendLine($"<b>{result.condition}</b>  ·  {(result.completed ? "Completed" : "Timed out")}\n"
                        + $"{elapsed}s  |  {result.matchedPairs}/6 pairs  |  {result.mismatches} mismatches\n"
                        + $"Mental effort: {result.mentalEffort}/5\n");
                }
                if (HasInterruption) text.AppendLine("Paused session: repeat for a fair comparison.");
                text.Append(savedFile != null ? "Results saved on this device."
                    : exportError != null ? "Could not save. Please keep a photo of these results." : "");
            }
            resultText.text = text.ToString();
            resultText.rectTransform.sizeDelta = new Vector2(700, HasInterruption ? 400 : 360);
            resultText.rectTransform.anchoredPosition = new Vector2(0, HasInterruption ? -225 : -205);
            foreach (var card in cards) card.SetAvailable(CanSelectCard(card.index));
        }

        void SaveResults()
        {
            try
            {
                string folder = Path.Combine(Application.persistentDataPath, "MemoryResults");
                Directory.CreateDirectory(folder);
                savedFile = Path.Combine(folder, $"session-{DateTime.UtcNow:yyyyMMdd-HHmmssfff}.csv");
                string inputMode = DesktopInput ? "keyboard_mouse" : "xr_controller_or_simulator";
                var csv = new StringBuilder("round,condition,completed,elapsed_seconds,matched_pairs,card_flips,pair_attempts,mismatches,mental_effort_1_to_5,input_mode,app_version,ambience_enabled,round_interrupted\n");
                for (int i = 0; i < Session.Results.Count; i++)
                {
                    var r = Session.Results[i];
                    csv.AppendLine($"{i + 1},{r.condition},{r.completed},{r.elapsedSeconds.ToString("F3", CultureInfo.InvariantCulture)},{r.matchedPairs},{r.cardFlips},{r.pairAttempts},{r.mismatches},{r.mentalEffort},{inputMode},{Application.version},{(atmosphere == null || atmosphere.soundEnabled ? 1 : 0)},{(interruptedRounds[i] ? 1 : 0)}");
                }
                File.WriteAllText(savedFile, csv.ToString());
                Debug.Log("Memory session exported: " + savedFile);
            }
            catch (Exception error) { savedFile = null; exportError = error.Message; Debug.LogWarning("Could not export results: " + error.Message); }
        }
    }
}
