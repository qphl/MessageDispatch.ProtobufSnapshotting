// Copyright (c) Pharmaxo. All rights reserved.

using System.IO.Abstractions.TestingHelpers;
using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests;

public class JsonSerialisingFileStateSnapshotterTests
{
    private const string SnapshotVersion = "1";
    private const string SnapshotBasePath = "BasePath";

    private JsonSerialisingFileStateSnapshotter<TestState> _snapshotter;
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
        var initialState = new TestState("Wof", 34);

        var expectedState = new SnapshotState<TestState>(initialState, eventNumber);

        _snapshotter.SaveSnapshot(eventNumber, initialState);

        _snapshotter = CreateSnapshotter();
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.EqualTo(expectedState));
    }

    [Test]
    public void GivenDirectoryDidNotExistExists_WhenSavedAndReloaded_ReturnsSnapshotState()
    {
        const int eventNumber = 34324;
        var initialState = new TestState("Wof", 34);

        var expectedState = new SnapshotState<TestState>(initialState, eventNumber);

        _snapshotter.SaveSnapshot(eventNumber, initialState);

        _snapshotter = CreateSnapshotter();
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.EqualTo(expectedState));
    }

    [Test]
    public void GivenMultipleExistingSnapshots_WhenReloaded_ReturnsLatestSnapshotState()
    {
        const long latestEventNumber = 8978;
        var latestState = new TestState("TheLatestOne", 99);
        var expectedState = new SnapshotState<TestState>(latestState, latestEventNumber);

        _snapshotter.SaveSnapshot(1, new TestState("Wof", 34));
        _snapshotter.SaveSnapshot(10, new TestState("Tam", 34345));
        _snapshotter.SaveSnapshot(478, new TestState("Foo", 79878));
        _snapshotter.SaveSnapshot(latestEventNumber, latestState);

        _snapshotter = CreateSnapshotter();
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.EqualTo(expectedState));
    }

    private record TestState(string Field1, int Field2);

    private JsonSerialisingFileStateSnapshotter<TestState> CreateSnapshotter() =>
        new(
            _mockFileSystem,
            SnapshotBasePath,
            SnapshotVersion);
}
