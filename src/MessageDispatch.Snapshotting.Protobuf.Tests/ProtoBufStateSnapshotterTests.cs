// Copyright (c) Pharmaxo. All rights reserved.

using System.IO.Abstractions.TestingHelpers;
using MessageDispatch.Snapshotting.Core.Tests;
using MessageDispatch.Snapshotting.Core.Tests.TestDoubles;
using PharmaxoScientific.MessageDispatch.Snapshotting.Core;
using PharmaxoScientific.MessageDispatch.Snapshotting.Protobuf;

namespace MessageDispatch.Snapshotting.Protobuf.Tests;

public class ProtoBufStateSnapshotterTests
{
    private static readonly SnapshotStateEqualityComparer<IEnumerable<TestState>> _snapshotStateEqualityComparer = new();

    private const string SnapshotVersion = "1";
    private const string SnapshotBasePath = "BasePath";

    private ProtoBufStateSnapshotter<TestState> _snapshotter;
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
        var initialState = new List<TestState> { new() { Field1 = "Wof", Field2 = 34, } };

        var expectedState = new SnapshotState<IEnumerable<TestState>>(initialState, eventNumber);

        _snapshotter.SaveSnapshot(eventNumber, initialState);

        _snapshotter = CreateSnapshotter();
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.EqualTo(expectedState).Using(_snapshotStateEqualityComparer));
    }

    [Test]
    public void GivenDirectoryDidNotExistExists_WhenSavedAndReloaded_ReturnsSnapshotState()
    {
        const int eventNumber = 34324;
        var initialState = new List<TestState> { new() { Field1 = "Wof", Field2 = 34, } };

        var expectedState = new SnapshotState<IEnumerable<TestState>>(initialState, eventNumber);

        _snapshotter.SaveSnapshot(eventNumber, initialState);

        _snapshotter = CreateSnapshotter();
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.EqualTo(expectedState).Using(_snapshotStateEqualityComparer));
    }

    [Test]
    public void GivenMultipleExistingSnapshots_WhenReloaded_ReturnsLatestSnapshotState()
    {
        const long latestEventNumber = 8978;
        var latestState = new List<TestState> { new() { Field1 = "Wo4f", Field2 = 34, } };
        var expectedState = new SnapshotState<IEnumerable<TestState>>(latestState, latestEventNumber);

        _snapshotter.SaveSnapshot(1, [new TestState { Field1 = "Wof", Field2 = 34, }]);
        _snapshotter.SaveSnapshot(10, [new TestState { Field1 = "Tam", Field2 = 98794, }]);
        _snapshotter.SaveSnapshot(478, [new TestState { Field1 = "Wof2", Field2 = 344, }]);
        _snapshotter.SaveSnapshot(latestEventNumber, latestState);

        _snapshotter = CreateSnapshotter();
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.EqualTo(expectedState).Using(_snapshotStateEqualityComparer));
    }

    private ProtoBufStateSnapshotter<TestState> CreateSnapshotter() =>
        new(
            _mockFileSystem,
            SnapshotBasePath,
            SnapshotVersion);
}
