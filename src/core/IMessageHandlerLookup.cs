// <copyright file="IMessageHandlerLookup.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for implementing the Message Handler Lookups
    /// </summary>
    /// <typeparam name="TKey">Key types.</typeparam>
    public interface IMessageHandlerLookup<in TKey>
    {
        /// <summary>
        /// Function to get the Handlers based on a message type.
        /// </summary>
        /// <param name="messageType">Message Type we are getting the handlers of.</param>
        /// <returns>A list of handlers.</returns>
        List<object> HandlersForMessageType(TKey messageType);
    }
}
