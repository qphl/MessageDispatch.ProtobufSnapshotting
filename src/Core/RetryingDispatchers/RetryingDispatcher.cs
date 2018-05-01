// <copyright file="RetryingDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;
    using System.Threading;

    /// <summary>
    /// Dispatcher that retries on an exception.
    /// </summary>
    /// <typeparam name="TMessage">Message Type</typeparam>
    public abstract class RetryingDispatcher<TMessage> : IDispatcher<TMessage>
    {
        private readonly int? _retryLimit;
        private readonly IDispatcher<TMessage> _dispatcher;
        private readonly Action<string, Exception> _retryLogAction;
        private readonly Func<Exception, RetryDecision> _decide;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryingDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="dispatcher">Dispatcher</param>
        /// <param name="retryLimit">nullable retry limit.</param>
        /// <param name="retryLogAction">Action to perform on every retry.</param>
        /// <param name="decide">Function to decide on if a retry should happen or not.</param>
        protected RetryingDispatcher(IDispatcher<TMessage> dispatcher, int? retryLimit, Action<string, Exception> retryLogAction = null, Func<Exception, RetryDecision> decide = null)
        {
            _retryLimit = retryLimit;
            _retryLogAction = retryLogAction ?? ((_, e) => { });
            _decide = decide ?? (_ => RetryDecision.Retry);
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Retry Decision enum.
        /// </summary>
        public enum RetryDecision
        {
            Fail,
            Retry,
            Ignore,
        }

        /// <inheritdoc />
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
            }
            while (true);
        }

        /// <summary>
        /// Calculates the next retry interval.
        /// </summary>
        /// <param name="attempt">Amount of attempts.</param>
        /// <returns>Next retry interval</returns>
        public abstract TimeSpan RetryInterval(int attempt);
    }
}
