// <copyright file="FixedRetryingDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    /// <summary>
    /// Dispatcher that reties on a fixed time limit.
    /// </summary>
    /// <typeparam name="TMessage">Message Type</typeparam>
    public class FixedRetryingDispatcher<TMessage> : RetryingDispatcher<TMessage>
    {
        private readonly TimeSpan _retryPeriod;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedRetryingDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="dispatcher">Dispatcher</param>
        /// <param name="retryLimit">Nullable retry limit</param>
        /// <param name="retryPeriod">Period of time between retries.</param>
        /// <param name="retryLogAction">Action to perform on every retry.</param>
        /// <param name="decide">Function to decide if the dispatcher retries</param>
        public FixedRetryingDispatcher(IDispatcher<TMessage> dispatcher, int? retryLimit, TimeSpan retryPeriod, Action<string, Exception> retryLogAction = null, Func<Exception, RetryDecision> decide = null)
            : base(dispatcher, retryLimit, retryLogAction, decide)
        {
            _retryPeriod = retryPeriod;
        }

        /// <summary>
        /// Gets the next retry interval.
        /// </summary>
        /// <param name="attempt">Amount of attempts.</param>
        /// <returns>THe retry period passed in the constructor.</returns>
        public override TimeSpan RetryInterval(int attempt)
        {
            return _retryPeriod;
        }
    }
}
