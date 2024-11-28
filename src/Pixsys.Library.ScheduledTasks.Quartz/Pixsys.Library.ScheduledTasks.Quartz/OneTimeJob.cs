// -----------------------------------------------------------------------
// <copyright file="OneTimeJob.cs" company="Pixsys">
// Copyright (c) Pixsys. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Pixsys.Library.ScheduledTasks.Quartz.Base;
using Quartz;
using Quartz.Impl.Matchers;

namespace Pixsys.Library.ScheduledTasks.Quartz
{
    /// <summary>
    /// A representation of a one-time job.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="schedulerFactory">The scheduling factory.</param>
    /// <seealso cref="BaseJob" />
    [DisallowConcurrentExecution]
    public abstract class OneTimeJob(ILogger<BaseJob> logger, ISchedulerFactory schedulerFactory) : BaseJob(logger, schedulerFactory)
    {
        /// <summary>
        /// Starts the job now.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StartNowAsync(JobDataMap? data = null)
        {
            TriggerBuilder trigger = TriggerBuilder.Create();
            trigger = trigger.ForJob(Key);
            if (data != null)
            {
                trigger = trigger.UsingJobData(data);
            }

            trigger = trigger.StartNow();
            IScheduler scheduler = await GetSchedulerAsync();
            foreach (IJobListener listener in GetJobListeners())
            {
                scheduler.ListenerManager.AddJobListener(listener, KeyMatcher<JobKey>.KeyEquals(Key));
            }

            _ = await scheduler.ScheduleJob(Build(), trigger.Build());
        }

        /// <summary>
        /// Gets the job listeners.
        /// </summary>
        /// <returns>
        /// The list of listeners.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Reviewed.")]
        public override List<IJobListener> GetJobListeners()
        {
            return [];
        }
    }
}