// Copyright (c) Pharmaxo. All rights reserved.

using CorshamScience.MessageDispatch.Core;
using KurrentDB.Client;

namespace MessageDispatch.Snapshotting.Core.Tests.TestDoubles;

public class DispatcherSpy : IDispatcher<ResolvedEvent>
{
    public List<ResolvedEvent> DispatchedEvents { get; } = [];

    public void Dispatch(ResolvedEvent message) => DispatchedEvents.Add(message);
}
