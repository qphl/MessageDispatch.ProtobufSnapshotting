// <copyright file="DeserializingMessageDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System.Linq;

    /// <summary>
    /// <inheritdoc />
    /// A message dispatcher which deserializes messages upon dispatch.
    /// </summary>
    /// <typeparam name="TRaw">The type of raw messages the disaptcher can handle.</typeparam>
    /// <typeparam name="TLookupKey">The type of message handler lookup keys the dispatcher can handle.</typeparam>
    public abstract class DeserializingMessageDispatcher<TRaw, TLookupKey> : IDispatcher<TRaw>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializingMessageDispatcher{TRaw, TLookupKey}"/> class.
        /// </summary>
        /// <param name="handlers">The handler methods for processing messages with.</param>
        protected DeserializingMessageDispatcher(IMessageHandlerLookup<TLookupKey> handlers)
        {
            Handlers = handlers;
        }

        /// <summary>
        /// Deserializes raw messages.
        /// </summary>
        /// <param name="rawMessage">Represents a raw message that should be deserialized.</param>
        /// <param name="deserialized">An object with which the deserialized messaged is outputted.</param>
        /// <returns>A boolean value indicating if the message was succesfully deserialized.
        /// This will equal <c>true</c> if the message was succesfully deserialized.</returns>
        public delegate bool Deserializer(TRaw rawMessage, out object deserialized);

        /// <summary>
        /// Gets the IMessageHandlerLookups that were set on initialisation.
        /// </summary>
        protected IMessageHandlerLookup<TLookupKey> Handlers { get; }

        /// <inheritdoc />
        public virtual void Dispatch(TRaw message)
        {
            if (!TryGetMessageType(message, out var type))
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
        /// Gets the type of a raw message.
        /// </summary>
        /// <param name="rawMessage">The raw message which the method should return the type of.</param>
        /// <param name="type">An object which can represent the raw messages's type.</param>
        /// <returns>A boolean value indicating if the message's type was successfully determined.
        /// This value will be <c>true</c> if the message's type was succesfully determined.</returns>
        protected abstract bool TryGetMessageType(TRaw rawMessage, out TLookupKey type);

        /// <summary>
        /// Deserializes raw messages.
        /// </summary>
        /// <param name="messageType">The type of the message that should be deserialized.</param>
        /// <param name="rawMessage">The raw message that should be deserialized.</param>
        /// <param name="deserialized">An object with which the deserialized message can be outputted.</param>
        /// <returns>A boolean value indicating if the message was successfully deserialized.
        /// This value will be <c>true</c> if the raw message was successfully deserialized.</returns>
        protected abstract bool TryDeserialize(TLookupKey messageType, TRaw rawMessage, out object deserialized);
    }
}
