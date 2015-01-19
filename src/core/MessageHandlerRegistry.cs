using System;
using System.Collections.Generic;
using System.Linq;

namespace CR.MessageDispatch.Core
{
    public class MessageHandlerRegistry<TKey> : IMessageHandlerRegistration<TKey>, IMessageHandlerLookup<TKey>
    {
        private Dictionary<TKey, List<object>> _eventHandlers = new Dictionary<TKey,List<object>>();

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

        public void Add(TKey messageType, Action<object> handler)
        {
            Add(messageType, new { Handle = handler });
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

    public static class MessageHandlerRegistryExtensions
    {
        /// <summary>
        /// Adds based on all the IConsume interfaces that this object implements
        /// </summary>
        public static void AddByConvention(this MessageHandlerRegistry<Type> registry, object handler)
        {
            var handlerType = handler.GetType();

            var handlerInterfaces = handlerType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsume<>)).ToArray();

            foreach (var messageType in handlerInterfaces.Select(i => i.GetGenericArguments()[0]))
            {
                registry.Add(messageType, handler);
            }
        }
    }
}