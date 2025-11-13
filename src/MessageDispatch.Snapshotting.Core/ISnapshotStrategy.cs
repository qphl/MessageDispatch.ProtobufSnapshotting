// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

/// <summary>
/// Defines methods for determining whether a snapshot should be written.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISnapshotStrategy<in T>
{
    /// <summary>
    /// Returns a boolean indicating whether a snapshot should be written.
    /// </summary>
    /// <param name="event">An event which may be used in the implementation of the strategy.</param>
    /// <returns>True, if a snapshot should be written, otherwise false.</returns>
    bool ShouldSnapshotForEvent(T @event);
}
