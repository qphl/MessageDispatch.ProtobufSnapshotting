using System;
using System.Collections.Generic;
using System.Reflection;
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
        private readonly JsonSerializerSettings _serializerSettings;

        public EventStoreAggregateEventDispatcher(IMessageHandlerLookup<Type> handlers, JsonSerializerSettings serializerSettings = null) : base(handlers)
        {
            _serializerSettings = serializerSettings ?? new JsonSerializerSettings();
        }

        private Dictionary<String,Type> typeCache = new Dictionary<string, Type>(); 

        protected override bool TryGetMessageType(ResolvedEvent rawMessage, out Type type)
        {
            type = null;

            //optimization: don't even bother trying to deserialize metadata for system events
            if (rawMessage.Event.EventType.StartsWith("$") || rawMessage.Event.Metadata.Length == 0)
                return false;

            try
            {
                IDictionary<string, JToken> metadata = JObject.Parse(Encoding.UTF8.GetString(rawMessage.Event.Metadata));
                
                if (!metadata.ContainsKey("ClrType"))
                    return false;

                var typeString = (string) metadata["ClrType"];

                Type cached = null;
                if (!typeCache.TryGetValue(typeString, out cached))
                {
                    try
                    {
                        cached = Type.GetType((string) metadata["ClrType"], true);
                    }
                    catch (Exception ex)
                    {
                        cached = typeof (TypeNotFound);
                    }

                    typeCache.Add(typeString, cached);
                }

                if (cached.Name.Equals("TypeNotFound"))
                    return false;

                type = cached;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private class TypeNotFound { }

        protected override bool TryDeserialize(Type messageType, ResolvedEvent rawMessage, out object deserialized)
        {
            deserialized = null;

            try
            {
                var jsonString = Encoding.UTF8.GetString(rawMessage.Event.Data);
                deserialized = JsonConvert.DeserializeObject(jsonString, messageType, _serializerSettings);
                return deserialized != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
