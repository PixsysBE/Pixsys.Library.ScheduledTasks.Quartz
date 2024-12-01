# Pixsys.Library.ScheduledTasks.Quartz

A set of helpers to create one time job and recurrent jobs using [Quartz Scheduler](https://www.quartz-scheduler.net).

## 1. Installation

### 1.1 Register the service in `Program.cs`

```csharp
using Pixsys.Library.ScheduledTasks.Quartz;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddQuartzScheduler();
```

If you have recurrent jobs, you can also register them. This function will look for classes inheriting `RecurringJob` located in the provided assembly and schedule jobs if triggers are found (manual or in the appsettings). Just remember to make the `Main` method async

```csharp
public static async Task Main(string[] args)
{
...
    var app = builder.Build();

    await app.ScheduleRecurringJobsAsync(typeof(Program).Assembly);
}
```

## 2. Usage

You can then create your one-time job :

```csharp
using Pixsys.Library.ScheduledTasks.Quartz;
using Pixsys.Library.ScheduledTasks.Quartz.Base;
using Quartz;
using Pixsys.Library.ScheduledTasks.Quartz.Extensions;

public class HelloJob(ILogger<BaseJob> logger, ISchedulerFactory schedulerFactory, IServiceProvider serviceProvider) : OneTimeJob(logger, schedulerFactory)
{
    protected override JobKey Key => new JobKey("hello", "global");

    public override async Task ExecuteTask(IJobExecutionContext context)
    {
        logger.LogJobInformation("Hello World!");
        // Testing job exception
        throw new JobExecutionException("Error while executing the job");
        await Task.CompletedTask;
    }

    public override List<IJobListener> GetJobListeners()
    {
        return [ActivatorUtilities.CreateInstance<HelloJobListener>(serviceProvider)];
    }
}

```

A job listener can be attached to the job
```csharp
using Pixsys.Library.ScheduledTasks.Quartz.Base;
using Pixsys.Library.ScheduledTasks.Quartz.Extensions;
using Quartz;

public class HelloJobListener(ILogger<BaseJob> logger) : BaseJobListener(logger)
{
    private readonly ILogger<BaseJob> logger = logger;

    public override async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        this.logger.LogJobInformation("job will be executed");
        await Task.CompletedTask;
    }

    public override async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
    {
        if (jobException != null)
        {
            this.logger.LogJobInformation("A job exception has been raised");
        }
        this.logger.LogJobInformation("job was executed");
        await Task.CompletedTask;
    }
}
```

Here is an example of a recurring job, where you can add additional manual triggers

```csharp
using Pixsys.Library.ScheduledTasks.Quartz;
using Pixsys.Library.ScheduledTasks.Quartz.Base;
using Pixsys.Library.ScheduledTasks.Quartz.Extensions;
using Quartz;

[DisallowConcurrentExecution]
public class RecurringHello(ILogger<BaseJob> logger, ISchedulerFactory schedulerFactory, IConfiguration config, IServiceProvider serviceProvider) : RecurringJob(logger, schedulerFactory, config)
{
    protected override JobKey Key => new JobKey("hello", "recurring");

    public override Task ExecuteTask(IJobExecutionContext context)
    {
        //var var1 = context.MergedJobDataMap.GetIntValue("var1");
        //var var2 = context.MergedJobDataMap.GetString("var2");

        logger.LogJobInformation($"Hello world! {DateTime.Now.ToShortTimeString()}");
        return Task.CompletedTask;
    }

    public override List<IJobListener> GetJobListeners()
    {
        return [ActivatorUtilities.CreateInstance<HelloJobListener>(serviceProvider)];
    }

    protected override List<ITrigger> GetManualTriggers()
    {
        var trigger = TriggerBuilder.Create()
       .ForJob(this.Key)
       .StartNow()
       .Build();

        return new List<ITrigger> { trigger };
    }
}
```

You can also register your crontab schedules for your recurrent jobs in `appsettings.json`

```json
  "ScheduledTasks": {
    "Quartz": {
      "job group": {
        "job name": [ "0 0/1 * 1/1 * ? *" ]
      }
    }
  }     
```