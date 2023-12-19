using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.Services.ContractServices;

namespace WeHire.Application.BackgroundJob.Contract
{
    public class ContractBackgroundJob : IJob
    {
        private readonly IContractService _contractService;

        public ContractBackgroundJob(IContractService contractService)
        {
            _contractService = contractService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _contractService.EndContractOnBackgroundAsync(DateTime.Now);
        }
    }
}
