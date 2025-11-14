// Copyright (c) Pharmaxo. All rights reserved.

using System.IO.Abstractions.TestingHelpers;
using MessageDispatch.Snapshotting.Core.Tests;
using PharmaxoScientific.MessageDispatch.Snapshotting.Core;
using PharmaxoScientific.MessageDispatch.Snapshotting.Protobuf;

namespace MessageDispatch.Snapshotting.Protobuf.Tests;

public class ProtoBufStateSnapshotterTests
{
    private static readonly SnapshotStateEqualityComparer<IEnumerable<object>> _snapshotStateEqualityComparer = new();

    private const string SnapshotVersion = "1";
    private const string SnapshotBasePath = "BasePath";

    private ProtoBufStateSnapshotter _snapshotter;
    private MockFileSystem _mockFileSystem;

    [SetUp]
    public void Setup()
    {
        _mockFileSystem = new MockFileSystem();
        _snapshotter = CreateSnapshotter();
    }

    [Test]
    public void GivenNoSnapshotFilesOrDirectory_ReturnsEmptyState()
    {
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.Null);
    }

    [Test]
    public void GivenNoSnapshotFilesButDirectoryExists_ReturnsEmptyDictionary()
    {
        _mockFileSystem.Directory.CreateDirectory($"{SnapshotBasePath}/{SnapshotVersion}");

        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.Null);
    }

    [Test]
    public void GivenDirectoryAlreadyExists_WhenSavedAndReloaded_ReturnsSnapshotState()
    {
        _mockFileSystem.Directory.CreateDirectory($"{SnapshotBasePath}/{SnapshotVersion}/");

        const int eventNumber = 34324;
        var initialState = new List<object> { "2", "3", "5" };

        var expectedState = new SnapshotState<IEnumerable<object>>(initialState, eventNumber);

        _snapshotter.SaveSnapshot(eventNumber, initialState);

        _snapshotter = CreateSnapshotter();
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.EqualTo(expectedState).Using(_snapshotStateEqualityComparer));
    }

    [Test]
    public void GivenDirectoryDidNotExistExists_WhenSavedAndReloaded_ReturnsSnapshotState()
    {
        const int eventNumber = 34324;
        var initialState = new List<object> { "2", "3", "5" };

        var expectedState = new SnapshotState<IEnumerable<object>>(initialState, eventNumber);

        _snapshotter.SaveSnapshot(eventNumber, initialState);

        _snapshotter = CreateSnapshotter();
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.EqualTo(expectedState).Using(_snapshotStateEqualityComparer));
    }

    [Test]
    public void GivenMultipleExistingSnapshots_WhenReloaded_ReturnsLatestSnapshotState()
    {
        const long latestEventNumber = 8978;
        var latestState = new List<object> { "2", "3", "5" };
        var expectedState = new SnapshotState<IEnumerable<object>>(latestState, latestEventNumber);

        _snapshotter.SaveSnapshot(1, new List<object> { "24", "33", "55" });
        _snapshotter.SaveSnapshot(10, new List<object> { "72", "63", "85" });
        _snapshotter.SaveSnapshot(478, new List<object> { "52", "37", "565" });
        _snapshotter.SaveSnapshot(latestEventNumber, latestState);

        _snapshotter = CreateSnapshotter();
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.EqualTo(expectedState).Using(_snapshotStateEqualityComparer));
    }

    private ProtoBufStateSnapshotter CreateSnapshotter() =>
        new(
            _mockFileSystem,
            SnapshotBasePath,
            SnapshotVersion);
}
