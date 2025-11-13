// Copyright (c) Pharmaxo. All rights reserved.

using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests.TestDoubles;

internal class SimpleStateProvider : IStateProvider<TestState>
{
    private readonly TestState _testState;

    public SimpleStateProvider(TestState testState) => _testState = testState;

    public TestState GetState() => _testState;
}
