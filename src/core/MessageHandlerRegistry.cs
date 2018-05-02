// <copyright file="MessageHandlerRegistry.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Message Handler Rewgistry implementation.
    /// </summary>
    /// <typeparam name="TKey">Key Type</typeparam>
    public class MessageHandlerRegistry<TKey> : IMessageHandlerRegistration<TKey>, IMessageHandlerLookup<TKey>
    {
        private Dictionary<TKey, List<object>> _eventHandlers = new Dictionary<TKey, List<object>>();

        /// <inheritdoc />
        public void Add(TKey messageType, object handler)
        {
            List<object> handlerList;
            var hasHandlerList = _eventHandlers.TryGetValue(messageType, out handlerList);

            if (!hasHandlerList)
            {
                handlerList = new List<object>();
                _eventHandlers.Add(messageType, handlerList);
            }

            handlerList.Add(handler);
        }

        /// <inheritdoc />
        public void Add(TKey messageType, Action<object> handler)
        {
            Add(messageType, new { Handle = handler });
        }

        /// <inheritdoc />
        public void Clear()
        {
            _eventHandlers = new Dictionary<TKey, List<object>>();
        }

        /// <inheritdoc />
        public List<object> HandlersForMessageType(TKey messageType)
        {
            List<object> handlerList;
            var hasHandlerList = _eventHandlers.TryGetValue(messageType, out handlerList);

            return hasHandlerList ? handlerList : new List<object>();
        }
    }
}
