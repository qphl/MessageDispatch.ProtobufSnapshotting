using System;
using System.Collections.Generic;
using System.Text;
using CR.MessageDispatch.Core;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace CR.MessageDispatch.Dispatchers.EventStore
{
    public class SimpleEventStoreDispatcher : DeserializingMessageDispatcher<ResolvedEvent, Type>
    {
        private readonly Dictionary<String, Type> _eventTypeMapping;

        public SimpleEventStoreDispatcher(IMessageHandlerLookup<Type> handlers, Dictionary<string, Type> eventTypeMapping) : base(handlers)
        {
            _eventTypeMapping = eventTypeMapping;
        }

        protected override bool TryGetMessageType(ResolvedEvent rawMessage, out Type type)
        {
            var eventType = rawMessage.Event.EventType;
            return _eventTypeMapping.TryGetValue(eventType, out type);
        }

        protected override bool TryDeserialize(Type messageType, ResolvedEvent rawMessage, out object deserialized)
        {
            deserialized = null;

            try
            {
                var jsonString = Encoding.UTF8.GetString(rawMessage.Event.Data);
                deserialized = JsonConvert.DeserializeObject(jsonString, messageType);
                return deserialized != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}