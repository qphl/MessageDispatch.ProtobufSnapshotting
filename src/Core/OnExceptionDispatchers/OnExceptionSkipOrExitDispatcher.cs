// <copyright file="OnExceptionSkipOrExitDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    /// <summary>
    /// A wrapping message dispatcher which will either skip or exit the application when an exception is thrown by its inner dispatcher.
    /// </summary>
    /// <typeparam name="TMessage">The type of message this dispatcher handles.</typeparam>
    public class OnExceptionSkipOrExitDispatcher<TMessage> : IDispatcher<TMessage>
    {
        private readonly IDispatcher<TMessage> _dispatcher;
        private readonly Func<TMessage, Exception, bool> _shouldExit;
        private readonly Action<TMessage, Exception> _onExitLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnExceptionSkipOrExitDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="dispatcher">The inner dispatcher that this dispatcher will wrap.</param>
        /// <param name="shouldExit">The <see cref="Func{TResult}"/> that determines what the application will do after the inner dispatcher throws an exception.
        /// It should return <c>true</c> if the exception is fatal (to instruct the application to exit) and <c>false</c> to skip the current message and continue processing future messages.</param>
        /// <param name="onExitLog">An action that will be executed after the inner dispatcher throws a fatal exception (indicated by <see cref="_shouldExit"/>), before the application exits.
        /// This can be used to log the exception.</param>
        public OnExceptionSkipOrExitDispatcher(
            IDispatcher<TMessage> dispatcher,
            Func<TMessage, Exception, bool> shouldExit,
            Action<TMessage, Exception> onExitLog = null)
        {
            _dispatcher = dispatcher;
            _shouldExit = shouldExit;
            _onExitLog = onExitLog ?? ((_, e) => { });
        }

        /// <inheritdoc />
        public void Dispatch(TMessage message)
        {
            try
            {
                _dispatcher.Dispatch(message);
            }
            catch (Exception e)
            {
                if (_shouldExit(message, e))
                {
                    _onExitLog(message, e);
                    Environment.Exit(1);
                }
            }
        }
    }
}
