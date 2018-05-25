// <copyright file="IDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    /// <summary>
    /// A common interface for a message dispatcher.
    /// </summary>
    /// <typeparam name="TMessage">The type of message this dispatcher handles.</typeparam>
    public interface IDispatcher<in TMessage>
    {
        /// <summary>
        /// The method which controls dispatching messages.
        /// </summary>
        /// <param name="message">Represents a message for the dipatcher to attempt to process.</param>
        void Dispatch(TMessage message);
    }
}
