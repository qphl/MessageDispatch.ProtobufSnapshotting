// <copyright file="FixedRetryingDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    /// <summary>
    /// A wrapping message dispatcher whose inner dispatcher will keep making attempts to process a message at a constant frequency, until the number of retries equals an assigned (nullable) limit.
    /// </summary>
    /// <typeparam name="TMessage">Message Type</typeparam>
    public class FixedRetryingDispatcher<TMessage> : RetryingDispatcher<TMessage>
    {
        private readonly TimeSpan _retryPeriod;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedRetryingDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="dispatcher">The inner dispatcher that this dispatcher will wrap.</param>
        /// <param name="retryLimit">Represents a number of message processing attempts.
        /// After the inner dispatcher has attempted to process a message this many times, it will stop retrying to process that message.
        /// This can be set as null to enable infinite retries.</param>
        /// <param name="retryPeriod">The period of time the inner dispatcher will wait before retrying to process a message after a failed attempt.</param>
        /// <param name="retryLogAction">An action the dispatcher should perform on every retry.
        /// This can be used to log that a retry is being attempted.</param>
        /// <param name="decide">The <see cref="Func{TResult}"/> that determines if retries should continue to be attempted.
        /// This should return <c>true</c> to indicate that the dispatcher should continue attempting to process the message, and <c>false</c> to indicate that the dispatcher should stop attempting to process the message.</param>
        public FixedRetryingDispatcher(IDispatcher<TMessage> dispatcher, int? retryLimit, TimeSpan retryPeriod, Action<string, Exception> retryLogAction = null, Func<Exception, RetryDecision> decide = null)
            : base(dispatcher, retryLimit, retryLogAction, decide)
        {
            _retryPeriod = retryPeriod;
        }

        /// <inheritdoc />
        public override TimeSpan RetryInterval(int attempt)
        {
            return _retryPeriod;
        }
    }
}
