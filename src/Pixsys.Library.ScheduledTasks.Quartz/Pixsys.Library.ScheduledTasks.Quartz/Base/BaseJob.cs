// -----------------------------------------------------------------------
// <copyright file="BaseJob.cs" company="Pixsys">
// Copyright (c) Pixsys. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Pixsys.Library.ScheduledTasks.Quartz.Extensions;
using Quartz;

namespace Pixsys.Library.ScheduledTasks.Quartz.Base
{
    /// <summary>
    /// The base job.
    /// </summary>
    /// <seealso cref="IJob" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="BaseJob"/> class.
    /// </remarks>
    /// <param name="logger">The logger.</param>
    /// <param name="schedulerFactory">The scheduler factory.</param>
    public abstract class BaseJob(ILogger<BaseJob> logger, ISchedulerFactory schedulerFactory) : IJob
    {
        /// <summary>
        /// Gets the type name.
        /// </summary>
        /// <value>
        /// The type name.
        /// </value>
        public virtual string? TypeName
        {
            get
            {
                Type type = GetType();
                return type == null ? string.Empty : type.FullName;
            }
        }

        /// <summary>
        /// Gets or sets the job detail.
        /// </summary>
        /// <value>
        /// The job detail.
        /// </value>
        public IJobDetail? JobDetail { get; set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        protected abstract JobKey Key { get; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <returns>The job key.</returns>
        public virtual JobKey GetKey()
        {
            return Key;
        }

        /// <summary>
        /// Gets the job listeners.
        /// </summary>
        /// <returns>The list of listeners.</returns>
        public abstract List<IJobListener> GetJobListeners();

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public abstract Task ExecuteTask(IJobExecutionContext context);

        /// <inheritdoc />
        public Task Execute(IJobExecutionContext context)
        {
            return ExecuteTask(context);
        }

        /// <summary>
        /// Builds the job.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The job detail.</returns>
        public IJobDetail Build(JobDataMap? data = null)
        {
            JobBuilder job = JobBuilder.Create(GetType())
                                .WithIdentity(GetKey());
            if (data != null)
            {
                job = job.UsingJobData(data);
            }

            JobDetail = job.Build();
            return JobDetail;
        }

        /// <summary>
        /// Gets the scheduler.
        /// </summary>
        /// <returns>The <see cref="IScheduler"/>.</returns>
        protected async Task<IScheduler> GetSchedulerAsync()
        {
            return await schedulerFactory.GetScheduler();
        }

        /// <summary>
        /// Sends an error mail if smtp settings are configured.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        protected void SendMailError(string errorMessage)
        {
            logger.LogJobError(errorMessage);

            // if (string.IsNullOrWhiteSpace(AppSettings.Email.EmailsError)) return;
            // var emails = AppSettings.Email.EmailsError.Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            // var key = GetKey();
            // var content = "An error occured in job (with key = " + key.Name + "-" + key.Group +
            //              ") " + Environment.NewLine + Environment.NewLine +
            //              Environment.NewLine + "Error message : " + errorMessage;
            // MailManager.Instance.SendEmail("An error occured in job", content, null, emails);
        }
    }
}