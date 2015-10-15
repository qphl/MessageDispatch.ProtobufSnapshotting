using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Timers;
using CR.MessageDispatch.Core;
using EventStore.ClientAPI;
using Microsoft.Win32.SafeHandles;
using Timer = System.Timers.Timer;

namespace CR.MessageDispatch.Dispatchers.EventStore
{
    public class EventStoreSubscriber
    {
        public class CatchupProgress
        {
            public string StreamName { get; private set; }
            public int EventsProcessed { get; private set; }
            public int StartPosition { get; private set; }
            public int TotalEvents { get; private set; }

            public decimal StreamPercentage
            {
                get { return (((decimal) StartPosition + EventsProcessed)/TotalEvents)*100; }
            }

            public decimal CatchupPercentage
            {
                get { return ((decimal) EventsProcessed/(TotalEvents - StartPosition))*100; }
            }


            public CatchupProgress(int eventsProcessed, int startPosition, string streamName, int totalEvents)
            {
                EventsProcessed = eventsProcessed;
                StartPosition = startPosition;
                StreamName = streamName;
                TotalEvents = totalEvents;
            }

            public override string ToString()
            {
                return String.Format("[{0}] Stream Pos: {1:0.#}% ({2}/{3}), Caught up: {4:0.#}% ({5}/{6})", StreamName,
                    StreamPercentage, EventsProcessed + StartPosition, TotalEvents, CatchupPercentage, EventsProcessed,
                    TotalEvents - StartPosition);
            }
        }

        private bool _viewModelIsReady;
        private IEventStoreConnection _connection;
        private IDispatcher<ResolvedEvent> _dispatcher;

        private int? _startingPosition;
        private EventStoreCatchUpSubscription _subscription;
        private string _streamName;
        private readonly WriteThroughFileCheckpoint _checkpoint;
        private int _catchupPageSize;
        private BlockingCollection<ResolvedEvent> _queue;

        private int _eventsProcessed;

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

        public bool ViewModelsReady
        {
            get { return _viewModelIsReady; }
        }

        private ILogger _logger;

        public EventStoreSubscriber(IEventStoreConnection connection, IDispatcher<ResolvedEvent> dispatcher,
            string streamName, ILogger logger, int? startingPosition = null, int catchUpPageSize = 1024,
            int upperQueueBound = 2048)
        {
            Init(connection, dispatcher, streamName, logger, startingPosition, catchUpPageSize, upperQueueBound);
        }

        public EventStoreSubscriber(IEventStoreConnection connection, IDispatcher<ResolvedEvent> dispatcher,
            ILogger logger,
            string streamName, string checkpointFilePath, int catchupPageSize = 1024, int upperQueueBound = 2048)
        {
            int? startingPosition = null;
            _checkpoint = new WriteThroughFileCheckpoint(checkpointFilePath, "lastProcessedPosition", false, -1);

            var initialCheckpointPosition = _checkpoint.Read();

            if (initialCheckpointPosition != -1)
                startingPosition = (int) initialCheckpointPosition;

            Init(connection, dispatcher, streamName, logger, startingPosition, catchupPageSize, upperQueueBound);
        }

        private void Init(IEventStoreConnection connection, IDispatcher<ResolvedEvent> dispatcher, string streamName,
            ILogger logger, int? startingPosition = null, int catchupPageSize = 1024, int upperQueueBound = 2048)
        {
            _liveProcessingTimer.Elapsed += LiveProcessingTimerOnElapsed;
            _logger = logger;
            _eventsProcessed = 0;
            _startingPosition = startingPosition;
            _dispatcher = dispatcher;
            _streamName = streamName;
            _connection = connection;
            _catchupPageSize = catchupPageSize;
            _queue = new BlockingCollection<ResolvedEvent>(upperQueueBound);
        }

        private void LiveProcessingTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _logger.Error("Event Store Subscription has been down for 10 minutes");
        }

        public void Start()
        {
            _subscription = _connection.SubscribeToStreamFrom(_streamName,
                _startingPosition.HasValue ? (int?) _startingPosition.Value : null, true, EventAppeared,
                LiveProcessingStarted, SubscriptionDropped, readBatchSize: _catchupPageSize);
            Thread processor = new Thread(ProcessEvents);
            processor.Start();
        }

        private void ProcessEvents()
        {
            foreach (var item in _queue.GetConsumingEnumerable())
            {
                ProcessEvent(item);
            }
        }

        private readonly Timer _liveProcessingTimer = new Timer(10*60*1000);

        private void SubscriptionDropped(EventStoreCatchUpSubscription eventStoreCatchUpSubscription,
            SubscriptionDropReason subscriptionDropReason, Exception ex)
        {
            if (ex != null)
            {
                _logger.Info(ex, "Event Store subscription dropped {0}", subscriptionDropReason.ToString());
            }
            else
            {
                _logger.Info("Event Store subscription dropped {0}", subscriptionDropReason.ToString());
            }
            _viewModelIsReady = false;

            lock (_liveProcessingTimer)
            {
                if (!_liveProcessingTimer.Enabled)
                    _liveProcessingTimer.Start();
            }
        }

