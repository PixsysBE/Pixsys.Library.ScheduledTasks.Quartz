// -----------------------------------------------------------------------
// <copyright file="RecurringJob.cs" company="Pixsys">
// Copyright (c) Pixsys. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pixsys.Library.ScheduledTasks.Quartz.Base;
using Quartz;

namespace Pixsys.Library.ScheduledTasks.Quartz
{
    /// <summary>
    /// A representation of a recurring job.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="schedulerFactory">The scheduler factory.</param>
    /// <param name="config">The configuration.</param>
    /// <seealso cref="BaseJob" />
    public abstract class RecurringJob(ILogger<BaseJob> logger, ISchedulerFactory schedulerFactory, IConfiguration config) : BaseJob(logger, schedulerFactory)
    {
        /// <summary>
        /// Gets all the triggers.
        /// </summary>
        /// <returns>The list of triggers.</returns>
        public List<ITrigger> GetAllTriggers()
        {
            List<ITrigger> triggers = GetCrontabTriggersFromConfig();
            triggers.AddRange(GetManualTriggers());
            return triggers;
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

        /// <summary>
        /// Gets the manual triggers.
        /// </summary>
        /// <returns>The list of triggers.</returns>
        protected abstract List<ITrigger> GetManualTriggers();

        /// <summary>
        /// <para>CronMaker: <see href="http://www.cronmaker.com/"/>.</para>
        /// <para>Doc: <see href="https://www.quartz-scheduler.net/documentation/quartz-3.x/how-tos/crontrigger.html" />.</para>
        /// </summary>
        /// <remarks>
        /// <code>
        /// "ScheduledTasks": {
        ///          "Quartz": {
        ///            "job group": {
        ///              "job name": [ "0 0/1 * 1/1 * ? *" ]
        ///            }
        ///        }
        ///    }
        ///    </code>
        /// </remarks>
        /// <returns>The list of triggers.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1010:Opening square brackets should be spaced correctly", Justification = "Reviewed.")]
        protected List<ITrigger> GetCrontabTriggersFromConfig()
        {
            List<ITrigger> list = [];
            string configKey = $"ScheduledTasks:Quartz:{Key.Group}:{Key.Name}";

            List<string>? cronSchedules = config.GetSection(configKey).Get<List<string>>();

            if (cronSchedules != null && cronSchedules.Count != 0)
            {
                foreach (string cronSchedule in cronSchedules)
                {
                    ITrigger trigger = TriggerBuilder.Create()
                                   .ForJob(Key)
                                   .WithCronSchedule(cronSchedule)
                                   .Build();
                    list.Add(trigger);
                }
            }

            return list;
        }
    }
}