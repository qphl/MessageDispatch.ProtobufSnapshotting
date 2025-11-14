// Copyright (c) Pharmaxo. All rights reserved.

using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;
using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Json;

/// <summary>
/// An implementation of <see cref="ISnapshotStrategy{T}"/> that persists its snapshots to a JSON file.
/// </summary>
/// <typeparam name="TState">The type of state to persist.</typeparam>
public class JsonSerialisingFileStateSnapshotter<TState> : IStateSnapshotter<TState>
{
    private readonly IFileSystem _fileSystem;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _basePath;

    /// <summary>
    /// Initialises a new instance of the <see cref="JsonSerialisingFileStateSnapshotter{TState}"/>.
    /// </summary>
    /// <param name="fileSystem">An abstraction of the file system to facilitate unit testing.</param>
    /// <param name="snapshotBasePath">The base path of the snapshot fileSystem.</param>
    /// <param name="snapshotVersion">The version of the snapshot.</param>
    /// <param name="jsonOptions">Optional JSON options for the serialiser.</param>
    public JsonSerialisingFileStateSnapshotter(
        IFileSystem fileSystem,
        string snapshotBasePath,
        string snapshotVersion,
        JsonSerializerOptions jsonOptions = null)
    {
        _fileSystem = fileSystem;
        _basePath = Path.Combine(snapshotBasePath, snapshotVersion);
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions { WriteIndented = false };
    }

    /// <inheritdoc />
    public void SaveSnapshot(long eventNumber, TState state)
    {
        if (!_fileSystem.Directory.Exists(_basePath))
        {
            _fileSystem.Directory.CreateDirectory(_basePath);
        }

        var checkpointFilePath = Path.Combine(_basePath, eventNumber.ToString());

        if (_fileSystem.File.Exists(checkpointFilePath))
        {
            return;
        }

        var json = JsonSerializer.Serialize(state, _jsonOptions);

        _fileSystem.File.WriteAllText(checkpointFilePath, json);
    }

    /// <inheritdoc />
    public SnapshotState<TState> LoadStateFromSnapshot()
    {
        if (!_fileSystem.Directory.Exists(_basePath))
        {
            return null;
        }

        var files = _fileSystem.Directory.GetFiles(_basePath);
        if (files.Length == 0)
        {
            return null;
        }

        var latest = files
            .Select(f => _fileSystem.FileInfo.New(f))
            .OrderByDescending(f => long.TryParse(f.Name, out var n) ? n : -1)
            .First();

        var json = _fileSystem.File.ReadAllText(latest.FullName);
        var state = JsonSerializer.Deserialize<TState>(json, _jsonOptions);

        var fileName = Path.GetFileName(latest.Name);
        long.TryParse(fileName, out var eventNumber);

        return new SnapshotState<TState>(state, eventNumber);
    }
}
