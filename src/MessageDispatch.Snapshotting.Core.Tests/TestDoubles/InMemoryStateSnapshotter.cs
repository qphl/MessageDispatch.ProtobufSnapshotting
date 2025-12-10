// Copyright (c) Pharmaxo. All rights reserved.

using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests.TestDoubles;

public class InMemoryStateSnapshotter<TState> : IStateSnapshotter<TState>
{
    private SnapshotState<TState>? _state;

    public void SaveSnapshot(long eventNumber, TState state) => _state = new SnapshotState<TState>(state, eventNumber);

    public SnapshotState<TState>? LoadStateFromSnapshot() => _state;
}
