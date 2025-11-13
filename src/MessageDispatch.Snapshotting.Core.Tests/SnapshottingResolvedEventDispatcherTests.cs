// Copyright (c) Pharmaxo. All rights reserved.

using MessageDispatch.Snapshotting.Core.Tests.TestDoubles;
using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests;

public class SnapshottingResolvedEventDispatcherTests
{
    private SnapshottingResolvedEventDispatcher<TestState> _dispatcher;
    private SimpleStrategy _snapshotStrategy;
    private DispatcherSpy _innerDispatcher;
    private InMemoryStateSnapshotter<TestState> _snapshotter;
    private SimpleStateProvider _stateProvider;

    [SetUp]
    public void Setup()
    {
        _stateProvider = new SimpleStateProvider();
        _snapshotStrategy = new SimpleStrategy();
        _snapshotter = new InMemoryStateSnapshotter<TestState>();
        _innerDispatcher = new DispatcherSpy();

        _dispatcher = new SnapshottingResolvedEventDispatcher<TestState>(
            _stateProvider,
            _snapshotStrategy,
            _snapshotter,
            _innerDispatcher);
    }

    [Test]
    public void Dispatch_WhenShouldNotSnapshot_DispatchesWithoutSnapshotting()
    {
        _snapshotStrategy.ShouldSnapshot = false;
        var resolvedEvent = TestHelpers.BuildResolvedEvent("AnyType", 0);

        _dispatcher.Dispatch(resolvedEvent);

        Assert.Multiple(() =>
        {
            Assert.That(_innerDispatcher.DispatchedEvents, Has.Count.EqualTo(1));
            Assert.That(_innerDispatcher.DispatchedEvents.First(), Is.EqualTo(resolvedEvent));
            Assert.That(_snapshotter.LoadStateFromSnapshot(), Is.Null);
        });
    }

    [Test]
    public void Dispatch_WhenShouldSnapshot_DispatchesWithSnapshotting()
    {
        _stateProvider.State = new TestState("Wof", 234);
        _snapshotStrategy.ShouldSnapshot = true;
        const int eventNumber = 0;
        var resolvedEvent = TestHelpers.BuildResolvedEvent("AnyType", eventNumber);
        var expectedState = new SnapshotState<TestState>(_stateProvider.State, eventNumber);

        _dispatcher.Dispatch(resolvedEvent);

        Assert.Multiple(() =>
        {
            Assert.That(_innerDispatcher.DispatchedEvents, Has.Count.EqualTo(1));
            Assert.That(_innerDispatcher.DispatchedEvents.First(), Is.EqualTo(resolvedEvent));
            Assert.That(_snapshotter.LoadStateFromSnapshot(), Is.EqualTo(expectedState));
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
            Assert.That(_innerDispatcher.DispatchedEvents, Has.Count.EqualTo(1));
            Assert.That(_innerDispatcher.DispatchedEvents.First(), Is.EqualTo(resolvedEvent));
            Assert.That(_snapshotter.LoadStateFromSnapshot(), Is.Null);
        });
    }
}
