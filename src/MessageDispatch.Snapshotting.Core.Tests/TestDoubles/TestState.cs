// Copyright (c) Pharmaxo. All rights reserved.

using ProtoBuf;

namespace MessageDispatch.Snapshotting.Core.Tests.TestDoubles;

[Serializable]
[ProtoContract]
public record TestState
{
    [ProtoMember(1)]
    public required string Field1 { get; init; }

    [ProtoMember(2)]
    public required int Field2 { get; init; }
}
