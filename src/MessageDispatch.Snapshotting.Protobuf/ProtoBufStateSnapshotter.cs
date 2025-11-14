// Copyright (c) Pharmaxo. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using KurrentDB.Client;
using PharmaxoScientific.MessageDispatch.Snapshotting.Core;
using ProtoBuf;

namespace PharmaxoScientific.MessageDispatch.ProtobufSnapshotting;

/// <summary>
/// An implementation of <see cref="ISnapshotStrategy{T}"/> that persists its snapshots to Protobuf files.
/// </summary>
public class ProtoBufStateSnapshotter : IStateSnapshotter<IEnumerable<object>>
{
    private const string TempDirectoryName = "tmp/";
    private const int ChunkSize = 52428800;

    private readonly IFileSystem _fileSystem;
    private readonly string _snapshotBasePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProtoBufStateSnapshotter"/> class.
    /// </summary>
    /// <param name="fileSystem">An abstraction of the file system to facilitate unit testing.</param>
    /// <param name="snapshotBasePath">The base path of where the snapshots will be saved.</param>
    /// <param name="snapshotVersion">The version of the snapshot being saved.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="snapshotBasePath"/> is null, empty, or contains a potentially malicious path (e.g., path traversal).
    /// </exception>
    public ProtoBufStateSnapshotter(
        IFileSystem fileSystem,
        string snapshotBasePath,
        string snapshotVersion)
    {
        ThrowIfMaliciousFilePath(snapshotBasePath);

        _fileSystem = fileSystem;
        _snapshotBasePath = snapshotBasePath;

        if (!_fileSystem.Directory.Exists(_snapshotBasePath))
        {
            _fileSystem.Directory.CreateDirectory(_snapshotBasePath);
        }

        _snapshotBasePath += $"/{snapshotVersion}/";

        if (!_fileSystem.Directory.Exists(_snapshotBasePath))
        {
            _fileSystem.Directory.CreateDirectory(_snapshotBasePath);
        }

        // delete any temp directories on startup
        if (_fileSystem.Directory.Exists(_snapshotBasePath + TempDirectoryName))
        {
            _fileSystem.Directory.Delete(_snapshotBasePath + TempDirectoryName, true);
        }
    }

    /// <inheritdoc />
    public void SaveSnapshot(long eventNumber, IEnumerable<object> state) =>
        DoCheckpoint(StreamPosition.FromInt64(eventNumber), state);

    /// <inheritdoc />
    public SnapshotState<IEnumerable<object>> LoadStateFromSnapshot()
    {
        var checkpoint = LoadCheckpoint();
        if (checkpoint is null)
        {
            return null;
        }

        return new SnapshotState<IEnumerable<object>>(LoadObjects().ToList(), (long)checkpoint);
    }

    private int? LoadCheckpoint()
    {
        var pos = GetHighestSnapshotPosition();
        return pos == -1 ? null : pos;
    }

    private IEnumerable<object> LoadObjects()
    {
        var pos = GetHighestSnapshotPosition();

        if (pos == -1)
        {
            yield break;
        }

        var path = _snapshotBasePath + GetHighestSnapshotPosition() + "/";

        var chunksRead = 0;

        while (true)
        {
            using (var retrieveStream = StreamForChunk(chunksRead, path, FileMode.Open))
            {
                if (retrieveStream == null)
                {
                    break;
                }

                var wrappedItems = Serializer.DeserializeItems<ItemWrapper>(retrieveStream, PrefixStyle.Base128, 0);

                foreach (var item in wrappedItems)
                {
                    yield return item.Item;
                }
            }

            chunksRead++;
        }
    }

    private Stream StreamForChunk(int chunkNumber, string basePath, FileMode mode)
    {
        var filePath = basePath + chunkNumber.ToString().PadLeft(5, '0') + ".chunk";

        switch (mode)
        {
            case FileMode.Open:
                return !_fileSystem.File.Exists(filePath)
                    ? null
                    : _fileSystem.File.Open(filePath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read);
            case FileMode.Create:
                {
                    var dir = Path.GetDirectoryName(filePath)!;
                    if (!_fileSystem.Directory.Exists(dir))
                    {
                        _fileSystem.Directory.CreateDirectory(dir);
                    }

                    return _fileSystem.File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                }
            default:
                throw new NotSupportedException($"Unsupported mode: {mode}");
        }
    }

    private int GetHighestSnapshotPosition()
    {
        var directories = _fileSystem.Directory.GetDirectories(_snapshotBasePath);
        if (!directories.Any())
        {
            return -1;
        }

        return directories.Select(d => int.Parse(d.Replace(_snapshotBasePath, string.Empty))).Max();
    }

    private void DoCheckpoint(StreamPosition eventNumber, IEnumerable<object> state)
    {
        var tempPath = _snapshotBasePath + TempDirectoryName;
        _fileSystem.Directory.CreateDirectory(tempPath);

        var chunkCount = 0;
        var didMoveNext = false;

        using (var enumerator = state.GetEnumerator())
        {
            do
            {
                using var serializeStream = StreamForChunk(chunkCount, tempPath, FileMode.Create);
                while (serializeStream.Length <= ChunkSize)
                {
                    didMoveNext = enumerator.MoveNext();
                    if (!didMoveNext)
                    {
                        break;
                    }

                    Serializer.SerializeWithLengthPrefix(serializeStream, new ItemWrapper { Item = enumerator.Current }, PrefixStyle.Base128, 0);
                }

                chunkCount++;
            }
            while (didMoveNext);
        }

        _fileSystem.Directory.Move(tempPath, _snapshotBasePath + "/" + eventNumber.ToInt64());
    }

    /// <summary>
    /// Validates the specified file path to detect potentially malicious input.
    /// </summary>
    /// <param name="filePath">The file path to validate.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is null, empty, or contains a potentially malicious path (e.g., path traversal).</exception>
    private static void ThrowIfMaliciousFilePath(string filePath)
    {
        if (filePath is null || filePath.Contains("../") || filePath.Contains(@"..\"))
        {
            throw new ArgumentException("Invalid file path", filePath);
        }
    }

    [ProtoContract]
    private class ItemWrapper
    {
        [ProtoMember(1, DynamicType = true)]
        public object Item { get; set; }
    }
}
