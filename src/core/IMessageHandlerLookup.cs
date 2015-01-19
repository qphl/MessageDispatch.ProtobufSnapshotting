using System.Collections.Generic;

namespace CR.MessageDispatch.Core
{
    public interface IMessageHandlerLookup<in TKey>
    {
        List<object> HandlersForMessageType(TKey messageType);
    }
}