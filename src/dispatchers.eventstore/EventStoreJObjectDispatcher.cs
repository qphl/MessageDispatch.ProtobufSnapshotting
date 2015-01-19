using System;
using System.Text;
using CR.MessageDispatch.Core;
using EventStore.ClientAPI;
using Newtonsoft.Json.Linq;

namespace CR.MessageDispatch.Dispatchers.EventStore
{
    public class EventStoreJObjectDispatcher : DeserializingMessageDispatcher<ResolvedEvent, string>
    {
        public EventStoreJObjectDispatcher(IMessageHandlerLookup<string> handlers) : base(handlers)
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
                if (!rawMessage.Event.IsJson)
                    return false;

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