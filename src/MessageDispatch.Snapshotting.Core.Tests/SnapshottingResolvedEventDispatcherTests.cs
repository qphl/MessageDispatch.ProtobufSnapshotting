// Copyright (c) Pharmaxo. All rights reserved.

using MessageDispatch.Snapshotting.Core.Tests.TestDoubles;
using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests;

public class SnapshottingResolvedEventDispatcherTests
{
    private static readonly SnapshotStateEqualityComparer<TestState> _snapshotStateEqualityComparer = new();

    private SnapshottingResolvedEventDispatcher<TestState> _dispatcher;
    private SimpleStrategy _snapshotStrategy;
    private ThrowingDispatcherSpy _innerThrowingDispatcher;
    private InMemoryStateSnapshotter<TestState> _snapshotter;
    private SimpleStateProvider _stateProvider;

    [SetUp]
    public void Setup()
    {
        _stateProvider = new SimpleStateProvider();
        _snapshotStrategy = new SimpleStrategy();
        _snapshotter = new InMemoryStateSnapshotter<TestState>();
        _innerThrowingDispatcher = new ThrowingDispatcherSpy();

        _dispatcher = new SnapshottingResolvedEventDispatcher<TestState>(
            _stateProvider,
            _snapshotStrategy,
            _snapshotter,
            _innerThrowingDispatcher);
    }

    [Test]
    public void Dispatch_WhenShouldNotSnapshot_DispatchesWithoutSnapshotting()
    {
        _snapshotStrategy.ShouldSnapshot = false;
        var resolvedEvent = TestHelpers.BuildResolvedEvent("AnyType", 0);

        _dispatcher.Dispatch(resolvedEvent);

        Assert.Multiple(() =>
        {
            Assert.That(_innerThrowingDispatcher.DispatchedEvents, Has.Count.EqualTo(1));
            Assert.That(_innerThrowingDispatcher.DispatchedEvents.First(), Is.EqualTo(resolvedEvent));
            Assert.That(_snapshotter.LoadStateFromSnapshot(), Is.Null);
        });
    }

    [Test]
    public void Dispatch_WhenShouldSnapshot_DispatchesWithSnapshotting()
    {
        _stateProvider.State = new TestState { Field1 = "Wof", Field2 = 34, };
        _snapshotStrategy.ShouldSnapshot = true;
        const int eventNumber = 0;
        var resolvedEvent = TestHelpers.BuildResolvedEvent("AnyType", eventNumber);
        var expectedState = new SnapshotState<TestState>(_stateProvider.State, eventNumber);

        _dispatcher.Dispatch(resolvedEvent);

        Assert.Multiple(() =>
        {
            Assert.That(_innerThrowingDispatcher.DispatchedEvents, Has.Count.EqualTo(1));
            Assert.That(_innerThrowingDispatcher.DispatchedEvents.First(), Is.EqualTo(resolvedEvent));
            Assert.That(_snapshotter.LoadStateFromSnapshot(), Is.EqualTo(expectedState).Using(_snapshotStateEqualityComparer));
        });
    }

    [Test]
    public void Dispatch_WhenShouldSnapshotButNothingToSnapshot_DispatchesWithoutSnapshotting()
    {
        _stateProvider.State = null;
        _snapshotStrategy.ShouldSnapshot = true;
        const int eventNumber = 0;
        var resolvedEvent = TestHelpers.BuildResolvedEvent("AnyType", eventNumber);

        _dispatcher.Dispatch(resolvedEvent);

        Assert.Multiple(() =>
        {
            Assert.That(_innerThrowingDispatcher.DispatchedEvents, Has.Count.EqualTo(1));
            Assert.That(_innerThrowingDispatcher.DispatchedEvents.First(), Is.EqualTo(resolvedEvent));
            Assert.That(_snapshotter.LoadStateFromSnapshot(), Is.Null);
        });
    }

    [Test]
    public void Dispatch_WhenShouldSnapshotButDispatchingFails_DoesNotRecordSnapshot()
    {
        _stateProvider.State = new TestState { Field1 = "Wof", Field2 = 34, };
        _snapshotStrategy.ShouldSnapshot = true;
        _innerThrowingDispatcher.ThrowOnDispatch = true;
        const int eventNumber = 0;
        var resolvedEvent = TestHelpers.BuildResolvedEvent("AnyType", eventNumber);

        Assert.Multiple(() =>
        {
            Assert.That(() => _dispatcher.Dispatch(resolvedEvent), Throws.Exception);
            Assert.That(_innerThrowingDispatcher.DispatchedEvents, Has.Count.EqualTo(1));
            Assert.That(_innerThrowingDispatcher.DispatchedEvents.First(), Is.EqualTo(resolvedEvent));
            Assert.That(_snapshotter.LoadStateFromSnapshot(), Is.Null);
        });
    }
}
