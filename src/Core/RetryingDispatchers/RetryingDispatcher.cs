// <copyright file="RetryingDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;
    using System.Threading;

    /// <summary>
    /// A wrapping message dispatcher that will allow its inner dispatcher to continue retrying to processing a message until the number of retries equals an assigned (nullable) limit.
    /// </summary>
    /// <typeparam name="TMessage">The type of message this dispatcher handles.</typeparam>
    public abstract class RetryingDispatcher<TMessage> : IDispatcher<TMessage>
    {
        private readonly int? _retryLimit;
        private readonly IDispatcher<TMessage> _dispatcher;
        private readonly Action<string, Exception> _retryLogAction;
        private readonly Func<Exception, RetryDecision> _decide;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryingDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="dispatcher">The inner dispatcher that this dispatcher will wrap.</param>
        /// <param name="retryLimit">Represents a number of message processing attempts.
        /// After the inner dispatcher has attempted to process a message this many times, it will stop retrying to process that message.
        /// This can be set as null to enable infinite retries.</param>
        /// <param name="retryLogAction">An action the dispatcher should perform on every retry.
        /// This can be used to log that a retry is being attempted.</param>
        /// <param name="decide">The <see cref="Func{TResult}"/> that determines if retries should continue to be attempted.
        /// This should return <c>true</c> to indicate that the dispatcher should continue attempting to process the message, and <c>false</c> to indicate that the dispatcher should stop attempting to process the message.</param>
        protected RetryingDispatcher(IDispatcher<TMessage> dispatcher, int? retryLimit, Action<string, Exception> retryLogAction = null, Func<Exception, RetryDecision> decide = null)
        {
            _retryLimit = retryLimit;
            _retryLogAction = retryLogAction ?? ((_, e) => { });
            _decide = decide ?? (_ => RetryDecision.Retry);
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Represents the next action a dispatcher should take after it has failed to process a message.
        /// </summary>
        /// <remarks>
        /// When the value is <c>Fail</c>, the dispatcher will throw an exception.
        /// When the value is <c>Retry</c>, the dispatcher will continue trying to process the message.
        /// When the value is <c>Ignore</c>, the dispatcher will skip the current message and continue to process future messages.
        /// </remarks>
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
        /// The method used to calculate a message dispatcher's retry interval (the time for which a dispatcher will wait before it retries to process a message).
        /// </summary>
        /// <param name="attempt">The number of previous attempts to process the message.</param>
        /// <returns>The time period the dispatcher should wait before retrying to process the message.</returns>
        public abstract TimeSpan RetryInterval(int attempt);
    }
}
