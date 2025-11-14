// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

/// <summary>
/// Stores the current state and associated event number.
/// </summary>
/// <typeparam name="TState">The type of the state.</typeparam>
public class SnapshotState<TState>
{
    /// <summary>
    /// Gets the current state.
    /// </summary>
    public TState State { get; }

    /// <summary>
    /// Gets the event number associated with the current state.
    /// </summary>
    public long EventNumber { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="SnapshotState{TState}"/>.
    /// <param name="state">The current state.</param>
    /// <param name="eventNumber">The event number associated with the current state.</param>
    /// </summary>
    public SnapshotState(TState state, long eventNumber)
    {
        State = state;
        EventNumber = eventNumber;
    }
}
