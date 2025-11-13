// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

/// <summary>
/// An implementation of the <see cref="ISnapshotStrategy{T}"/>
/// that returns true if the last written snapshot was on the previous day.
/// </summary>
/// <typeparam name="T">The type of the event (unused in this strategy).</typeparam>
public class OnceDailySnapshotStrategy<T> : ISnapshotStrategy<T>
{
    private readonly TimeProvider _timeProvider;
    private DateTime? _lastSnapshotTime;

    /// <summary>
    /// Initialises a new instance of the <see cref="OnceDailySnapshotStrategy{T}"/>.
    /// </summary>
    /// <param name="timeProvider">An abstraction of the time provider to facilitate unit testing.</param>
    public OnceDailySnapshotStrategy(TimeProvider timeProvider) => _timeProvider = timeProvider;

    /// <inheritdoc />
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
