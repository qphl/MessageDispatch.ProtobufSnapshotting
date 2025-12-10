// Copyright (c) Pharmaxo. All rights reserved.

using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests;

public class NamedEventSnapshotStrategyTests
{
    [TestCase("CheckpointRequested")]
    [TestCase("DoSnapshotNowPlease")]
    public void ShouldSnapshotForEvent_GivenMatchingEventType_ReturnsTrue(string snapshottingEventType)
    {
        var strategy = new NamedEventSnapshotStrategy(snapshottingEventType);
        var resolvedEvent = TestHelpers.BuildResolvedEvent(snapshottingEventType, 0);

        Assert.That(strategy.ShouldSnapshotForEvent(resolvedEvent), Is.True);
    }

    [TestCase("CheckpointPlease")]
    [TestCase("CheckpointNotRequested")]
    public void ShouldSnapshotForEvent_GivenNonMatchingEventType_ReturnsFalse(string nonSnapshottingEventType)
    {
        var strategy = new NamedEventSnapshotStrategy("CheckpointRequested");
        var resolvedEvent = TestHelpers.BuildResolvedEvent(nonSnapshottingEventType, 0);

        Assert.That(strategy.ShouldSnapshotForEvent(resolvedEvent), Is.False);
    }
}
