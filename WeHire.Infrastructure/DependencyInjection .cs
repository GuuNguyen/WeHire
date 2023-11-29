using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.Utilities.Helper.Mapping;
using WeHire.Infrastructure.BackgroundJob.Contract;
using WeHire.Infrastructure.BackgroundJob.HiringRequest;
using WeHire.Infrastructure.BackgroundJob.Interview;
using WeHire.Infrastructure.Services;

namespace WeHire.Infrastructure
{
    public static class DependencyInjection 
    {
        public static void InfrastructureRegister(this IServiceCollection services)
        {
            services.ServiceRegister();
            services.ConfigureAutoMapper();
            services.AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
            });
            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            services.ConfigureOptions<HiringRequestBackgroundJobSetup>();
            services.ConfigureOptions<InterviewBackgroundJobSetup>();
            services.ConfigureOptions<ContractBackgroundJobSetup>();
        }
    }
}
