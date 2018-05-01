// <copyright file="OnExceptionExitDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    /// <summary>
    /// Sets up a dispatcher which Exits when an exception is thrown.
    /// </summary>
    /// <typeparam name="TMessage">Message Type</typeparam>
    public class OnExceptionExitDispatcher<TMessage> : OnExceptionSkipOrExitDispatcher<TMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnExceptionExitDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="dispatcher">Dispatcher.</param>
        /// <param name="onExitLog">What to do on exit.</param>
        public OnExceptionExitDispatcher(
            IDispatcher<TMessage> dispatcher,
            Action<TMessage, Exception> onExitLog = null)
            : base(dispatcher, (_, e) => true, onExitLog)
        {
        }
    }
}
