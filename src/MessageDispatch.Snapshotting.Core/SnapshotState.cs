// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

public record SnapshotState<TState>(TState InitialState, long EventNumber);
