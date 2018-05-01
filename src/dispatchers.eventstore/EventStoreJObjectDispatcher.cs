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

    public class EventStoreJObjectDispatcher : DeserializingMessageDispatcher<ResolvedEvent, string>
    {
        public EventStoreJObjectDispatcher(IMessageHandlerLookup<string> handlers)
            : base(handlers)
        {
        }

        protected override bool TryGetMessageType(ResolvedEvent rawMessage, out string type)
        {
            type = rawMessage.Event.EventType;
            return true;
        }

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
