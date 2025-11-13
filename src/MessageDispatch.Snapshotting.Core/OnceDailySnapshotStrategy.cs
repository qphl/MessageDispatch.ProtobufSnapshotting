// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

public class OnceDailySnapshotStrategy<T> : ISnapshotStrategy<T>
{
    private readonly TimeProvider _timeProvider;
    private bool _hasSnapshotted = false;

    public OnceDailySnapshotStrategy(TimeProvider timeProvider) => _timeProvider = timeProvider;

    public bool ShouldSnapshotForEvent(T @event)
    {
        if (!_hasSnapshotted)
        {
            _hasSnapshotted = true;
            return true;
        }

        return false;
    }
}
