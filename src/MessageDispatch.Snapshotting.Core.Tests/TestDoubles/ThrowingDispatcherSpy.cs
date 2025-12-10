// Copyright (c) Pharmaxo. All rights reserved.

using CorshamScience.MessageDispatch.Core;
using KurrentDB.Client;

namespace MessageDispatch.Snapshotting.Core.Tests.TestDoubles;

public class ThrowingDispatcherSpy : IDispatcher<ResolvedEvent>
{
    public bool ThrowOnDispatch { get; set; }

    public List<ResolvedEvent> DispatchedEvents { get; } = [];

    public void Dispatch(ResolvedEvent message)
    {
        DispatchedEvents.Add(message);

        if (ThrowOnDispatch)
        {
            throw new Exception("Something went wrong");
        }
    }
}
