// <copyright file="OnExceptionExitDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    /// <summary>
    /// A wrapping message dispatcher which will kill the application when an exception is thrown by its inner dispatcher.
    /// </summary>
    /// <typeparam name="TMessage">The type of message this dispatcher handles.</typeparam>
    public class OnExceptionExitDispatcher<TMessage> : OnExceptionSkipOrExitDispatcher<TMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnExceptionExitDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="dispatcher">The inner dispatcher that this dispatcher will wrap.</param>
        /// <param name="onExitLog">An action that will be executed after the inner dispatcher throws an exception, before the application exits.
        /// This can be used to log the exception.</param>
        public OnExceptionExitDispatcher(
            IDispatcher<TMessage> dispatcher,
            Action<TMessage, Exception> onExitLog = null)
            : base(dispatcher, (_, e) => true, onExitLog)
        {
        }
    }
}
