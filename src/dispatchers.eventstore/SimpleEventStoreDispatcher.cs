// <copyright file="SimpleEventStoreDispatcher.cs" company="Cognisant">
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

    /// <summary>
    /// A simple event store dispatcher.
    /// </summary>
    public class SimpleEventStoreDispatcher : DeserializingMessageDispatcher<ResolvedEvent, Type>
    {
        private readonly Dictionary<string, Type> _eventTypeMapping;
        private readonly JsonSerializerSettings _serializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleEventStoreDispatcher"/> class.
        /// </summary>
        /// <param name="handlers">Message handler lookup of a type.</param>
        /// <param name="eventTypeMapping">Event Type Map</param>
        /// <param name="serializerSettings">Json Serializer settings.</param>
        public SimpleEventStoreDispatcher(IMessageHandlerLookup<Type> handlers, Dictionary<string, Type> eventTypeMapping, JsonSerializerSettings serializerSettings = null)
            : base(handlers)
        {
            _eventTypeMapping = eventTypeMapping;
            _serializerSettings = serializerSettings ?? new JsonSerializerSettings();
        }

        /// <inheritdoc />
        protected override bool TryGetMessageType(ResolvedEvent rawMessage, out Type type)
        {
            var eventType = rawMessage.Event.EventType;
            return _eventTypeMapping.TryGetValue(eventType, out type);
        }

        /// <inheritdoc />
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
