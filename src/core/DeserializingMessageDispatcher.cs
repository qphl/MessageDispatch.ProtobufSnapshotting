using System.Linq;

namespace core
{
    public abstract class DeserializingMessageDispatcher<TRaw, TLookupKey> : IDispatcher<TRaw>
    {
        public delegate bool Deserializer(TRaw rawMessage, out object deserialized);

        protected IMessageHandlerLookup<TLookupKey> Handlers;

        protected abstract TLookupKey GetMessageType(TRaw rawMessage);
        protected abstract bool TryDeserialize(TLookupKey messageType, TRaw rawMessage, out object deserialized);

        protected DeserializingMessageDispatcher(IMessageHandlerLookup<TLookupKey> handlers)
        {
            Handlers = handlers;
        }

        public void Dispatch(TRaw message)
        {
            object deserialized;
            var type = GetMessageType(message);
            var handlers = Handlers.HandlersForMessageType(type);
            
            if (!handlers.Any()) return;

            if (!TryDeserialize(type, message, out deserialized)) return;

            foreach (var handler in handlers)
            {
                ((dynamic) handler).handle((dynamic) message);
            }
        }
    }
}