﻿using System;

namespace MaginusLunch.Logging
{
    /// <summary>Provides logging methods and utility functions.</summary>
    public interface ILog
    {
        /// <summary>
        /// Gets a value indicating whether logging is enabled for the level.
        /// </summary>
        bool IsVerboseEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the level.
        /// </summary>
        bool IsDebugEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the level.
        /// </summary>
        bool IsInfoEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the level.
        /// </summary>
        bool IsWarnEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the level.
        /// </summary>
        bool IsErrorEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether logging is enabled for the level.
        /// </summary>
        bool IsFatalEnabled { get; }

        /// <summary>
        /// Writes the message at the level.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Verbose(string message);

        /// <summary>
        /// Writes the message and exception at the level.
        /// </summary>
        /// <param name="message">A string to be written.</param>
        /// <param name="exception">An exception to be logged.</param>
        void Verbose(string message, Exception exception);

        /// <summary>
        /// Writes the message at the level using the specified <paramref name="format" /> provider
        /// and format <paramref name="args" />.
        /// </summary>
        /// <param name="format">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void VerboseFormat(string format, params object[] args);

        /// <summary>
        /// Writes the message at the level.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Debug(string message);

        /// <summary>
        /// Writes the message and exception at the level.
        /// </summary>
        /// <param name="message">A string to be written.</param>
        /// <param name="exception">An exception to be logged.</param>
        void Debug(string message, Exception exception);

        /// <summary>
        /// Writes the message at the level using the specified <paramref name="format" /> provider
        /// and format <paramref name="args" />.
        /// </summary>
        /// <param name="format">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void DebugFormat(string format, params object[] args);

        /// <summary>
        /// Writes the message at the level.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Info(string message);

        /// <summary>
        /// Writes the message and exception at the level.
        /// </summary>
        /// <param name="message">A string to be written.</param>
        /// <param name="exception">An exception to be logged.</param>
        void Info(string message, Exception exception);

        /// <summary>
        /// Writes the message at the level using the specified <paramref name="format" /> provider
        /// and format <paramref name="args" />.
        /// </summary>
        /// <param name="format">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void InfoFormat(string format, params object[] args);

        /// <summary>
        /// Writes the message at the level.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Warn(string message);

        /// <summary>
        /// Writes the message and exception at the level.
        /// </summary>
        /// <param name="message">A string to be written.</param>
        /// <param name="exception">An exception to be logged.</param>
        void Warn(string message, Exception exception);

        /// <summary>
        /// Writes the message at the level using the specified <paramref name="format" /> provider
        /// and format <paramref name="args" />.
        /// </summary>
        /// <param name="format">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void WarnFormat(string format, params object[] args);

        /// <summary>
        /// Writes the message at the level.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Error(string message);

        /// <summary>
        /// Writes the message and exception at the level.
        /// </summary>
        /// <param name="message">A string to be written.</param>
        /// <param name="exception">An exception to be logged.</param>
        void Error(string message, Exception exception);

        /// <summary>
        /// Writes the message at the level using the specified <paramref name="format" /> provider
        /// and format <paramref name="args" />.
        /// </summary>
        /// <param name="format">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void ErrorFormat(string format, params object[] args);

        /// <summary>
        /// Writes the message at the level.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Fatal(string message);

        /// <summary>
        /// Writes the message and exception at the level.
        /// </summary>
        /// <param name="message">A string to be written.</param>
        /// <param name="exception">An exception to be logged.</param>
        void Fatal(string message, Exception exception);

        /// <summary>
        /// Writes the message at the level using the specified <paramref name="format" /> provider
        /// and format <paramref name="args" />.
        /// </summary>
        /// <param name="format">A string containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void FatalFormat(string format, params object[] args);
    }
}
