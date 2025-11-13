// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

public class OnceDailySnapshotStrategy<T> : ISnapshotStrategy<T>
{
    private readonly TimeProvider _timeProvider;
    private DateTime? _lastSnapshotTime;

    public OnceDailySnapshotStrategy(TimeProvider timeProvider) => _timeProvider = timeProvider;

    public bool ShouldSnapshotForEvent(T @event)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        if (now.Date <= _lastSnapshotTime?.Date)
        {
            return false;
        }

        _lastSnapshotTime = now;
        return true;
    }
}
