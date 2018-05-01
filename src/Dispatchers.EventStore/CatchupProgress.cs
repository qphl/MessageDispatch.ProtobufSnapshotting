// <copyright file="CatchupProgress.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Dispatchers.EventStore
{
    public class CatchupProgress
    {
        public CatchupProgress(int eventsProcessed, long startPosition, string streamName, long totalEvents)
        {
            EventsProcessed = eventsProcessed;
            StartPosition = startPosition;
            StreamName = streamName;
            TotalEvents = totalEvents;
        }

        public string StreamName { get; private set; }

        public int EventsProcessed { get; private set; }

        public long StartPosition { get; private set; }

        public long TotalEvents { get; private set; }

        public decimal StreamPercentage
        {
            get { return (((decimal)StartPosition + EventsProcessed) / TotalEvents) * 100; }
        }

        public decimal CatchupPercentage
        {
            get { return ((decimal)EventsProcessed / (TotalEvents - StartPosition)) * 100; }
        }

        public override string ToString()
        {
            return
                $"[{StreamName}] Stream Pos: {StreamPercentage:0.#}% ({EventsProcessed + StartPosition}/{TotalEvents}), Caught up: {CatchupPercentage:0.#}% ({EventsProcessed}/{TotalEvents - StartPosition})";
        }
    }
}
