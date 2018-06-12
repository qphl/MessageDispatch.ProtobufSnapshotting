// <copyright file="ExponentialRetryingDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    /// <summary>
    /// A wrapping message dispatcher that will allow its inner dispatcher to continue retrying to processing a message at an exponentially decreasing frequency until the number of retries equals an assigned (nullable) limit.
    /// </summary>
    /// <typeparam name="TMessage">The type of message this dispatcher handles.</typeparam>
    public class ExponentialRetryingDispatcher<TMessage> : RetryingDispatcher<TMessage>
    {
        private readonly TimeSpan _retryPeriod;
        private readonly double _exponentialMultiplier;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialRetryingDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="dispatcher">The inner dispatcher that this dispatcher will wrap.</param>
        /// <param name="retryLimit">Represents a number of message processing attempts.
        /// After the inner dispatcher has attempted to process a message this many times, it will stop retrying to process that message.
        /// This can be set as null to enable infinite retries.</param>
        /// <param name="retryPeriod">The initial period of time for which the dispatcher should wait before retrying to process a message.</param>
        /// <param name="exponentialMultiplier">The value with which the <see cref="RetryInterval"/> is multiplied by each time an attempt to process the current message fails.</param>
        /// <param name="retryLogAction">An action the dispatcher should perform on every retry.
        /// This can be used to log that a retry is being attempted.</param>
        /// <param name="decide">The <see cref="Func{TResult}"/> that determines if retries should continue to be attempted.
        /// This should return <c>true</c> to indicate that the dispatcher should continue attempting to process the message, and <c>false</c> to indicate that the dispatcher should stop attempting to process the message.</param>
        public ExponentialRetryingDispatcher(IDispatcher<TMessage> dispatcher, int? retryLimit, TimeSpan retryPeriod, double exponentialMultiplier, Action<string, Exception> retryLogAction = null, Func<Exception, RetryDecision> decide = null)
            : base(dispatcher, retryLimit, retryLogAction, decide)
        {
            _retryPeriod = retryPeriod;
            _exponentialMultiplier = exponentialMultiplier;
        }

        /// <summary>
        /// The method used after every message processing attmept to recalculate the retry interval (the time for which a dispatcher will wait before it retries to process a message).
        /// </summary>
        /// <param name="attempt">Represents the number of times the dispatcher has already attempted to process a message.</param>
        /// <returns>The amount of time the dispatcher should wait before retrying to process a message.</returns>
        public override TimeSpan RetryInterval(int attempt)
        {
            return TimeSpan.FromMilliseconds(_retryPeriod.Milliseconds * Math.Pow(attempt, _exponentialMultiplier));
        }
    }
}
