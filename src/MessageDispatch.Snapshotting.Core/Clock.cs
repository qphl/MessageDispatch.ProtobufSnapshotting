// Copyright (c) Pharmaxo. All rights reserved.

using System;

namespace PharmaxoScientific.MessageDispatch.Snapshotting.Core
{
    /// <summary>
    /// Provides a way to retrieve the current date and time, with the ability to override the default implementation for testing purposes.
    /// </summary>
    public static class Clock
    {
        private static Func<DateTime> _nowValueProvider;

        /// <summary>
        /// Initializes static members of the <see cref="Clock"/> class.
        /// <remarks>
        /// Default implementation uses <see cref="DateTime.Now"/>.
        /// </remarks>
        /// </summary>
        static Clock() => _nowValueProvider = () => DateTime.Now;

        /// <summary>
        /// Gets the current date and time.
        /// </summary>
        /// <remarks>
        /// This property retrieves the current date and time by invoking the current implementation specified by <see cref="Initialize"/>.
        /// </remarks>
        public static DateTime Now => _nowValueProvider();

        /// <summary>
        /// Initializes the <see cref="Clock"/> class with a custom implementation of <see cref="DateTime"/>.
        /// </summary>
        /// <param name="nowValueProvider">The function that provides the current date and time.</param>
        /// <remarks>
        /// This method allows the default implementation of <see cref="DateTime.Now"/> to be replaced with a custom implementation for testing purposes.
        /// </remarks>
        public static void Initialize(Func<DateTime> nowValueProvider) => _nowValueProvider = nowValueProvider;
    }
}
