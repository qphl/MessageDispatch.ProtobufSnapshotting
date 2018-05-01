// <copyright file="ErrorHandlingDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    /// <summary>
    /// A dispatcher to call an action/function on an error and rethrow the exception if required.
    /// </summary>
    /// <typeparam name="TMessage">The message type to be dispatched.</typeparam>
    public class ErrorHandlingDispatcher<TMessage> : IDispatcher<TMessage>
    {
        private readonly IDispatcher<TMessage> _dispatcher;
        private readonly Func<TMessage, Exception, bool> _onError;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandlingDispatcher{TMessage}"/> class.
        /// Constructor for setting a function to determine if the exception should be rethrown.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to wrap.</param>
        /// <param name="onError">The function to call when an exception is thrown
        /// Should return if the dispatcher should rethrow the exception.</param>
        public ErrorHandlingDispatcher(IDispatcher<TMessage> dispatcher, Func<TMessage, Exception, bool> onError)
        {
            _dispatcher = dispatcher;
            _onError = onError;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandlingDispatcher{TMessage}"/> class.
        /// Constructor for always setting if the exception should be rethrown after the onError action.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to wrap.</param>
        /// <param name="onError">The action to call when an exception is thrown</param>
        /// <param name="shouldRethrow">If the exception should be rethrown after the onError action is called</param>
        public ErrorHandlingDispatcher(IDispatcher<TMessage> dispatcher, Action<TMessage, Exception> onError, bool shouldRethrow)
        {
            _dispatcher = dispatcher;
            _onError = (message, exception) =>
            {
                onError(message, exception);
                return shouldRethrow;
            };
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
                var shouldRethrow = _onError(message, e);
                if (shouldRethrow)
                {
                    throw;
                }
            }
        }
    }
}
