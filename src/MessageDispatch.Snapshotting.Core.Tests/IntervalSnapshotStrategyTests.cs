// Copyright (c) Pharmaxo. All rights reserved.

using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests;

public class IntervalSnapshotStrategyTests
{
    private static readonly List<TimeSpan> _timeSpanValues =
    [
        new(1, 0, 0, 0),
        new(0, 1, 0, 0),
        new(0, 0, 1, 0)
    ];

    [TestCaseSource(nameof(_timeSpanValues))]
    public void ShouldSnapshotForEvent_GivenFirstEvent_ReturnsFalse(TimeSpan timeSpan)
    {
        var strategy = new IntervalSnapshotStrategy<object>(timeSpan);
        var resolvedEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 0);

        Clock.Initialize(() => DateTime.UtcNow);

        Assert.That(strategy.ShouldSnapshotForEvent(resolvedEvent), Is.False);
    }

    [TestCaseSource(nameof(_timeSpanValues))]
    public void ShouldSnapshotForEvent_GivenTwoEventsWithinInterval_ReturnsFalse(TimeSpan timeSpan)
    {
        var strategy = new IntervalSnapshotStrategy<object>(timeSpan);

        var firstEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 0);
        var secondEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 1);

        var now = DateTime.UtcNow;
        Clock.Initialize(() => now);

        strategy.ShouldSnapshotForEvent(firstEvent);

        Clock.Initialize(() => now + timeSpan - TimeSpan.FromSeconds(1));

        Assert.That(strategy.ShouldSnapshotForEvent(secondEvent), Is.False);
    }

    [TestCaseSource(nameof(_timeSpanValues))]
    public void ShouldSnapshotForEvent_GivenEventAfterIntervalElapsed_ReturnsTrue(TimeSpan timeSpan)
    {
        var strategy = new IntervalSnapshotStrategy<object>(timeSpan);

        var firstEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 0);
        var secondEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 1);

        var now = DateTime.UtcNow;
        Clock.Initialize(() => now);

        strategy.ShouldSnapshotForEvent(firstEvent);

        Clock.Initialize(() => now +  timeSpan);

        Assert.That(strategy.ShouldSnapshotForEvent(secondEvent), Is.True);
    }

    [TestCaseSource(nameof(_timeSpanValues))]
    public void ShouldSnapshotForEvent_GivenMultipleIntervalsElapsed_ReturnsTrue(TimeSpan timeSpan)
    {
        var strategy = new IntervalSnapshotStrategy<object>(timeSpan);

        var firstEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 0);
        var secondEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 1);

        var now = DateTime.UtcNow;
        Clock.Initialize(() => now);

        strategy.ShouldSnapshotForEvent(firstEvent);

        Clock.Initialize(() => now +  timeSpan + timeSpan);

        Assert.That(strategy.ShouldSnapshotForEvent(secondEvent), Is.True);
    }
}
