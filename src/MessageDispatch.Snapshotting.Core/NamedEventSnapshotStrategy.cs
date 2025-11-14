// Copyright (c) Pharmaxo. All rights reserved.

using KurrentDB.Client;

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core
{
    /// <summary>
    /// An implementation of <see cref="ISnapshotStrategy{T}"/>
    /// that returns true if the type of event matches the configured event.
    /// </summary>
    public class NamedEventSnapshotStrategy : ISnapshotStrategy<ResolvedEvent>
    {
        private readonly string _eventTypeToSnapshotOn;

        /// <summary>
        /// Initialises a new instance of the <see cref="NamedEventSnapshotStrategy"/>.
        /// </summary>
        /// <param name="eventTypeToSnapshotOn">The event type that should trigger a snapshot to be written.</param>
        public NamedEventSnapshotStrategy(string eventTypeToSnapshotOn) => _eventTypeToSnapshotOn = eventTypeToSnapshotOn;

        /// <inheritdoc />
        public bool ShouldSnapshotForEvent(ResolvedEvent @event) => @event.Event.EventType == _eventTypeToSnapshotOn;
    }
}
