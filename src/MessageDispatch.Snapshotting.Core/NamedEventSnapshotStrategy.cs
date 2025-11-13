// Copyright (c) Pharmaxo. All rights reserved.

using KurrentDB.Client;

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

public class NamedEventSnapshotStrategy : ISnapshotStrategy<ResolvedEvent>
{
    private readonly string _eventTypeToSnapshotOn;

    public NamedEventSnapshotStrategy(string eventTypeToSnapshotOn) => _eventTypeToSnapshotOn = eventTypeToSnapshotOn;

    public bool ShouldSnapshotForEvent(ResolvedEvent @event) => @event.Event.EventType == _eventTypeToSnapshotOn;
}
