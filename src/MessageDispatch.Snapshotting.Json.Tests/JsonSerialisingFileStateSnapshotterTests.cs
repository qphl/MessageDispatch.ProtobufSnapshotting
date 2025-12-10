// Copyright (c) Pharmaxo. All rights reserved.

using System.IO.Abstractions.TestingHelpers;
using MessageDispatch.Snapshotting.Core.Tests;
using MessageDispatch.Snapshotting.Core.Tests.TestDoubles;
using PharmaxoScientific.MessageDispatch.Snapshotting.Core;
using PharmaxoScientific.MessageDispatch.Snapshotting.Json;

namespace MessageDispatch.Snapshotting.Json.Tests;

public class JsonSerialisingFileStateSnapshotterTests
{
    private static readonly SnapshotStateEqualityComparer<TestState> _snapshotStateEqualityComparer = new();

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
        var initialState = new TestState { Field1 = "Wof", Field2 = 34, };

        var expectedState = new SnapshotState<TestState>(initialState, eventNumber);

        _snapshotter.SaveSnapshot(eventNumber, initialState);

        _snapshotter = CreateSnapshotter();
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.EqualTo(expectedState).Using(_snapshotStateEqualityComparer));
    }

    [Test]
    public void GivenDirectoryDidNotExistExists_WhenSavedAndReloaded_ReturnsSnapshotState()
    {
        const int eventNumber = 34324;
        var initialState = new TestState { Field1 = "Wof4", Field2 = 3324, };

        var expectedState = new SnapshotState<TestState>(initialState, eventNumber);

        _snapshotter.SaveSnapshot(eventNumber, initialState);

        _snapshotter = CreateSnapshotter();
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.EqualTo(expectedState).Using(_snapshotStateEqualityComparer));
    }

    [Test]
    public void GivenMultipleExistingSnapshots_WhenReloaded_ReturnsLatestSnapshotState()
    {
        const long latestEventNumber = 8978;
        var latestState = new TestState { Field1 = "Wof", Field2 = 34, };
        var expectedState = new SnapshotState<TestState>(latestState, latestEventNumber);

        _snapshotter.SaveSnapshot(1, new TestState { Field1 = "Wof", Field2 = 34, });
        _snapshotter.SaveSnapshot(10, new TestState { Field1 = "Tam", Field2 = 98794, });
        _snapshotter.SaveSnapshot(478, new TestState { Field1 = "Wof2", Field2 = 344, });
        _snapshotter.SaveSnapshot(latestEventNumber, latestState);

        _snapshotter = CreateSnapshotter();
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.EqualTo(expectedState).Using(_snapshotStateEqualityComparer));
    }

    [Test]
    public void WhenCheckpointFileAlreadyExists_DoesNotOverwriteExistingSnapshotFile()
    {
        const int eventNumber = 2134234;
        const string basePath = $"{SnapshotBasePath}/{SnapshotVersion}/";
        var filePath = $"{basePath}/{eventNumber}";
        var expectedLastWriteTime = DateTime.Now;
        var state = new TestState { Field1 = "Wof", Field2 = 34, };

        _snapshotter.SaveSnapshot(eventNumber, state);

        _mockFileSystem.File.SetLastWriteTime(filePath, expectedLastWriteTime);

        _snapshotter.SaveSnapshot(eventNumber, state);

        var actualLastWriteTime = _mockFileSystem.File.GetLastWriteTime(filePath);

        Assert.That(actualLastWriteTime, Is.EqualTo(expectedLastWriteTime));
    }

    private JsonSerialisingFileStateSnapshotter<TestState> CreateSnapshotter() =>
        new(
            _mockFileSystem,
            SnapshotBasePath,
            SnapshotVersion);
}
