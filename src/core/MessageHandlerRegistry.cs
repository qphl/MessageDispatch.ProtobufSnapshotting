using System;
using System.Collections.Generic;
using System.Linq;

namespace core
{
    public class MessageHandlerRegistry<TKey> : IMessageHandlerRegistration<TKey>, IMessageHandlerLookup<TKey>
    {
        private Dictionary<TKey, List<object>> _eventHandlers;

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

        public void Clear()
        {
            _eventHandlers = new Dictionary<TKey, List<object>>();
        }

        public List<object> HandlersForMessageType(TKey messageType)
        {
            List<object> handlerList;
            var hasHandlerList = _eventHandlers.TryGetValue(messageType, out handlerList);

            return hasHandlerList ? handlerList : new List<object>();
        }
    }
}