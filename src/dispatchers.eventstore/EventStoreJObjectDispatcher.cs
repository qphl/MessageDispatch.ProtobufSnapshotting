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
    /// A message dispatcher that deserializes messages to a JObject upon dispatch.
    /// </summary>
    public class EventStoreJObjectDispatcher : DeserializingMessageDispatcher<ResolvedEvent, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventStoreJObjectDispatcher"/> class.
        /// </summary>
        /// <param name="handlers">Lookups for the handlers which the class can use to process messages.</param>
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

        /// <inheritdoc />
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
