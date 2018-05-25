// <copyright file="MultiplexingDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    /// <summary>
    /// A message dispatcher consisting of several other message dispatchers.
    /// This dispatcher will attempt to process every message through each of its constituent dispatchers.
    /// </summary>
    /// <typeparam name="TMessage">Message Type</typeparam>
    public class MultiplexingDispatcher<TMessage> : IDispatcher<TMessage>
    {
        private readonly IDispatcher<TMessage>[] _dispatchers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplexingDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="dispatchers">The dispatchers to multiplex.</param>
        public MultiplexingDispatcher(params IDispatcher<TMessage>[] dispatchers)
        {
            _dispatchers = dispatchers;
        }

        /// <inheritdoc />
        public void Dispatch(TMessage message)
        {
            foreach (var dispatcher in _dispatchers)
            {
                dispatcher.Dispatch(message);
            }
        }
    }
}
