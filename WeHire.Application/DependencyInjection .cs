using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.Utilities.Helper.Mapping;
using WeHire.Application.Services;
using Quartz;
using WeHire.Application.BackgroundJob.Contract;
using WeHire.Application.BackgroundJob.HiringRequest;
using WeHire.Application.BackgroundJob.Interview;

namespace WeHire.Application
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
            //services.ConfigureOptions<ContractBackgroundJobSetup>();
        }
    }
}
