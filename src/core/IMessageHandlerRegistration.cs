// <copyright file="IMessageHandlerRegistration.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    /// <summary>
    /// Interface for Message Handler Registration.
    /// </summary>
    /// <typeparam name="TKey">Key Type</typeparam>
    public interface IMessageHandlerRegistration<in TKey>
    {
        /// <summary>
        /// Registers a message type along with the repective handler object.
        /// </summary>
        /// <param name="messageType">Message Type</param>
        /// <param name="handler">Handler Object</param>
        void Add(TKey messageType, object handler);

        /// <summary>
        /// Registers a message type along with the repective handler object.
        /// </summary>
        /// <param name="messageType">Message Type</param>
        /// <param name="handler">Action to perform with the handler object</param>
        void Add(TKey messageType, Action<object> handler);

        /// <summary>
        /// Clears the Handler Registrations.
        /// </summary>
        void Clear();
    }
}
