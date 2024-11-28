// -----------------------------------------------------------------------
// <copyright file="LoggerExtensions.cs" company="Pixsys">
// Copyright (c) Pixsys. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Pixsys.Library.ScheduledTasks.Quartz.Extensions
{
    /// <summary>
    /// Logger extensions.
    /// </summary>
    public static partial class LoggerExtensions
    {
        /// <summary>
        /// Logs the job information.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        [LoggerMessage(
            EventId = 2,
            Level = LogLevel.Information,
            Message = "{message}"
        )]
        public static partial void LogJobInformation(this ILogger logger, string @message);

        /// <summary>
        /// Logs the job error.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        [LoggerMessage(
            EventId = 4,
            Level = LogLevel.Error,
            Message = "{message}"
        )]
        public static partial void LogJobError(this ILogger logger, string @message);
    }
}