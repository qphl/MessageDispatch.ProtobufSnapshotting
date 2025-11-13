// Copyright (c) Pharmaxo. All rights reserved.

using CorshamScience.MessageDispatch.Core;
using KurrentDB.Client;

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

public class SnapshottingResolvedEventDispatcher<TState> : IDispatcher<ResolvedEvent>
{
    private readonly IStateProvider<TState> _stateProvider;
    private readonly ISnapshotStrategy<TState> _snapshotStrategy;
    private readonly IStateSnapshotter<TState> _stateSnapshotter;
    private readonly IDispatcher<TState> _innerDispatcher;

    public SnapshottingResolvedEventDispatcher(
        IStateProvider<TState> stateProvider,
        ISnapshotStrategy<TState> snapshotStrategy,
        IStateSnapshotter<TState> stateSnapshotter,
        IDispatcher<TState> innerDispatcher)
    {
        _stateProvider = stateProvider;
        _snapshotStrategy = snapshotStrategy;
        _stateSnapshotter = stateSnapshotter;
        _innerDispatcher = innerDispatcher;
    }

    public void Dispatch(ResolvedEvent message)
    {
        // Does _snapshotStrategy want us to snapshot?
        //       If yes, get state from _stateProvider and tell _stateSnapshotter to snapshot
        // Dispatch event to inner dispatcher

        throw new NotImplementedException();
    }
}
