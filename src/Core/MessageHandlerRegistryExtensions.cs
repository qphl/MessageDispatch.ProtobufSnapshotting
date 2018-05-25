// <copyright file="MessageHandlerRegistryExtensions.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;
    using System.Linq;

    /// <summary>
    /// Extension methods for a <see cref="MessageHandlerRegistry{TKey}"/>.
    /// </summary>
    public static class MessageHandlerRegistryExtensions
    {
        /// <summary>
        /// Registers all message handlers (which implement <see cref="IConsume{TMessage}"/>) whose type matches a specified handler's type.
        /// </summary>
        /// <param name="registry">The <see cref="MessageHandlerRegistry{TKey}"/> to add handler methods to.</param>
        /// <param name="handler">An message handler object whose type will be used to identify other handler methods to register.</param>
        public static void AddByConvention(this MessageHandlerRegistry<Type> registry, object handler)
        {
            var handlerType = handler.GetType();

            var handlerInterfaces = handlerType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsume<>)).ToArray();

            foreach (var messageType in handlerInterfaces.Select(i => i.GetGenericArguments()[0]))
            {
                registry.Add(messageType, handler);
            }
        }
    }
}
