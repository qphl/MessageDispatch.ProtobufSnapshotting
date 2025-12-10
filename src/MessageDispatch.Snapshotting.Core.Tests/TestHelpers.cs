// Copyright (c) Pharmaxo. All rights reserved.

using System.Text;
using System.Text.Json;
using KurrentDB.Client;

namespace MessageDispatch.Snapshotting.Core.Tests;

internal static class TestHelpers
{
    internal static ResolvedEvent BuildResolvedEvent(string eventType, int eventNumber)
    {
        var metaData = new Dictionary<string, string>
        {
            { "type", eventType },
            { "created", DateTime.Now.Ticks.ToString() },
            { "content-type", "application/json" },
        };

        var serialisedData = JsonSerializer.Serialize(new { Some = "Data" });
        var serialisedCustomMetaData = JsonSerializer.Serialize(new { Wof = "Tam" });

        var eventRecord = new EventRecord(
            "event-stream",
            Uuid.NewUuid(),
            StreamPosition.FromInt64(eventNumber),
            Position.Start,
            metaData,
            Encoding.UTF8.GetBytes(serialisedData),
            Encoding.UTF8.GetBytes(serialisedCustomMetaData));

        var resolvedEvent = new ResolvedEvent(eventRecord, null, null);
        return resolvedEvent;
    }
}
