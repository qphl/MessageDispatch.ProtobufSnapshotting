// Copyright (c) Pharmaxo. All rights reserved.

using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests;

public class OnceDailySnapshotStrategyTests
{
    private OnceDailySnapshotStrategy<object> _strategy;

    [SetUp]
    public void Setup() => _strategy = new OnceDailySnapshotStrategy<object>();

    [Test]
    public void ShouldSnapshotForEvent_GivenFirstEvent_ReturnsFalse()
    {
        var resolvedEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 0);

        Assert.That(_strategy.ShouldSnapshotForEvent(resolvedEvent), Is.False);
    }

    [Test]
    public void ShouldSnapshotForEvent_GivenTwoEventsOnSameDay_ReturnsFalse()
    {
        var firstEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 0);
        var secondEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 1);

        Clock.Initialize(() => DateTime.UtcNow);
        _strategy.ShouldSnapshotForEvent(firstEvent);

        Clock.Initialize(() => DateTime.UtcNow);
        Assert.That(_strategy.ShouldSnapshotForEvent(secondEvent), Is.False);
    }

    [Test]
    public void ShouldSnapshotForEvent_GivenEventsOnDifferentDays_ReturnsTrue()
    {
        var firstEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 0);
        var secondEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 1);

        Clock.Initialize(() => DateTime.UtcNow);
        _strategy.ShouldSnapshotForEvent(firstEvent);

        Clock.Initialize(() => DateTime.UtcNow.AddDays(1));
        Assert.That(_strategy.ShouldSnapshotForEvent(secondEvent), Is.True);
    }
}
