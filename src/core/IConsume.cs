// <copyright file="IConsume.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    /// <summary>
    /// Consumer which takes in and handles messages.
    /// </summary>
    /// <typeparam name="TMessage">Message which will be consumed.</typeparam>
    public interface IConsume<in TMessage>
    {
        /// <summary>
        /// Handle method for processing messages.
        /// </summary>
        /// <param name="message">Message which will be handled.</param>
        void Handle(TMessage message);
    }
}
