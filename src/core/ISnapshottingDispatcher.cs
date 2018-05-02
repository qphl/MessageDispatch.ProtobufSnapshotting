// <copyright file="ISnapshottingDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for implementing a Snapshotting dispatcher.
    /// </summary>
    /// <typeparam name="TMessage">Message Type</typeparam>
    public interface ISnapshottingDispatcher<TMessage> : IDispatcher<TMessage>
    {
        /// <summary>
        /// Gets or sets the Inner Dispatcher.
        /// </summary>
        IDispatcher<TMessage> InnerDispatcher { get; set; }

        /// <summary>
        /// Loads the latest checkout.
        /// </summary>
        /// <returns>Null or a checkpoint number.</returns>
        int? LoadCheckpoint();

        /// <summary>
        /// Loads objects form the snapshot files.
        /// </summary>
        /// <returns>An enumerable list of objects.</returns>
        IEnumerable<object> LoadObjects();
    }
}
