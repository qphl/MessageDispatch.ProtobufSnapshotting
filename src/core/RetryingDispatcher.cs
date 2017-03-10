using System;
using System.Threading;

namespace CR.MessageDispatch.Core
{
    public abstract class RetryingDispatcher<TMessage> : IDispatcher<TMessage>
    {
        public enum RetryDecision
        {
            Fail,
            Retry,
            Ignore
        }

        private readonly int? _retryLimit;
        private readonly IDispatcher<TMessage> _dispatcher;
        private readonly Action<string,Exception> _retryLogAction;
        private readonly Func<Exception, RetryDecision> _decide;

        protected RetryingDispatcher(IDispatcher<TMessage> dispatcher, int? retryLimit,
            Action<string, Exception> retryLogAction = null, Func<Exception, RetryDecision> decide = null)
        {
            _retryLimit = retryLimit;
            _retryLogAction = retryLogAction ?? ((_, __) => { });
            _decide = decide ?? (_ => RetryDecision.Retry);
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
                    switch (_decide(e))
                    {
                        case RetryDecision.Fail:
                            throw;
                        case RetryDecision.Ignore:
                            return;
                        case RetryDecision.Retry:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (_retryLimit.HasValue && attempts > _retryLimit.Value)
                    {
                        var limitHitString = $"Retry limit of {_retryLimit} hit.";
                        _retryLogAction(limitHitString, e);
                        throw;
                    }

                    var retryIn = RetryInterval(attempts);
                    var limitString = _retryLimit.HasValue ? $" of {_retryLimit.Value} " : " ";
                    var attemptString = $"Attempt {attempts}{limitString}failed, retrying in {retryIn}";
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

        public FixedRetryingDispatcher(IDispatcher<TMessage> dispatcher, int? retryLimit, TimeSpan retryPeriod, Action<string, Exception> retryLogAction = null, Func<Exception, RetryDecision> decide = null) : base(dispatcher, retryLimit, retryLogAction, decide)
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
        private readonly double _exponentialMultiplier;

        public ExponentialRetryingDispatcher(IDispatcher<TMessage> dispatcher, int? retryLimit, TimeSpan retryPeriod,
            double exponentialMultiplier, Action<string, Exception> retryLogAction = null,
            Func<Exception, RetryDecision> decide = null) : base(dispatcher, retryLimit, retryLogAction, decide)
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