// <copyright file="MessageHandlerRegistryExtensions.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;
    using System.Linq;

    public static class MessageHandlerRegistryExtensions
    {
        /// <summary>
        /// Adds based on all the IConsume interfaces that this object implements
        /// </summary>
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
