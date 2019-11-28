// <copyright file="ProtoBufSnapshottingResolvedEventDispatcher.cs" company="Corsham Science">
// Copyright (c) Corsham Science. All rights reserved.
// </copyright>

namespace CorshamScience.MessageDispatch.ProtobufSnapshotting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Core;
    using EventStore.ClientAPI;
    using ProtoBuf;

    /// <summary>
    /// A wrapping message dispatcher that can take and load snapshots of application states using protobuf.
    /// </summary>
    public class ProtoBufSnapshottingResolvedEventDispatcher : ISnapshottingDispatcher<ResolvedEvent>
    {
        private const string TempDirectoryName = "tmp/";
        private const int ChunkSize = 52428800;

        private readonly Func<IEnumerable<object>> _stateProvider;
        private readonly string _snapshotBasePath;

        private int _catchupCheckpointCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtoBufSnapshottingResolvedEventDispatcher"/> class.
        /// </summary>
        /// <param name="stateProvider">A function to provide a list of objects that will be snapshotted.</param>
        /// <param name="snapshotBasePath">The base path of where the snapshots will be saved.</param>
        /// <param name="snapshotVersion">The version of the snapshot being saved.</param>
        // ReSharper disable once UnusedMember.Global
        public ProtoBufSnapshottingResolvedEventDispatcher(Func<IEnumerable<object>> stateProvider, string snapshotBasePath, string snapshotVersion)
        {
            _stateProvider = stateProvider;
            _snapshotBasePath = snapshotBasePath;

            if (!Directory.Exists(_snapshotBasePath))
            {
                Directory.CreateDirectory(_snapshotBasePath);
            }

            _snapshotBasePath += $"/{snapshotVersion}/";

            if (!Directory.Exists(_snapshotBasePath))
            {
                Directory.CreateDirectory(_snapshotBasePath);
            }

            // delete any temp directories on startup
            if (Directory.Exists(_snapshotBasePath + TempDirectoryName))
            {
                Directory.Delete(_snapshotBasePath + TempDirectoryName, true);
            }
        }

        /// <summary>
        /// Gets or sets the inner dispatcher which this will wrap.
        /// </summary>
        public IDispatcher<ResolvedEvent> InnerDispatcher { get; set; }

        /// <inheritdoc />
        public int? LoadCheckpoint()
        {
            var pos = GetHighestSnapshotPosition();
            return pos == -1 ? (int?)null : pos;
        }

        /// <inheritdoc />
        public IEnumerable<object> LoadObjects()
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

        /// <inheritdoc />
        public void Dispatch(ResolvedEvent message)
        {
            if (message.Event.EventType.Equals("CheckpointRequested"))
            {
                // checkpoint less often while catching up
                if (message.Event.Created < DateTime.Today)
                {
                    _catchupCheckpointCount++;
                    if (_catchupCheckpointCount % 30 == 0)
                    {
                        DoCheckpoint(message.OriginalEventNumber);
                        return;
                    }
                }
                else
                {
                    DoCheckpoint(message.OriginalEventNumber);
                    return;
                }
            }

            InnerDispatcher.Dispatch(message);
        }

        private static FileStream StreamForChunk(int chunkNumber, string basePath, FileMode mode)
        {
            var filePath = basePath + chunkNumber.ToString().PadLeft(5, '0') + ".chunk";

            if (mode == FileMode.Open && !File.Exists(filePath))
            {
                return null;
            }

            return new FileStream(filePath, mode);
        }

        private int GetHighestSnapshotPosition()
        {
            var directories = Directory.GetDirectories(_snapshotBasePath);
            if (!directories.Any())
            {
                return -1;
            }

            return directories.Select(d => int.Parse(d.Replace(_snapshotBasePath, string.Empty))).Max();
        }

        private void DoCheckpoint(long eventNumber)
        {
            var tempPath = _snapshotBasePath + TempDirectoryName;
            Directory.CreateDirectory(tempPath);
            var itemEnumerable = _stateProvider();

            var chunkCount = 0;
            var didMoveNext = false;

            using (var enumerator = itemEnumerable.GetEnumerator())
            {
                do
                {
                    using (var serializeStream = StreamForChunk(chunkCount, tempPath, FileMode.Create))
                    {
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
                }
                while (didMoveNext);
            }

            Directory.Move(tempPath, _snapshotBasePath + "/" + eventNumber);
        }

        [ProtoContract]
        private class ItemWrapper
        {
            [ProtoMember(1, DynamicType = true)]
            public object Item { get; set; }
        }
    }
}
