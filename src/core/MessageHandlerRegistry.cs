// <copyright file="MessageHandlerRegistry.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A registry of message types and the methods which can be used to handle those types.
    /// </summary>
    /// <typeparam name="TKey">Key Type</typeparam>
    public class MessageHandlerRegistry<TKey> : IMessageHandlerRegistration<TKey>, IMessageHandlerLookup<TKey>
    {
        private Dictionary<TKey, List<object>> _eventHandlers = new Dictionary<TKey, List<object>>();

        /// <inheritdoc />
        public void Add(TKey messageType, object handler)
        {
            var hasHandlerList = _eventHandlers.TryGetValue(messageType, out var handlerList);
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
            var hasHandlerList = _eventHandlers.TryGetValue(messageType, out var handlerList);
            return hasHandlerList ? handlerList : new List<object>();
        }
    }
}
