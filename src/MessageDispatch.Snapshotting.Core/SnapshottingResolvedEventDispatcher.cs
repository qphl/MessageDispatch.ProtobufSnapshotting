// Copyright (c) Pharmaxo. All rights reserved.

using CorshamScience.MessageDispatch.Core;
using KurrentDB.Client;

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

public class SnapshottingResolvedEventDispatcher<TState> : IDispatcher<ResolvedEvent>
{
    private readonly IStateProvider<TState> _stateProvider;
    private readonly ISnapshotStrategy<ResolvedEvent> _snapshotStrategy;
    private readonly IStateSnapshotter<TState> _stateSnapshotter;
    private readonly IDispatcher<ResolvedEvent> _innerDispatcher;

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
