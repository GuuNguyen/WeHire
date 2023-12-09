using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.BackgroundJob.Interview
{
    public class InterviewBackgroundJobSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            var jobKey = JobKey.Create(nameof(InterviewBackgroundJob));

            options.AddJob<InterviewBackgroundJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
                   .AddTrigger(trigger =>
                               trigger.ForJob(jobKey)
                                      .WithSimpleSchedule(schedule =>
                                                          schedule.WithIntervalInHours(1)
                                                                  .RepeatForever()));
        }
    }
}
