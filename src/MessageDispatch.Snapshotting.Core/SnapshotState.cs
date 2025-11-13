// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

/// <summary>
/// Stores the current state and associated event number.
/// </summary>
/// <param name="State">The current state.</param>
/// <param name="EventNumber">The event number associated with the current state.</param>
/// <typeparam name="TState">The type of the state.</typeparam>
public record SnapshotState<TState>(TState State, long EventNumber);
