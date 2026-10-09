using System;
using System.Collections.Generic;

namespace FXR.Memory
{
    public enum EnvironmentCondition { Office, Nature }
    public enum SessionPhase { Idle, Running, Rating, ReadyForNextRound, Completed }
    public enum CardSelection { Ignored, FirstCard, Mismatch, Match }

    [Serializable]
    public sealed class RoundResult
    {
        public EnvironmentCondition condition;
        public bool completed;
        public float elapsedSeconds;
        public int matchedPairs;
        public int cardFlips;
        public int pairAttempts;
        public int mismatches;
        public int mentalEffort;
    }

    // Independent of scene objects so scoring, order and reset can be tested reliably.
    public sealed class MemorySession
    {
        public const int PairCount = 6;
        public const int CardCount = PairCount * 2;
        public SessionPhase Phase { get; private set; } = SessionPhase.Idle;
        public EnvironmentCondition Condition { get; private set; }
        public int RoundIndex { get; private set; }
        public float RemainingSeconds { get; private set; }
        public int MatchedPairs { get; private set; }
        public int Flips { get; private set; }
        public int Attempts { get; private set; }
        public int Mismatches { get; private set; }
        public int[] PairIds { get; } = new int[CardCount];
        public bool[] Matched { get; } = new bool[CardCount];
        public List<RoundResult> Results { get; } = new List<RoundResult>();
        public bool SelectionPending => second >= 0;

        readonly float duration;
        int first = -1, second = -1, seed;
        bool natureFirst;

        public MemorySession(float durationSeconds = 180f)
        {
            duration = Math.Max(1f, durationSeconds);
            Reset();
        }

        public void Begin(bool startWithNature, int randomSeed)
        {
            Reset();
            natureFirst = startWithNature;
            seed = randomSeed;
            StartRound();
        }

        public void Continue()
        {
            if (Phase != SessionPhase.ReadyForNextRound) return;
            RoundIndex++;
            StartRound();
        }

        void StartRound()
        {
            Condition = (RoundIndex == 0) == natureFirst
                ? EnvironmentCondition.Nature : EnvironmentCondition.Office;
            RemainingSeconds = duration;
            MatchedPairs = Flips = Attempts = Mismatches = 0;
            first = second = -1;
            Array.Clear(Matched, 0, Matched.Length);
            for (int i = 0; i < CardCount; i++) PairIds[i] = i / 2;
            var random = new Random(seed + RoundIndex * 7919);
            for (int i = CardCount - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int item = PairIds[i]; PairIds[i] = PairIds[j]; PairIds[j] = item;
            }
            Phase = SessionPhase.Running;
        }

        public CardSelection Choose(int index)
        {
            if (Phase != SessionPhase.Running || index < 0 || index >= CardCount
                || Matched[index] || SelectionPending || first == index)
                return CardSelection.Ignored;
            Flips++;
            if (first < 0) { first = index; return CardSelection.FirstCard; }
            second = index;
            Attempts++;
            if (PairIds[first] == PairIds[second]) return CardSelection.Match;
            Mismatches++;
            return CardSelection.Mismatch;
        }

        public void ResolveSelection()
        {
            if (Phase != SessionPhase.Running || !SelectionPending) return;
            if (PairIds[first] == PairIds[second])
            {
                Matched[first] = Matched[second] = true;
                MatchedPairs++;
            }
            first = second = -1;
            if (MatchedPairs == PairCount) FinishRound(true);
        }

        public void Tick(float deltaSeconds)
        {
            if (Phase != SessionPhase.Running) return;
            RemainingSeconds = Math.Max(0f, RemainingSeconds - Math.Max(0f, deltaSeconds));
            if (RemainingSeconds <= 0f) FinishRound(false);
        }

        void FinishRound(bool complete)
        {
            Results.Add(new RoundResult {
                condition = Condition, completed = complete,
                elapsedSeconds = duration - RemainingSeconds, matchedPairs = MatchedPairs,
                cardFlips = Flips, pairAttempts = Attempts, mismatches = Mismatches
            });
            first = second = -1;
            Phase = SessionPhase.Rating;
        }

        public void Rate(int score)
        {
            if (Phase != SessionPhase.Rating || score < 1 || score > 5) return;
            Results[Results.Count - 1].mentalEffort = score;
            Phase = RoundIndex == 0 ? SessionPhase.ReadyForNextRound : SessionPhase.Completed;
        }

        public void Reset()
        {
            Phase = SessionPhase.Idle;
            RoundIndex = MatchedPairs = Flips = Attempts = Mismatches = 0;
            RemainingSeconds = duration;
            Condition = EnvironmentCondition.Office;
            first = second = -1;
            Results.Clear();
            Array.Clear(Matched, 0, Matched.Length);
            for (int i = 0; i < CardCount; i++) PairIds[i] = i / 2;
        }
    }
}
