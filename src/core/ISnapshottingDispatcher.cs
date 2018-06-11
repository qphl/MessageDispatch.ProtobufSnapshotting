// <copyright file="ISnapshottingDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface for implementing a wrapping message dispatcher that can take and load snapshots of application states.
    /// </summary>
    /// <typeparam name="TMessage">Message Type</typeparam>
    public interface ISnapshottingDispatcher<TMessage> : IDispatcher<TMessage>
    {
        /// <summary>
        /// Gets or sets the inner dispatcher which the implementation of this interface will wrap.
        /// </summary>
        IDispatcher<TMessage> InnerDispatcher { get; set; }

        /// <summary>
        /// A method used to load the position of the most recent snapshot of the inner dispatcher.
        /// </summary>
        /// <returns>Null, or an integer representing the current snapshot checkpoint.</returns>
        int? LoadCheckpoint();

        /// <summary>
        /// A method used to load any objects from the snapshot files.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects.</returns>
        IEnumerable<object> LoadObjects();
    }
}
