// <copyright file="RawMessageDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    /// <summary>
    /// A message dispatcher which dispatches messages in the raw form in which they were passed to it. It does not perform any deserialization.
    /// </summary>
    /// <typeparam name="TMessage">Message Type</typeparam>
    public class RawMessageDispatcher<TMessage> : DeserializingMessageDispatcher<TMessage, Type>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawMessageDispatcher{TMessage}"/> class.
        /// </summary>
        /// <param name="handlers">Message handler methods (this parameter is not used).</param>
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

        /// <summary>
        /// A method which passes messages through it. It does not perform any deserialization.
        /// </summary>
        /// <param name="messageType">A message type (this parameter is not used).</param>
        /// <param name="rawMessage">The message to pass through.</param>
        /// <param name="deserialized">The object to output the raw message as.</param>
        /// <returns><c>true</c></returns>
        protected override bool TryDeserialize(Type messageType, TMessage rawMessage, out object deserialized)
        {
            deserialized = rawMessage;
            return true;
        }
    }
}
