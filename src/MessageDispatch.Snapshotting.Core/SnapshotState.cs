// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

public class SnapshotState<TState>
{
    public TState? State { get; init; }

    public int? LastHandledEventNumber { get; set; }
}