        private void LiveProcessingStarted(EventStoreCatchUpSubscription eventStoreCatchUpSubscription)
        {
            lock (_liveProcessingTimer)
            {
                _liveProcessingTimer.Stop();
            }

            _logger.Info("Live event processing started");
            _viewModelIsReady = true;
        }

        private void EventAppeared(EventStoreCatchUpSubscription eventStoreCatchUpSubscription,
            ResolvedEvent resolvedEvent)
        {
            _queue.Add(resolvedEvent);
        }

        private void ProcessEvent(ResolvedEvent resolvedEvent)
        {
            _eventsProcessed++;
            if (resolvedEvent.Event == null || resolvedEvent.Event.EventType.StartsWith("$"))
                return;
            try
            {
                _dispatcher.Dispatch(resolvedEvent);

                if (_checkpoint == null) return;

                _checkpoint.Write(resolvedEvent.OriginalEventNumber);
                _checkpoint.Flush();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error dispatching event from Event Store subscriber ({0}/{1})",
                    resolvedEvent.Event.EventStreamId,
                    resolvedEvent.Event.EventNumber);
            }
        }

        public void ShutDown()
        {
            _subscription.Stop(TimeSpan.FromSeconds(1));
        }
    }

    //pinched from Event Store source http://geteventstore.com
    internal class WriteThroughFileCheckpoint
    {
        private static class Filenative
        {
            [DllImport("kernel32", SetLastError = true)]
            internal static extern SafeFileHandle CreateFile(
                string FileName,
                uint DesiredAccess,
                uint ShareMode,
                IntPtr SecurityAttributes,
                uint CreationDisposition,
                int FlagsAndAttributes,
                IntPtr hTemplate
                );

            public const int FILE_FLAG_NO_BUFFERING = 0x20000000;
        }

        private readonly string _filename;
        private readonly string _name;
        private readonly bool _cached;
        private long _last;
        private long _lastFlushed;
        private FileStream _stream;
        private readonly BinaryWriter _writer;
        private readonly BinaryReader _reader;
        private readonly MemoryStream _memStream;
        private readonly byte[] buffer;

        public WriteThroughFileCheckpoint(string filename)
            : this(filename, Guid.NewGuid().ToString(), false)
        {
        }

        public WriteThroughFileCheckpoint(string filename, string name) : this(filename, name, false)
        {
        }

        public WriteThroughFileCheckpoint(string filename, string name, bool cached, long initValue = 0)
        {
            _filename = filename;
            _name = name;
            _cached = cached;
            buffer = new byte[4096];
            _memStream = new MemoryStream(buffer);

            var handle = Filenative.CreateFile(_filename,
                (uint) FileAccess.ReadWrite,
                (uint) FileShare.ReadWrite,
                IntPtr.Zero,
                (uint) FileMode.OpenOrCreate,
                Filenative.FILE_FLAG_NO_BUFFERING | (int) FileOptions.WriteThrough,
                IntPtr.Zero);

            _stream = new FileStream(handle, FileAccess.ReadWrite, 4096);
            var exists = _stream.Length == 4096;
            _stream.SetLength(4096);
            _reader = new BinaryReader(_stream);
            _writer = new BinaryWriter(_memStream);
            if (!exists)
            {
                Write(initValue);
                Flush();
            }
            _last = _lastFlushed = ReadCurrent();
        }


        [DllImport("kernel32.dll")]
        private static extern bool FlushFileBuffers(IntPtr hFile);

        public void Close()
        {
            Flush();
            _stream.Close();
            _stream.Dispose();
        }

        public string Name
        {
            get { return _name; }
        }

        public void Write(long checkpoint)
        {
            Interlocked.Exchange(ref _last, checkpoint);
        }

        public void Flush()
        {
            _memStream.Seek(0, SeekOrigin.Begin);
            _stream.Seek(0, SeekOrigin.Begin);
            var last = Interlocked.Read(ref _last);
            _writer.Write(last);
            _stream.Write(buffer, 0, buffer.Length);

            Interlocked.Exchange(ref _lastFlushed, last);
            //FlushFileBuffers(_file.SafeMemoryMappedFileHandle.DangerousGetHandle());
        }

        public long Read()
        {
            return _cached ? Interlocked.Read(ref _lastFlushed) : ReadCurrent();
        }

        private long ReadCurrent()
        {
            _stream.Seek(0, SeekOrigin.Begin);
            return _reader.ReadInt64();
        }

        public long ReadNonFlushed()
        {
            return Interlocked.Read(ref _last);
        }

        public void Dispose()
        {
            Close();
        }
    }
}