// Copyright (c) Pharmaxo. All rights reserved.

using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests.TestDoubles;

internal class SimpleStrategy : ISnapshotStrategy<TestState>
{
    public bool ShouldSnapshot { get; set; }

    public bool ShouldSnapshotForEvent(TestState @event) => ShouldSnapshot;
}
