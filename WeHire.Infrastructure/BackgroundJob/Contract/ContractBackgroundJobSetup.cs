using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Infrastructure.BackgroundJob.HiringRequest;
using WeHire.Infrastructure.Services.ContractServices;

namespace WeHire.Infrastructure.BackgroundJob.Contract
{
    public class ContractBackgroundJobSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            var jobKey = JobKey.Create(nameof(ContractBackgroundJob));

            options.AddJob<ContractBackgroundJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
                   .AddTrigger(trigger =>
                               trigger.ForJob(jobKey)
                                      //.StartAt(DateBuilder.TodayAt(0, 0, 0))
                                      .WithSimpleSchedule(schedule =>
                                                          schedule.WithIntervalInHours(1)
                                                                  .RepeatForever()));
        }
    }
}
