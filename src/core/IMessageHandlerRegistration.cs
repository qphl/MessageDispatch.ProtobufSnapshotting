using System;

namespace core
{
    public interface IMessageHandlerRegistration<in TKey>
    {
        void Add(TKey messageType, object handler);
        void Add(TKey messageType, Action<object> handler);
        void Clear();
    }
}