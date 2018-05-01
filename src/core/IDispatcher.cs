// <copyright file="IDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    /// <summary>
    /// Interface for the Dispatcher
    /// </summary>
    /// <typeparam name="TMessage">Message type which will be dispatched</typeparam>
    public interface IDispatcher<in TMessage>
    {
        /// <summary>
        /// Dispatching method for dispatching messages.
        /// </summary>
        /// <param name="message">Message to dispatch</param>
        void Dispatch(TMessage message);
    }
}
