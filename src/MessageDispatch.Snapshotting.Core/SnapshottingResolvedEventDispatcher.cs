// Copyright (c) Pharmaxo. All rights reserved.

using CorshamScience.MessageDispatch.Core;
using KurrentDB.Client;

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

/// <summary>
/// An <see cref="IDispatcher{TMessage}"/>
/// that conditionally writes a snapshot of the current system state based on rules defined by a <see cref="ISnapshotStrategy{T}"/>.
/// </summary>
/// <typeparam name="TState"></typeparam>
public class SnapshottingResolvedEventDispatcher<TState> : IDispatcher<ResolvedEvent>
{
    private readonly IStateProvider<TState> _stateProvider;
    private readonly ISnapshotStrategy<ResolvedEvent> _snapshotStrategy;
    private readonly IStateSnapshotter<TState> _stateSnapshotter;
    private readonly IDispatcher<ResolvedEvent> _innerDispatcher;

    /// <summary>
    /// Initialises a new instance of the <see cref="SnapshottingResolvedEventDispatcher{TState}"/>.
    /// </summary>
    /// <param name="stateProvider">Provides the state to be stored as a snapshot.</param>
    /// <param name="snapshotStrategy">Determines whether a snapshot should be written.</param>
    /// <param name="stateSnapshotter">Saves a snapshot.</param>
    /// <param name="innerDispatcher">An inner dispatcher, which this class wraps.</param>
    public SnapshottingResolvedEventDispatcher(
        IStateProvider<TState> stateProvider,
        ISnapshotStrategy<ResolvedEvent> snapshotStrategy,
        IStateSnapshotter<TState> stateSnapshotter,
        IDispatcher<ResolvedEvent> innerDispatcher)
    {
        _stateProvider = stateProvider;
        _snapshotStrategy = snapshotStrategy;
        _stateSnapshotter = stateSnapshotter;
        _innerDispatcher = innerDispatcher;
    }

    /// <inheritdoc />
    public void Dispatch(ResolvedEvent message)
    {
        if (_snapshotStrategy.ShouldSnapshotForEvent(message))
        {
            var state = _stateProvider.GetState();

            if (state != null)
            {
                _stateSnapshotter.SaveSnapshot(message.OriginalEventNumber.ToInt64(), state);
            }
        }

        _innerDispatcher.Dispatch(message);
    }
}
