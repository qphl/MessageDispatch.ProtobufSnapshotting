// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

public interface IStateSnapshotter<TState>
{
    void SaveSnapshot(long eventNumber, TState state);

    SnapshotState<TState>? LoadStateFromSnapshot();
}
