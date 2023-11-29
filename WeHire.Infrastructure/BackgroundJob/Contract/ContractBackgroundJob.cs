using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Infrastructure.Services.ContractServices;

namespace WeHire.Infrastructure.BackgroundJob.Contract
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
            Console.WriteLine(DateTime.Now);
            await _contractService.FailContractOnBackgroundAsync(DateTime.Now);
        }
    }
}
