// <copyright file="EventStoreJObjectDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Dispatchers.EventStore
{
    using System;
    using System.Text;
    using Core;
    using global::EventStore.ClientAPI;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Dispatcher that deserializes from a JObject.
    /// </summary>
    public class EventStoreJObjectDispatcher : DeserializingMessageDispatcher<ResolvedEvent, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreJObjectDispatcher"/> class.
        /// </summary>
        /// <param name="handlers">Message handler Lookup with a type of string.</param>
        public EventStoreJObjectDispatcher(IMessageHandlerLookup<string> handlers)
            : base(handlers)
        {
        }

        /// <inheritdoc />
        protected override bool TryGetMessageType(ResolvedEvent rawMessage, out string type)
        {
            type = rawMessage.Event.EventType;
            return true;
        }

        /// <summary>
        /// Attempts to deserialize the raw message.
        /// </summary>
        /// <param name="messageType">string of a message type</param>
        /// <param name="rawMessage">Raw Message as a resolved event.</param>
        /// <param name="deserialized">Deserialized object.</param>
        /// <returns>If the deserialization was successful or not.</returns>
        protected override bool TryDeserialize(string messageType, ResolvedEvent rawMessage, out object deserialized)
        {
            deserialized = null;

            try
            {
                deserialized = JObject.Parse(Encoding.UTF8.GetString(rawMessage.Event.Data));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
