using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using core;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dispatchers.eventstore
{
    public class EventStoreAggregateEventDispatcher : DeserializingMessageDispatcher<ResolvedEvent,Type>
    {
        public EventStoreAggregateEventDispatcher(IMessageHandlerLookup<Type> handlers) : base(handlers)
        {
        }

        protected override bool TryGetMessageType(ResolvedEvent rawMessage, out Type type)
        {
            type = null;

            if (rawMessage.Event.EventType.StartsWith("$"))
                return false;

            IDictionary<string, JToken> metadata;
            try
            {
                metadata = JObject.Parse(Encoding.UTF8.GetString(rawMessage.Event.Metadata));
            }
            catch (JsonReaderException)
            {
                return false;
            }

            if (!metadata.ContainsKey("ClrType"))
                return false;

            try
            {
                type = Type.GetType((string)metadata["ClrType"], true);
            }
            catch (Exception ex) //TODO be more specific here
            {
                return false;
            }

            return true;
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
            catch (JsonReaderException ex)
            {
                return false;
            }
        }
    }
}
