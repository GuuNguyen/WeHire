using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Infrastructure.Services.HiringRequestServices;

namespace WeHire.Infrastructure.BackgroundJob.HiringRequest
{
    [DisallowConcurrentExecution]
    public class HiringRequestBackgroundJob : IJob
    {
        private readonly IHiringRequestService _hiringRequestService;

        public HiringRequestBackgroundJob(IHiringRequestService hiringRequestService)
        {
            _hiringRequestService = hiringRequestService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var currentTime = TimeZoneInfo.ConvertTime(DateTime.Now,
                              TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            await _hiringRequestService.CheckRequestDeadline(currentTime);
        }
    }
}
