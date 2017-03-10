using System;
using System.Threading;

namespace CR.MessageDispatch.Core
{
    public abstract class RetryingDispatcher<TMessage> : IDispatcher<TMessage>
    {
        private readonly int? _retryLimit;
        private readonly IDispatcher<TMessage> _dispatcher;
        private readonly Action<string,Exception> _retryLogAction;

        protected RetryingDispatcher(IDispatcher<TMessage> dispatcher, int? retryLimit, Action<string, Exception> retryLogAction)
        {
            _retryLimit = retryLimit;
            _retryLogAction = retryLogAction;
            _dispatcher = dispatcher;
        }

        public void Dispatch(TMessage message)
        {
            var attempts = 1;
            do
            {
                try
                {
                    _dispatcher.Dispatch(message);
                    return;
                }
                catch (Exception e)
                {
                    if (_retryLimit.HasValue && attempts > _retryLimit.Value)
                        throw;

                    var retryIn = RetryInterval(attempts);
                    var attemptString = $"Attempt {attempts} of {_retryLimit} failed, retrying in {retryIn}";
                    _retryLogAction(attemptString, e);

                    Thread.Sleep(retryIn);
                    attempts++;
                }
            } while (true);
        }

        public abstract TimeSpan RetryInterval(int attempt);
    }

    public class FixedRetryingDispatcher<TMessage> : RetryingDispatcher<TMessage>
    {
        private readonly TimeSpan _retryPeriod;

        public FixedRetryingDispatcher(IDispatcher<TMessage> dispatcher, int? retryLimit, TimeSpan retryPeriod, Action<string, Exception> retryLogAction) : base(dispatcher, retryLimit, retryLogAction)
        {
            _retryPeriod = retryPeriod;
        }

        public override TimeSpan RetryInterval(int attempt)
        {
            return _retryPeriod;
        }
    }

    public class ExponentialRetryingDispatcher<TMessage> : RetryingDispatcher<TMessage>
    {
        private readonly TimeSpan _retryPeriod;
        private readonly int _exponentialMultiplier;

        public ExponentialRetryingDispatcher(IDispatcher<TMessage> dispatcher, int? retryLimit, TimeSpan retryPeriod, int exponentialMultiplier, Action<string, Exception> retryLogAction) : base(dispatcher, retryLimit, retryLogAction)
        {
            _retryPeriod = retryPeriod;
            _exponentialMultiplier = exponentialMultiplier;
        }

        public override TimeSpan RetryInterval(int attempt)
        {
            return TimeSpan.FromMilliseconds(_retryPeriod.Milliseconds * Math.Pow(attempt, _exponentialMultiplier));
        }
    }
}