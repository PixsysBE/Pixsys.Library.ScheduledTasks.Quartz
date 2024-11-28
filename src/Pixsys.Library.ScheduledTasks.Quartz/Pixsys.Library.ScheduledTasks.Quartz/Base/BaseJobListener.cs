// -----------------------------------------------------------------------
// <copyright file="BaseJobListener.cs" company="Pixsys">
// Copyright (c) Pixsys. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Pixsys.Library.ScheduledTasks.Quartz.Extensions;
using Quartz;

namespace Pixsys.Library.ScheduledTasks.Quartz.Base
{
    /// <summary>
    /// The base job listener.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <seealso cref="IJobListener" />
    public class BaseJobListener(ILogger<BaseJob> logger) : IJobListener
    {
        /// <inheritdoc />
        public string Name
        {
            get
            {
                Type type = GetType();
                return type == null ? string.Empty : $"{type.FullName}";
            }
        }

        /// <inheritdoc />
        public virtual async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            logger.LogJobInformation($"Job Execution Vetoed: {context.JobDetail.Key}");
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            // var var1 = context.MergedJobDataMap.GetIntValue("var1");
            // var var2 = context.MergedJobDataMap.GetString("var2");
            logger.LogJobInformation($"Job To Be Executed: {context.JobDetail.Key}");
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            // var var1 = context.MergedJobDataMap.GetIntValue("var1");
            // var var2 = context.MergedJobDataMap.GetString("var2");
            logger.LogJobInformation($"JobWasExecuted: {context.JobDetail.Key}");
            await Task.CompletedTask;
        }
    }
}