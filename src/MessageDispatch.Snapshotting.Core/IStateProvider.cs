// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core;

public interface IStateProvider<TState>
{
    TState? GetState();
}
