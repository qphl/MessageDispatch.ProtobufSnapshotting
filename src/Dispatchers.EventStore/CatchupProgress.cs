// <copyright file="CatchupProgress.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Dispatchers.EventStore
{
    /// <summary>
    /// Class to handle calculating catchup progress.
    /// </summary>
    public class CatchupProgress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatchupProgress"/> class.
        /// </summary>
        /// <param name="eventsProcessed">Amount of events processed.</param>
        /// <param name="startPosition">Starting poisiton of the catchup progress.</param>
        /// <param name="streamName">Name of the stream.</param>
        /// <param name="totalEvents">Total events.</param>
        public CatchupProgress(int eventsProcessed, long startPosition, string streamName, long totalEvents)
        {
            EventsProcessed = eventsProcessed;
            StartPosition = startPosition;
            StreamName = streamName;
            TotalEvents = totalEvents;
        }

        /// <summary>
        /// Gets the name of the stream.
        /// </summary>
        public string StreamName { get; private set; }

        /// <summary>
        /// Gets the amount of events processed.
        /// </summary>
        public int EventsProcessed { get; private set; }

        /// <summary>
        /// Gets the starting position in the stream.
        /// </summary>
        public long StartPosition { get; private set; }

        /// <summary>
        /// Gets the total events in the stream.
        /// </summary>
        public long TotalEvents { get; private set; }

        /// <summary>
        /// Gets the stream percentage.
        /// </summary>
        public decimal StreamPercentage
        {
            get { return (((decimal)StartPosition + EventsProcessed) / TotalEvents) * 100; }
        }

        /// <summary>
        /// Gets the catchup percentage.
        /// </summary>
        public decimal CatchupPercentage
        {
            get { return EventsProcessed == 0 ? 0.0m : (decimal)EventsProcessed / (TotalEvents - StartPosition) * 100; }
        }

        /// <summary>
        /// Generates a string based on the current catchup progress.
        /// </summary>
        /// <returns>A string detailing the Stream position and how much has been caught up.</returns>
        public override string ToString()
        {
            return
                $"[{StreamName}] Stream Pos: {StreamPercentage:0.#}% ({EventsProcessed + StartPosition}/{TotalEvents}), Caught up: {CatchupPercentage:0.#}% ({EventsProcessed}/{TotalEvents - StartPosition})";
        }
    }
}
