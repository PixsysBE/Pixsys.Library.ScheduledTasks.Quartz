// -----------------------------------------------------------------------
// <copyright file="ScheduledTasksQuartzExtensions.cs" company="Pixsys">
// Copyright (c) Pixsys. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Reflection;

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace Pixsys.Library.ScheduledTasks.Quartz
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Quartz extensions.
    /// </summary>
    public static class ScheduledTasksQuartzExtensions
    {
        /// <summary>
        /// Adds the quartz scheduler.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddQuartzScheduler(this IServiceCollection services)
        {
            _ = services
                .AddQuartz()
                .AddQuartzHostedService(options =>
                {
                    // when shutting down we want jobs to complete gracefully
                    options.WaitForJobsToComplete = true;
                });
        }

        /// <summary>
        /// Schedules the recurring jobs.
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
        /// <param name="app">The web application.</param>
        /// <param name="assembly">The assembly where are located the <see cref="RecurringJob"/> sub-classes.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ScheduleRecurringJobsAsync(this WebApplication app, Assembly assembly)
        {
            IServiceProvider? serviceProvider = app.Services.GetService<IServiceProvider>();
            if (serviceProvider != null)
            {
                using IServiceScope scope = app.Services.CreateScope();
                ISchedulerFactory? schedulerFactory = scope.ServiceProvider.GetService<ISchedulerFactory>();
                if (schedulerFactory != null)
                {
                    IScheduler scheduler = await schedulerFactory.GetScheduler();
                    IEnumerable<RecurringJob?> jobs = assembly.GetTypes()
                                                     .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(RecurringJob)))
                                                     .Select(t => (RecurringJob?)ActivatorUtilities.CreateInstance(serviceProvider, t)).ToList();
                    foreach (RecurringJob? job in jobs)
                    {
                        if (job != null)
                        {
                            List<ITrigger> jobTriggers = job.GetAllTriggers();
                            if (jobTriggers != null && jobTriggers.Count != 0)
                            {
                                await scheduler.ScheduleJob(job.Build(), jobTriggers, true);
                                List<IJobListener> listeners = job.GetJobListeners();
                                if (listeners != null && listeners.Count != 0)
                                {
                                    foreach (IJobListener listener in listeners)
                                    {
                                        scheduler.ListenerManager.AddJobListener(listener);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}