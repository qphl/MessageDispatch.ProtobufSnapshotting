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
        _snapshotter = new JsonSerialisingFileStateSnapshotter<TestState>(
            _mockFileSystem,
            SnapshotBasePath,
            SnapshotVersion);
    }

    [Test]
    public void LoadStateFromSnapshot_GivenNoSnapshotFiles_ReturnsEmptyState()
    {
        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.Null);
    }

    [Test]
    public void LoadStateFromSnapshot_GivenNoSnapshotFilesButDirectoryExists_ReturnsEmptyDictionary()
    {
        _mockFileSystem.Directory.CreateDirectory($"{SnapshotBasePath}/{SnapshotVersion}");

        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.Null);
    }

    [Test]
    public void LoadStateFromSnapshot_GivenPreviouslySavedSnapshot_ReturnsSnapshotState()
    {
        const int eventNumber = 34324;
        var initialState = new TestState("Wof", 34);

        var expectedState = new SnapshotState<TestState>(initialState, eventNumber);

        _snapshotter.SaveSnapshot(eventNumber, initialState);

        var loadedState = _snapshotter.LoadStateFromSnapshot();

        Assert.That(loadedState, Is.EqualTo(expectedState));
    }

    private record TestState(string Field1, int Field2);
}
