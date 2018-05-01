// <copyright file="DeserializingMessageDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System.Linq;

    /// <summary>
    /// <inheritdoc />
    /// Specifically deserializes the message upon dispatch.
    /// </summary>
    /// <typeparam name="TRaw"></typeparam>
    /// <typeparam name="TLookupKey"></typeparam>
    public abstract class DeserializingMessageDispatcher<TRaw, TLookupKey> : IDispatcher<TRaw>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializingMessageDispatcher{TRaw, TLookupKey}"/> class.
        /// </summary>
        /// <param name="handlers">Message handler lookups.</param>
        protected DeserializingMessageDispatcher(IMessageHandlerLookup<TLookupKey> handlers)
        {
            Handlers = handlers;
        }

        /// <summary>
        /// Deserializes the raw message.
        /// </summary>
        /// <param name="rawMessage">Raw Message to be deserialized</param>
        /// <param name="deserialized">Deserialized output.</param>
        /// <returns>If the deserialization was successful or not.</returns>
        public delegate bool Deserializer(TRaw rawMessage, out object deserialized);

        protected IMessageHandlerLookup<TLookupKey> Handlers { get; }

        /// <inheritdoc />
        public virtual void Dispatch(TRaw message)
        {
            var type = default(TLookupKey);

            if (!TryGetMessageType(message, out type))
            {
                return;
            }

            var handlers = Handlers.HandlersForMessageType(type);

            if (!handlers.Any())
            {
                return;
            }

            if (!TryDeserialize(type, message, out var deserialized))
            {
                return;
            }

            foreach (var handler in handlers)
            {
                ((dynamic)handler).Handle((dynamic)deserialized);
            }
        }

        /// <summary>
        /// Attempts to get type of the raw message.
        /// </summary>
        /// <param name="rawMessage">Raw Message</param>
        /// <param name="type">Output of the type.</param>
        /// <returns>If obtaining the type was successful or not.</returns>
        protected abstract bool TryGetMessageType(TRaw rawMessage, out TLookupKey type);

        /// <summary>
        /// Attempts to deserialize the raw message.
        /// </summary>
        /// <param name="messageType">Message Type</param>
        /// <param name="rawMessage">Raw Message</param>
        /// <param name="deserialized">Deserialized object.</param>
        /// <returns>If the deserialization was successful or not.</returns>
        protected abstract bool TryDeserialize(TLookupKey messageType, TRaw rawMessage, out object deserialized);
    }
}
