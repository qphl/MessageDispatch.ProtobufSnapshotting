// <copyright file="ExponentialRetryingDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    /// <summary>
    /// Sets up a dispatcher that exponentially retries.
    /// </summary>
    /// <typeparam name="TMessage">Message Type</typeparam>
    public class ExponentialRetryingDispatcher<TMessage> : RetryingDispatcher<TMessage>
    {
        private readonly TimeSpan _retryPeriod;
        private readonly double _exponentialMultiplier;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentialRetryingDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="dispatcher">Dispatcher.</param>
        /// <param name="retryLimit">Nullable retry limit.</param>
        /// <param name="retryPeriod">Initial period of time to retry.</param>
        /// <param name="exponentialMultiplier">How much to multiply the previous time span.</param>
        /// <param name="retryLogAction">Action to perform every retry.</param>
        /// <param name="decide">Function to decide if we retry or not.</param>
        public ExponentialRetryingDispatcher(IDispatcher<TMessage> dispatcher, int? retryLimit, TimeSpan retryPeriod, double exponentialMultiplier, Action<string, Exception> retryLogAction = null, Func<Exception, RetryDecision> decide = null)
            : base(dispatcher, retryLimit, retryLogAction, decide)
        {
            _retryPeriod = retryPeriod;
            _exponentialMultiplier = exponentialMultiplier;
        }

        /// <summary>
        /// Generates the retry interval based on the number of attempts.
        /// </summary>
        /// <param name="attempt">Number of attempts</param>
        /// <returns>The amount of time for the next retry.</returns>
        public override TimeSpan RetryInterval(int attempt)
        {
            return TimeSpan.FromMilliseconds(_retryPeriod.Milliseconds * Math.Pow(attempt, _exponentialMultiplier));
        }
    }
}
