// Copyright (c) Pharmaxo. All rights reserved.

using System.Text.Json;
using PharmaxoScientific.MessageDispatch.Snapshotting.Core;

namespace MessageDispatch.Snapshotting.Core.Tests;

public class SnapshotStateEqualityComparer<T> : IEqualityComparer<SnapshotState<T>>
{
    public bool Equals(SnapshotState<T>? x, SnapshotState<T>? y) =>
        JsonSerializer.Serialize(x) == JsonSerializer.Serialize(y);

    public int GetHashCode(SnapshotState<T> obj) => HashCode.Combine(obj.State, obj.EventNumber);
}
