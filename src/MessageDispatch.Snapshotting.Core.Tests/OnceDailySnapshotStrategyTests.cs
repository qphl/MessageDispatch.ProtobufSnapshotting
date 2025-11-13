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
        var resolvedEvent = TestHelpers.BuildResolvedEvent("Anything");

        Assert.That(_strategy.ShouldSnapshotForEvent(resolvedEvent), Is.True);
    }
}
