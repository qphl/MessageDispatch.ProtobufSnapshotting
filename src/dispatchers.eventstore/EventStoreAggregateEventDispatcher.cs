// <copyright file="EventStoreAggregateEventDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Dispatchers.EventStore
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Core;
    using global::EventStore.ClientAPI;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Deserializing dispatcher for events produced by CR.AggregateRepository
    /// </summary>
    public class EventStoreAggregateEventDispatcher : DeserializingMessageDispatcher<ResolvedEvent, Type>
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();

        public EventStoreAggregateEventDispatcher(IMessageHandlerLookup<Type> handlers, JsonSerializerSettings serializerSettings = null)
            : base(handlers)
        {
            _serializerSettings = serializerSettings ?? new JsonSerializerSettings();
        }

        protected override bool TryGetMessageType(ResolvedEvent rawMessage, out Type type)
        {
            type = null;

            // optimization: don't even bother trying to deserialize metadata for system events
            if (rawMessage.Event.EventType.StartsWith("$") || rawMessage.Event.Metadata.Length == 0)
            {
                return false;
            }

            try
            {
                IDictionary<string, JToken> metadata = JObject.Parse(Encoding.UTF8.GetString(rawMessage.Event.Metadata));

                if (!metadata.ContainsKey("ClrType"))
                {
                    return false;
                }

                var typeString = (string)metadata["ClrType"];

                Type cached = null;
                if (!_typeCache.TryGetValue(typeString, out cached))
                {
                    try
                    {
                        cached = Type.GetType((string)metadata["ClrType"], true);
                    }
                    catch (Exception)
                    {
                        cached = typeof(TypeNotFound);
                    }

                    _typeCache.Add(typeString, cached);
                }

                if (cached.Name.Equals("TypeNotFound"))
                {
                    return false;
                }

                type = cached;
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
                deserialized = JsonConvert.DeserializeObject(jsonString, messageType, _serializerSettings);
                return deserialized != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private class TypeNotFound
        {
        }
    }
}
