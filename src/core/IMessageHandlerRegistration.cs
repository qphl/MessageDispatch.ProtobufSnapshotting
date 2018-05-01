// <copyright file="IMessageHandlerRegistration.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System;

    public interface IMessageHandlerRegistration<in TKey>
    {
        void Add(TKey messageType, object handler);

        void Add(TKey messageType, Action<object> handler);

        void Clear();
    }
}
