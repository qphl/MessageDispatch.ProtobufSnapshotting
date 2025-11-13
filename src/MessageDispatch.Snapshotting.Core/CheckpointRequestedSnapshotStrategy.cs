// Copyright (c) Pharmaxo. All rights reserved.

using KurrentDB.Client;

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

public class CheckpointRequestedSnapshotStrategy : ISnapshotStrategy<ResolvedEvent>
{
    public bool ShouldSnapshotForEvent(ResolvedEvent @event) => @event.Event.EventType ==  "CheckpointRequested";
}
