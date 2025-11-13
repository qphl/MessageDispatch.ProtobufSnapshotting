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
        var metaData = new Dictionary<string, string>
        {
            { "type", EventTypeToSnapshotOn },
            { "created", DateTime.Now.Ticks.ToString() },
            { "content-type", "application/json" },
        };

        var serialisedData = JsonSerializer.Serialize(new { Some = "Data" });
        var serialisedCustomMetaData = JsonSerializer.Serialize(new { Wof = "Tam" });

        var eventRecord = new EventRecord(
            "event-stream",
            Uuid.NewUuid(),
            0,
            Position.Start,
            metaData,
            Encoding.UTF8.GetBytes(serialisedData),
            Encoding.UTF8.GetBytes(serialisedCustomMetaData));

        var resolvedEvent = new ResolvedEvent(eventRecord, null, null);

        Assert.That(_strategy.ShouldSnapshotForEvent(resolvedEvent), Is.True);
    }
}
