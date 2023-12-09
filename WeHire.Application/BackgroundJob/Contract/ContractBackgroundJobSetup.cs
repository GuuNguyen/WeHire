using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.BackgroundJob.HiringRequest;
using WeHire.Application.Services.ContractServices;

namespace WeHire.Application.BackgroundJob.Contract
{
    public class ContractBackgroundJobSetup : IConfigureOptions<QuartzOptions>
    {
        public void Configure(QuartzOptions options)
        {
            var jobKey = JobKey.Create(nameof(ContractBackgroundJob));

            options.AddJob<ContractBackgroundJob>(jobBuilder => jobBuilder.WithIdentity(jobKey))
                   .AddTrigger(trigger =>
                               trigger.ForJob(jobKey)
                                      .WithSimpleSchedule(schedule =>
                                                          schedule.WithIntervalInHours(12)
                                                                  .RepeatForever()));
        }
    }
}
