using System;
using System.Collections.Generic;
using System.Text;
using CR.MessageDispatch.Core;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CR.MessageDispatch.Dispatchers.EventStore
{
    /// <summary>
    /// Deserializing dispatcher for events produced by CR.AggregateRepository
    /// </summary>
    public class EventStoreAggregateEventDispatcher : DeserializingMessageDispatcher<ResolvedEvent,Type>
    {
        public EventStoreAggregateEventDispatcher(IMessageHandlerLookup<Type> handlers) : base(handlers)
        {
        }

        protected override bool TryGetMessageType(ResolvedEvent rawMessage, out Type type)
        {
            type = null;

            //optimization: don't even bother trying to deserialize metadata for system events
            if (rawMessage.Event.EventType.StartsWith("$"))
                return false;

            try
            {
                IDictionary<string, JToken> metadata = JObject.Parse(Encoding.UTF8.GetString(rawMessage.Event.Metadata));
                type = Type.GetType((string)metadata["ClrType"], true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
