// Copyright (c) Pharmaxo. All rights reserved.

using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests;

public class SnapshotStateEqualityComparer<T> :IEqualityComparer<SnapshotState<T>>
{
    public bool Equals(SnapshotState<T>? x, SnapshotState<T>? y) =>
        ReferenceEquals(x, y) || x is not null &&
        y is not null && x.GetType() == y.GetType() &&
        Equals(x.State, y.State) &&
        x.EventNumber == y.EventNumber;

    public int GetHashCode(SnapshotState<T> obj) => HashCode.Combine(obj.State, obj.EventNumber);
}
