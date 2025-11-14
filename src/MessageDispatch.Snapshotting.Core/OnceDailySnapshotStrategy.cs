// Copyright (c) Pharmaxo. All rights reserved.

using System;

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

/// <summary>
/// An implementation of the <see cref="ISnapshotStrategy{T}"/>
/// that returns true if the last written snapshot was on the previous day.
/// </summary>
/// <typeparam name="T">The type of the event (unused in this strategy).</typeparam>
public class OnceDailySnapshotStrategy<T> : ISnapshotStrategy<T>
{
    private DateTime? _lastSnapshotTime;

    /// <inheritdoc />
    public bool ShouldSnapshotForEvent(T @event)
    {
        var now = Clock.Now;
        if (now.Date <= _lastSnapshotTime?.Date)
        {
            return false;
        }

        _lastSnapshotTime = now;
        return true;
    }
}
