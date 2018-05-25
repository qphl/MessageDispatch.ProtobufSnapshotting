// <copyright file="IMessageHandlerRegistration.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    /// <summary>
    /// Interface for implementing a registry of message types and the methods which can be used to handle those types.
    /// </summary>
    /// <typeparam name="TKey">Key Type</typeparam>
    public interface IMessageHandlerRegistration<in TKey>
    {
        /// <summary>
        /// Registers a message type with its corresponding handler object.
        /// </summary>
        /// <param name="messageType">Message Type</param>
        /// <param name="handler">Handler Object</param>
        void Add(TKey messageType, object handler);

        /// <summary>
        /// Registers a message type with its correpsonding handler object in the form of an <see cref="Action{T}"/>.
        /// </summary>
        /// <param name="messageType">Message Type</param>
        /// <param name="handler">Action to perform with the handler object</param>
        void Add(TKey messageType, Action<object> handler);

        /// <summary>
        /// Clears the entire registry.
        /// </summary>
        void Clear();
    }
}
