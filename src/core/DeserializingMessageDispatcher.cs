using System.Linq;

namespace CR.MessageDispatch.Core
{
    public abstract class DeserializingMessageDispatcher<TRaw, TLookupKey> : IDispatcher<TRaw>
    {
        public delegate bool Deserializer(TRaw rawMessage, out object deserialized);

        protected IMessageHandlerLookup<TLookupKey> Handlers;

        protected abstract bool TryGetMessageType(TRaw rawMessage, out TLookupKey type);
        protected abstract bool TryDeserialize(TLookupKey messageType, TRaw rawMessage, out object deserialized);

        protected DeserializingMessageDispatcher(IMessageHandlerLookup<TLookupKey> handlers)
        {
            Handlers = handlers;
        }

        public void Dispatch(TRaw message)
        {
            object deserialized;
            var type = default(TLookupKey);

            if (!TryGetMessageType(message, out type)) return;

            var handlers = Handlers.HandlersForMessageType(type);
            
            if (!handlers.Any()) return;

            if (!TryDeserialize(type, message, out deserialized)) return;

            foreach (var handler in handlers)
            {
                ((dynamic) handler).Handle((dynamic) deserialized);
            }
        }
    }
}