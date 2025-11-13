// Copyright (c) Pharmaxo. All rights reserved.

using Microsoft.Extensions.Time.Testing;
using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests;

public class OnceDailySnapshotStrategyTests
{
    private FakeTimeProvider _timeProvider;
    private OnceDailySnapshotStrategy<object> _strategy;

    [SetUp]
    public void Setup()
    {
        _timeProvider = new FakeTimeProvider();
        _strategy = new OnceDailySnapshotStrategy<object>(_timeProvider);
    }

    [Test]
    public void ShouldSnapshotForEvent_GivenFirstEvent_ReturnsTrue()
    {
        var resolvedEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 0);

        Assert.That(_strategy.ShouldSnapshotForEvent(resolvedEvent), Is.True);
    }

    [Test]
    public void ShouldSnapshotForEvent_GivenTwoEventsOnSameDay_ReturnsFalse()
    {
        var firstEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 0);
        var secondEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 1);

        _timeProvider.SetUtcNow(DateTime.UtcNow);
        _strategy.ShouldSnapshotForEvent(firstEvent);

        _timeProvider.SetUtcNow(DateTime.UtcNow);
        Assert.That(_strategy.ShouldSnapshotForEvent(secondEvent), Is.False);
    }

    [Test]
    public void ShouldSnapshotForEvent_GivenEventsOnDifferentDays_ReturnsTrue()
    {
        var firstEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 0);
        var secondEvent = TestHelpers.BuildResolvedEvent("AnyEventType", 1);

        _timeProvider.SetUtcNow(DateTime.UtcNow);
        _strategy.ShouldSnapshotForEvent(firstEvent);

        _timeProvider.SetUtcNow(DateTime.UtcNow.AddDays(1));
        Assert.That(_strategy.ShouldSnapshotForEvent(secondEvent), Is.True);
    }
}
