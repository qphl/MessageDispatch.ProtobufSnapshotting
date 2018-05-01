// <copyright file="EventStoreSubscriber.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Dispatchers.EventStore
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using Core;
    using global::EventStore.ClientAPI;
    using Timer = System.Timers.Timer;

    public class EventStoreSubscriber
    {
        private readonly WriteThroughFileCheckpoint _checkpoint;
        private readonly string _heartbeatStreamName = "SubscriberHeartbeat-" + Guid.NewGuid();
        private readonly string _heartbeatEventType = "SubscriberHeartbeat";
        private readonly Timer _liveProcessingTimer = new Timer(10 * 60 * 1000);

        private IEventStoreConnection _connection;
        private IDispatcher<ResolvedEvent> _dispatcher;

        private long? _startingPosition;
        private object _subscription;
        private object _subscriptionLock = new object();
        private string _streamName;
        private int _catchupPageSize;
        private BlockingCollection<ResolvedEvent> _queue;
        private TimeSpan _heartbeatTimeout;
        private long? _lastReceivedEventNumber;
        private bool _liveOnly;
        private Timer _heartbeatTimer;
        private DateTime _lastHeartbeat;
        private bool _usingHeartbeats = false;
        private int _maxLiveQueueSize;

        private int _eventsProcessed;

        private ILogger _logger;

        private bool _catchingUp = true;
        private long _lastNonLiveEventNumber = long.MinValue;
        private long _lastDispatchedEventNumber;

        [Obsolete("Please use new static method CreateCatchUpSubscirptionFromPosition, this constructor will be removed in the future.")]
        public EventStoreSubscriber(
            IEventStoreConnection connection,
            IDispatcher<ResolvedEvent> dispatcher,
            string streamName,
            ILogger logger,
            int? startingPosition,
            int catchUpPageSize = 1024,
            int upperQueueBound = 2048,
            TimeSpan? heartbeatFrequency = null,
            TimeSpan? heartbeatTimeout = null,
            int maxLiveQueueSize = 10000)
        {
            Init(connection, dispatcher, streamName, logger, heartbeatFrequency, heartbeatTimeout, startingPosition, catchUpPageSize, upperQueueBound, maxLiveQueueSize);
        }

        [Obsolete("Please use new static method CreateCatchupSubscriptionUsingCheckpoint, this constructor will be removed in the future.")]
        public EventStoreSubscriber(
            IEventStoreConnection connection,
            IDispatcher<ResolvedEvent> dispatcher,
            ILogger logger,
            string streamName,
            string checkpointFilePath,
            int catchupPageSize = 1024,
            int upperQueueBound = 2048,
            TimeSpan? heartbeatFrequency = null,
            TimeSpan? heartbeatTimeout = null,
            int maxLiveQueueSize = 10000)
        {
            int? startingPosition = null;
            _checkpoint = new WriteThroughFileCheckpoint(checkpointFilePath, "lastProcessedPosition", false, -1);

            var initialCheckpointPosition = _checkpoint.Read();

            if (initialCheckpointPosition != -1)
            {
                startingPosition = (int)initialCheckpointPosition;
            }

            Init(connection, dispatcher, streamName, logger, heartbeatFrequency, heartbeatTimeout, startingPosition, catchupPageSize, upperQueueBound, maxLiveQueueSize);
        }

        private EventStoreSubscriber(
            IEventStoreConnection connection,
            IDispatcher<ResolvedEvent> dispatcher,
            string streamName,
            ILogger logger,
            int upperQueueBound = 2048,
            TimeSpan? heartbeatFrequency = null,
            TimeSpan? heartbeatTImeout = null)
        {
            Init(connection, dispatcher, streamName, logger, heartbeatFrequency, heartbeatTImeout, upperQueueBound: upperQueueBound, liveOnly: true);
        }

        public CatchupProgress CatchUpPercentage
        {
            get
            {
                var totalEvents =
                    _connection.ReadStreamEventsBackwardAsync(_streamName, StreamPosition.End, 1, true).Result.Events[0]
                        .OriginalEventNumber;
                var start = _startingPosition ?? 0;
                return new CatchupProgress(_eventsProcessed, start, _streamName, totalEvents);
            }
        }

        public bool ViewModelsReady => !_catchingUp && (_lastDispatchedEventNumber >= _lastNonLiveEventNumber);

        public static EventStoreSubscriber CreateLiveSubscription(
            IEventStoreConnection connection,
            IDispatcher<ResolvedEvent> dispatcher,
            string streamName,
            ILogger logger,
            int upperQueueBound = 2048,
            TimeSpan? heartbeatFrequency = null,
            TimeSpan? heartbeatTImeout = null)
        {
            return new EventStoreSubscriber(connection, dispatcher, streamName, logger, upperQueueBound, heartbeatFrequency, heartbeatTImeout);
        }

        public static EventStoreSubscriber CreateCatchupSubscriptionUsingCheckpoint(
            IEventStoreConnection connection,
            IDispatcher<ResolvedEvent> dispatcher,
            string streamName,
            ILogger logger,
            string checkpointFilePath,
            int catchupPageSize = 1024,
            int upperQueueBound = 2048,
            TimeSpan? heartbeatFrequency = null,
            TimeSpan? heartbeatTimeout = null,
            int maxLiveQueueSize = 10000)
        {
            return new EventStoreSubscriber(connection, dispatcher, logger, streamName, checkpointFilePath, catchupPageSize, upperQueueBound, heartbeatFrequency, heartbeatTimeout, maxLiveQueueSize);
        }

        public static EventStoreSubscriber CreateCatchupSubscriptionFromPosition(
            IEventStoreConnection connection,
            IDispatcher<ResolvedEvent> dispatcher,
            string streamName,
            ILogger logger,
            int? startingPosition,
            int catchupPageSize = 1024,
            int upperQueueBound = 2048,
            TimeSpan? heartbeatFrequency = null,
            TimeSpan? heartbeatTimeout = null,
            int maxLiveQueueSize = 10000)
        {
            return new EventStoreSubscriber(connection, dispatcher, streamName, logger, startingPosition, catchupPageSize, upperQueueBound, heartbeatFrequency, heartbeatTimeout, maxLiveQueueSize);
        }

        public void SendHeartbeat()
        {
            _connection.AppendToStreamAsync(_heartbeatStreamName, ExpectedVersion.Any, new EventData(Guid.NewGuid(), _heartbeatEventType, false, new byte[0], new byte[0])).Wait();
        }

        public void Start(bool restart = false)
        {
            if (_usingHeartbeats)
            {
                SendHeartbeat();
                _heartbeatTimer.Start();
            }

            lock (_subscriptionLock)
            {
                if (_liveOnly)
                {
                    _subscription =
                        _connection.SubscribeToStreamAsync(_streamName, true, EventAppeared, SubscriptionDropped).Result;
                }
                else
                {
                    var catchUpSettings = new CatchUpSubscriptionSettings(_maxLiveQueueSize, _catchupPageSize, true, true);
                    _subscription = _connection.SubscribeToStreamFrom(_streamName, _startingPosition, catchUpSettings, EventAppeared, LiveProcessingStarted, SubscriptionDropped);
                }
            }

            if (!restart)
            {
                if (_usingHeartbeats)
                {
                    _connection.SetStreamMetadataAsync(_heartbeatStreamName, ExpectedVersion.Any, StreamMetadata.Create(maxCount: 2));
                }

                Thread processor = new Thread(ProcessEvents) { IsBackground = true };
                processor.Start();
            }
        }

        public void ShutDown()
        {
            lock (_subscriptionLock)
            {
                if (_liveOnly)
                {
                    ((EventStoreSubscription)_subscription).Close();
                }
                else
                {
                    ((EventStoreCatchUpSubscription)_subscription).Stop(TimeSpan.FromSeconds(1));
                }
            }
        }

        private void Init(
            IEventStoreConnection connection,
            IDispatcher<ResolvedEvent> dispatcher,
            string streamName,
            ILogger logger,
            TimeSpan? heartbeatFrequency,
            TimeSpan? heartbeatTimeout,
            int? startingPosition = null,
            int catchupPageSize = 1024,
            int upperQueueBound = 2048,
            int maxLiveQueueSize = 10000,
            bool liveOnly = false)
        {
            _liveProcessingTimer.Elapsed += LiveProcessingTimerOnElapsed;
            _logger = logger;
            _eventsProcessed = 0;
            _startingPosition = startingPosition;
            _lastReceivedEventNumber = startingPosition;
            _dispatcher = dispatcher;
            _streamName = streamName;
            _connection = connection;
            _catchupPageSize = catchupPageSize;
            _maxLiveQueueSize = maxLiveQueueSize;
            _liveOnly = liveOnly;

            // Make heartbeats optional!
            // if (!heartbeatFrequency.HasValue)
            //    heartbeatFrequency = TimeSpan.FromMinutes(1);

            // if (!heartbeatTimeout.HasValue)
            //    heartbeatTimeout = TimeSpan.FromMinutes(2);
            if (heartbeatTimeout != null && heartbeatFrequency != null)
            {
                if (heartbeatFrequency > heartbeatTimeout)
                {
                    throw new ArgumentException(
                        "Heartbeat timeout must be greater than heartbeat frequency",
                        nameof(heartbeatTimeout));
                }

                _heartbeatTimeout = heartbeatTimeout.Value;
                _lastHeartbeat = DateTime.UtcNow;
                _heartbeatTimer = new Timer(heartbeatFrequency.Value.TotalMilliseconds);
                _heartbeatTimer.Elapsed += HeartbeatTimerOnElapsed;
                _usingHeartbeats = true;
            }
            else if (heartbeatTimeout == null && heartbeatFrequency != null)
            {
                throw new ArgumentException(
                    "Heartbeat timeout must be set if heartbeat frequency is set",
                    nameof(heartbeatTimeout));
            }
            else if (heartbeatTimeout != null && heartbeatFrequency == null)
            {
                throw new ArgumentException(
                    "Heartbeat frequency must be set if heartbeat timeout is set",
                    nameof(heartbeatFrequency));
            } // Do not do anything if both are null

            _queue = new BlockingCollection<ResolvedEvent>(upperQueueBound);
        }

        private void HeartbeatTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            SendHeartbeat();

            if (ViewModelsReady)
            {
                return;
            }

            if (_lastHeartbeat < DateTime.UtcNow.Subtract(_heartbeatTimeout))
            {
                _logger.Error(string.Format("Subscriber heartbeat timeout, last heartbeat: {0} restarting subscription", _lastHeartbeat.ToString("G")));
                RestartSubscription();
            }
        }

        private void RestartSubscription()
        {
            if (_usingHeartbeats)
            {
                _heartbeatTimer.Stop();
            }

            lock (_subscriptionLock)
            {
                if (_liveOnly)
                {
                    ((EventStoreSubscription)_subscription).Close();
                }
                else
                {
                    ((EventStoreCatchUpSubscription)_subscription).Stop();
                }

                _subscription = null;
                _catchingUp = true;
                _lastNonLiveEventNumber = int.MinValue;
                _startingPosition = _lastReceivedEventNumber;
                Start(true);
            }
        }

        private void LiveProcessingTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Error("Event Store Subscription has been down for 10 minutes");
        }

        private void ProcessEvents()
        {
            foreach (var item in _queue.GetConsumingEnumerable())
            {
                ProcessEvent(item);
                _lastDispatchedEventNumber = item.OriginalEventNumber;
            }
        }

        private void SubscriptionDropped(
            object eventStoreCatchUpSubscription,
            SubscriptionDropReason subscriptionDropReason,
            Exception ex)
        {
            if (ex != null)
            {
                _logger.Info(ex, "Event Store subscription dropped {0}", subscriptionDropReason.ToString());
            }
            else
            {
                _logger.Info("Event Store subscription dropped {0}", subscriptionDropReason.ToString());
            }

            if (subscriptionDropReason == SubscriptionDropReason.UserInitiated)
            {
                _logger.Info("Not attempting to restart user initiated drop. Subscription is dead.");
                return;
            }// We don't care if we called close,

            if (subscriptionDropReason == SubscriptionDropReason.ConnectionClosed)
            {
                _logger.Info("Not attempting to restart subscription on disposed connection. Subscription is dead.");
                return;
            }

            RestartSubscription();

            lock (_liveProcessingTimer)
            {
                if (!_liveProcessingTimer.Enabled)
                {
                    _liveProcessingTimer.Start();
                }
            }
        }

        private void LiveProcessingStarted(EventStoreCatchUpSubscription eventStoreCatchUpSubscription)
        {
            lock (_liveProcessingTimer)
            {
                _liveProcessingTimer.Stop();
                _catchingUp = false;
            }

            _logger.Info("Live event processing started");
        }

        private async Task EventAppeared(object eventStoreCatchUpSubscription, ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event != null && resolvedEvent.Event.EventType == _heartbeatEventType)
            {
                _lastHeartbeat = DateTime.UtcNow;
                return;
            }

            if (_catchingUp)
            {
                _lastNonLiveEventNumber = resolvedEvent.OriginalEventNumber;
            }

            _queue.Add(resolvedEvent);
            _lastReceivedEventNumber = resolvedEvent.OriginalEventNumber;
        }

        private void ProcessEvent(ResolvedEvent resolvedEvent)
        {
            _eventsProcessed++;
            if (resolvedEvent.Event == null || resolvedEvent.Event.EventType.StartsWith("$"))
            {
                return;
            }

            try
            {
                _dispatcher.Dispatch(resolvedEvent);

                if (_checkpoint == null)
                {
                    return;
                }

                _checkpoint.Write(resolvedEvent.OriginalEventNumber);
                _checkpoint.Flush();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error dispatching event from Event Store subscriber ({0}/{1})", resolvedEvent.Event.EventStreamId, resolvedEvent.Event.EventNumber);
            }
        }
    }
}
