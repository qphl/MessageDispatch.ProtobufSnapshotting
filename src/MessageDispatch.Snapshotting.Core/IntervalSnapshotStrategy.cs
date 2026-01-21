// Copyright (c) Pharmaxo. All rights reserved.

using System;

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

/// <summary>
/// An implementation of the <see cref="ISnapshotStrategy{T}"/>
/// that returns true when a configured time interval has elapsed
/// since the last written snapshot.
/// </summary>
/// <typeparam name="T">The type of the event (unused in this strategy).</typeparam>
public class IntervalSnapshotStrategy<T> : ISnapshotStrategy<T>
{
    private readonly TimeSpan _interval;
    private DateTime? _lastSnapshotTime;

    /// <summary>
    /// Initialises a new instance of the <see cref="IntervalSnapshotStrategy{TState}"/>.
    /// </summary>
    /// <param name="interval"></param>
    public IntervalSnapshotStrategy(TimeSpan interval)
    {
        if (interval <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval));
        }

        _interval = interval;
    }

    /// <inheritdoc />
    public bool ShouldSnapshotForEvent(T @event)
    {
        var now = Clock.Now;

        if (!_lastSnapshotTime.HasValue)
        {
            _lastSnapshotTime = now;
            return false;
        }

        if (now - _lastSnapshotTime.Value < _interval)
        {
            return false;
        }

        _lastSnapshotTime = now;
        return true;
    }
}
