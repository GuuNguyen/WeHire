using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Infrastructure.BackgroundJob.HiringRequest
{
    public class HiringRequestBackgroundJobSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            var jobKey = JobKey.Create(nameof(HiringRequestBackgroundJob));

            options.AddJob<HiringRequestBackgroundJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
                   .AddTrigger(trigger =>
                               trigger.ForJob(jobKey)
                                      .WithSimpleSchedule(schedule => 
                                                          schedule.WithIntervalInHours(1)
                                                                  .RepeatForever()));
        }
    }
}
