// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

public class OnceDailySnapshotStrategy<T> : ISnapshotStrategy<T>
{
    private readonly TimeProvider _timeProvider;

    public OnceDailySnapshotStrategy(TimeProvider timeProvider) => _timeProvider = timeProvider;

    public bool ShouldSnapshotForEvent(T @event) => true;
}
