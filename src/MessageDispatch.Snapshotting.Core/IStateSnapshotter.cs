// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

/// <summary>
/// Defines methods for saving and loading snapshots.
/// </summary>
/// <typeparam name="TState">The type of the snapshot.</typeparam>
public interface IStateSnapshotter<TState>
{
    /// <summary>
    /// Saves the snapshot.
    /// </summary>
    /// <param name="eventNumber">The event number associated with the current state.</param>
    /// <param name="state">The current state.</param>
    void SaveSnapshot(long eventNumber, TState state);

    /// <summary>
    /// Loads the latest state from a snapshot.
    /// </summary>
    /// <returns>A <see cref="SnapshotState{TState}"/> containing the state and its associated event number.</returns>
    SnapshotState<TState> LoadStateFromSnapshot();
}
