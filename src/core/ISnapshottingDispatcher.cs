// <copyright file="ISnapshottingDispatcher.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Core
{
    using System.Collections.Generic;

    public interface ISnapshottingDispatcher<TMessage> : IDispatcher<TMessage>
    {
        IDispatcher<TMessage> InnerDispatcher { get; set; }

        int? LoadCheckpoint();

        IEnumerable<object> LoadObjects();
    }
}
