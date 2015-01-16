using System.Collections.Generic;

namespace core
{
    public interface IMessageHandlerLookup<in TKey>
    {
        List<object> HandlersForMessageType(TKey messageType);
    }
}