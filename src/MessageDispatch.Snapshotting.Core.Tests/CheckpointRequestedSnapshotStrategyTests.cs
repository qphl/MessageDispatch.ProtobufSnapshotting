using System.Text;
using System.Text.Json;
using KurrentDB.Client;
using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests;

public class CheckpointRequestedSnapshotStrategyTests
{
    private const string EventTypeToSnapshotOn = "CheckpointRequested";

    private CheckpointRequestedSnapshotStrategy _strategy;

    [SetUp]
    public void Setup() => _strategy = new CheckpointRequestedSnapshotStrategy();

    [Test]
    public void ShouldSnapshotForEvent_GivenCheckpointRequested_ReturnsTrue()
    {
        var resolvedEvent = TestHelpers.BuildResolvedEvent(EventTypeToSnapshotOn);

        Assert.That(_strategy.ShouldSnapshotForEvent(resolvedEvent), Is.True);
    }

    [TestCase("CheckpointPlease")]
    [TestCase("CheckpointNotRequested")]
    public void ShouldSnapshotForEvent_GivenCheckpointRequested_ReturnsTrue(string nonSnapshottingEventType)
    {
        var resolvedEvent = TestHelpers.BuildResolvedEvent(nonSnapshottingEventType);

        Assert.That(_strategy.ShouldSnapshotForEvent(resolvedEvent), Is.False);
    }
}
