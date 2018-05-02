// <copyright file="RawMessageDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    /// <summary>
    /// A Deserializing message handler with no actual deserialization, just passes things through
    /// </summary>
    /// <typeparam name="TMessage">Message Type</typeparam>
    public class RawMessageDispatcher<TMessage> : DeserializingMessageDispatcher<TMessage, Type>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawMessageDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="handlers">Message handler lookup of a type</param>
        public RawMessageDispatcher(IMessageHandlerLookup<Type> handlers)
            : base(handlers)
        {
        }

        /// <inheritdoc />
        protected override bool TryGetMessageType(TMessage rawMessage, out Type type)
        {
            type = rawMessage.GetType();
            return true;
        }

        /// <inheritdoc />
        protected override bool TryDeserialize(Type messageType, TMessage rawMessage, out object deserialized)
        {
            deserialized = rawMessage;
            return true;
        }
    }
}
