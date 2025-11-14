// Copyright (c) Pharmaxo. All rights reserved.

using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests.TestDoubles;

internal class SimpleStateProvider : IStateProvider<TestState>
{
    public TestState? State { get; set; }

    public TestState GetState() => State!;
}
