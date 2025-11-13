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

    [SetUp]
    public void Setup()
    {
        var testState = new TestState("Test", 123);
        var stateProvider = new SimpleStateProvider(testState);
        _snapshotStrategy = new SimpleStrategy();
        _snapshotter = new InMemoryStateSnapshotter<TestState>();
        _innerDispatcher = new DispatcherSpy();

        _dispatcher = new SnapshottingResolvedEventDispatcher<TestState>(
            stateProvider,
            _snapshotStrategy,
            _snapshotter,
            _innerDispatcher);
    }

    [Test]
    public void Dispatch_WhenShouldNotSnapshot_DispatchesWithoutSnapshotting()
    {
        _snapshotStrategy.ShouldSnapshot = false;
        var resolvedEvent = TestHelpers.BuildResolvedEvent("AnyType");

        _dispatcher.Dispatch(resolvedEvent);

        Assert.Multiple(() =>
        {
            Assert.That(_innerDispatcher.DispatchedEvents, Has.Count.EqualTo(1));
            Assert.That(_innerDispatcher.DispatchedEvents.First(), Is.EqualTo(resolvedEvent));
            Assert.That(_snapshotter.LoadStateFromSnapshot(), Is.Null);
        });
    }
}
