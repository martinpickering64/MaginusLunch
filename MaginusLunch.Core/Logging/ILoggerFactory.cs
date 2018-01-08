using System;

namespace MaginusLunch.Logging
{
    /// <summary>
    /// Used by <see cref="T:NServiceBus.Logging.LogManager" /> to facilitate redirecting logging to a different library.
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Gets a <see cref="T:MaginusLunch.Logging.ILog" /> for a specific <paramref name="type" />.
        /// </summary>
        /// <param name="type">The <see cref="T:System.Type" /> to get the <see cref="T:MaginusLunch.Logging.ILog" /> for.</param>
        /// <returns>An instance of a <see cref="T:NServiceBus.Logging.ILog" /> specifically for <paramref name="type" />.</returns>
        ILog GetLogger(Type type);

        /// <summary>
        /// Gets a <see cref="T:MaginusLunch.Logging.ILog" /> for a specific <paramref name="name" />.
        /// </summary>
        /// <param name="name">The name of the usage to get the <see cref="T:MaginusLunch.Logging.ILog" /> for.</param>
        /// <returns>An instance of a <see cref="T:MaginusLunch.Logging.ILog" /> specifically for <paramref name="name" />.</returns>
        ILog GetLogger(string name);
    }
}
