using System;

namespace core
{
    public interface IMessageHandlerRegistration<in TKey>
    {
        void Add(TKey messageType, object handler);
        void Clear();
    }
}