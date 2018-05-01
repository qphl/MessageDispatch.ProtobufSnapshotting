// <copyright file="IMessageHandlerLookup.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System.Collections.Generic;

    public interface IMessageHandlerLookup<in TKey>
    {
        List<object> HandlersForMessageType(TKey messageType);
    }
}
