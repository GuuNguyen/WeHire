﻿using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.Services.InterviewServices;

namespace WeHire.Application.BackgroundJob.Interview
{
    public class InterviewBackgroundJob : IJob
    {
        private readonly IInterviewService _interviewService;

        public InterviewBackgroundJob(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _interviewService.ExpireInterviewAsync(DateTime.Now);
        }
    }
}
