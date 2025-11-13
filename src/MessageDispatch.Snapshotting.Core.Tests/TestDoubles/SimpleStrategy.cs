// Copyright (c) Pharmaxo. All rights reserved.

using KurrentDB.Client;
using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests.TestDoubles;

internal class SimpleStrategy : ISnapshotStrategy<ResolvedEvent>
{
    public bool ShouldSnapshot { get; set; }

    public bool ShouldSnapshotForEvent(ResolvedEvent @event) => ShouldSnapshot;
}
