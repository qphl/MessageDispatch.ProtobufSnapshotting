// Copyright (c) Pharmaxo. All rights reserved.

using System.IO.Abstractions;

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

public class JsonSerialisingFileStateSnapshotter<TState> : IStateSnapshotter<TState>
{
    private readonly IFileSystem _fileSystem;
    private readonly string _basePath;
    private SnapshotState<TState>? _state;

    public JsonSerialisingFileStateSnapshotter(
        IFileSystem fileSystem,
        string snapshotBasePath,
        string snapshotVersion)
    {
        _fileSystem = fileSystem;
        _basePath = Path.Combine(snapshotBasePath, snapshotVersion);
    }

    public void SaveSnapshot(long eventNumber, TState state) => _state = new SnapshotState<TState>(state, eventNumber);

    public SnapshotState<TState>? LoadStateFromSnapshot() => _state;
}
