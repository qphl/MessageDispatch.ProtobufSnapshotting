// Copyright (c) Pharmaxo. All rights reserved.

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core
{
    /// <summary>
    /// Defines methods for providing state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    public interface IStateProvider<out TState>
    {
        /// <summary>
        /// Gets the current state.
        /// </summary>
        /// <returns>The current state.</returns>
        TState GetState();
    }
}
