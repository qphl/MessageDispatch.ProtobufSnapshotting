using System;

namespace core
{
    public class RawMessageDispatcher<TMessage> : DeserializingMessageDispatcher<TMessage, Type>
    {
        public RawMessageDispatcher(IMessageHandlerLookup<Type> handlers) : base(handlers)
        {
        }

        protected override Type GetMessageType(TMessage rawMessage)
        {
            return typeof(TMessage);
        }

        protected override bool TryDeserialize(Type messageType, TMessage rawMessage, out object deserialized)
        {
            deserialized = rawMessage;
            return true;
        }

    }
}