// <copyright file="OnExceptionSkipOrExitDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    /// <summary>
    /// Sets up a dispatcher which will either skip or exit when an exception is thrown.
    /// </summary>
    /// <typeparam name="TMessage">Message Type</typeparam>
    public class OnExceptionSkipOrExitDispatcher<TMessage> : IDispatcher<TMessage>
    {
        private readonly IDispatcher<TMessage> _dispatcher;
        private readonly Func<TMessage, Exception, bool> _shouldExit;
        private readonly Action<TMessage, Exception> _onExitLog;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnExceptionSkipOrExitDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="dispatcher">Dispatcher to use.</param>
        /// <param name="shouldExit">Should the dispatcher exit or skip.</param>
        /// <param name="onExitLog">What should happen on exit.</param>
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
