using System.Linq;
using FXR.Memory;
using NUnit.Framework;

public sealed class MemorySessionTests
{
    [Test]
    public void TwoRoundSessionRecordsScoresAndRequiresRatings()
    {
        var session = new MemorySession(180f);
        session.Begin(false, 1234);
        Assert.That(session.Condition, Is.EqualTo(EnvironmentCondition.Office));
        int first = 0;
        int different = Enumerable.Range(1, 11).First(i => session.PairIds[i] != session.PairIds[first]);
        session.Choose(first);
        Assert.That(session.Choose(first), Is.EqualTo(CardSelection.Ignored));
        Assert.That(session.Choose(different), Is.EqualTo(CardSelection.Mismatch));
        Assert.That(session.Choose(5), Is.EqualTo(CardSelection.Ignored));
        session.ResolveSelection();
        session.Tick(12.5f);
        Solve(session);
        Assert.That(session.Phase, Is.EqualTo(SessionPhase.Rating));
        Assert.That(session.Results[0].completed, Is.True);
        Assert.That(session.Results[0].mismatches, Is.EqualTo(1));
        Assert.That(session.Results[0].elapsedSeconds, Is.EqualTo(12.5f).Within(0.01));
        session.Continue();
        Assert.That(session.RoundIndex, Is.Zero);
        session.Rate(3);
        session.Continue();
        Assert.That(session.Condition, Is.EqualTo(EnvironmentCondition.Nature));
        Solve(session);
        session.Rate(1);
        Assert.That(session.Phase, Is.EqualTo(SessionPhase.Completed));
        Assert.That(session.Results.Select(r => r.mentalEffort), Is.EqualTo(new[] {3, 1}));
    }

    [Test]
    public void TimeoutAndResetDoNotCarryStateIntoNewParticipant()
    {
        var session = new MemorySession(10f);
        session.Begin(true, 55);
        Assert.That(session.Condition, Is.EqualTo(EnvironmentCondition.Nature));
        session.Choose(0);
        session.Tick(100f);
        session.ResolveSelection();
        Assert.That(session.Results[0].elapsedSeconds, Is.EqualTo(10f));
        Assert.That(session.Results[0].completed, Is.False);
        Assert.That(session.Choose(1), Is.EqualTo(CardSelection.Ignored));
        session.Reset();
        Assert.That(session.Phase, Is.EqualTo(SessionPhase.Idle));
        Assert.That(session.Results, Is.Empty);
        Assert.That(session.Flips, Is.Zero);
        Assert.That(session.SelectionPending, Is.False);
    }

    static void Solve(MemorySession session)
    {
        for (int pair = 0; pair < MemorySession.PairCount; pair++)
        {
            int[] indices = Enumerable.Range(0, 12).Where(i => session.PairIds[i] == pair).ToArray();
            session.Choose(indices[0]); session.Choose(indices[1]); session.ResolveSelection();
        }
    }
}
