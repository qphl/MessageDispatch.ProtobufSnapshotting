# PharmaxoScientific.MessageDispatch.Snapshotting

A package that provides a composable `IDispatcher<ResolvedEvent>` that can be used to conditionally snapshot the current state of the system.

Included:

- `ISnapshotStrategy` - an interface that can be implemented to define when a snapshot should be recorded by the dispatcher
  - `OnceDailySnapshotStrategy` and `NamedEventSnapshotStrategy` are provided as ready made implementations
- `IStateProvider` - an interface that should be implemented by a class that provides the current state that should be persisted when a snapshot is recorded
- `IStateSnapshotter` - an interface that can be implemented to define how to save and load snapshots
  - `ProtoBufStateSnapshotter` and `JsonSerialisingFileStateSnapshotter` are provided as ready made implementations
- `SnapshottingResolvedEventDispatcher` - a dispatcher that is composable from the above, which dispatches `ResolvedEvent`s to an inner dispatcher and conditionally triggers a snapshot of the current state to be recorded