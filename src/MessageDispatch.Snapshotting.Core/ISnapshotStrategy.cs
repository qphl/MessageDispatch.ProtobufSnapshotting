// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

public interface ISnapshotStrategy<T>
{
    bool ShouldSnapshotForEvent(T @event);
}
