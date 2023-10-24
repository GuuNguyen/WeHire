using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.Utilities.Helper;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using WeHire.Entity.Repositories;
using WeHire.Infrastructure.Services.AgreementServices;
using WeHire.Infrastructure.Services.AssignTaskServices;
using WeHire.Infrastructure.Services.ComapnyPartnerServices;
using WeHire.Infrastructure.Services.CvServices;
using WeHire.Infrastructure.Services.DeveloperServices;
using WeHire.Infrastructure.Services.EmploymentTypeServices;
using WeHire.Infrastructure.Services.FileServices;
using WeHire.Infrastructure.Services.GenderServices;
using WeHire.Infrastructure.Services.GoogleCalendarServices;
using WeHire.Infrastructure.Services.HiringRequestServices;
using WeHire.Infrastructure.Services.InterviewServices;
using WeHire.Infrastructure.Services.LevelServices;
using WeHire.Infrastructure.Services.PaymentServices;
using WeHire.Infrastructure.Services.PercentCalculatServices;
using WeHire.Infrastructure.Services.PostedTimeCalculatorService;
using WeHire.Infrastructure.Services.RequestStatusServices;
using WeHire.Infrastructure.Services.RoleServices;
using WeHire.Infrastructure.Services.ScheduleTypeServices;
using WeHire.Infrastructure.Services.SelectingDevServices;
using WeHire.Infrastructure.Services.SkillServices;
using WeHire.Infrastructure.Services.TransactionServices;
using WeHire.Infrastructure.Services.TypeServices;
using WeHire.Infrastructure.Services.UserServices;

namespace WeHire.Infrastructure.Services
{
    public static class ModuleRegister
    {
        public static void ServiceRegister(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtHelper, JwtHelper>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDeveloperService, DeveloperService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<ITypeService, TypeService>();
            services.AddScoped<ILevelService, LevelService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IHiringRequestService, HiringRequestService>();
            services.AddScoped<ICvService, CvService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRequestStatusService, RequestStatusService>();
            services.AddScoped<ISelectingDevService, SelectingDevService>();
            services.AddScoped<IInterviewService, InterviewService>();
            services.AddScoped<IPercentCalculateService, PercentCalculateService>();
            services.AddScoped<IAssignTaskService, AssignTaskService>();
            services.AddScoped<IGenderService, GenderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEmploymentTypeService, EmploymentTypeService>();
            services.AddScoped<IScheduleTypeService, ScheduleTypeService>();
            services.AddScoped<IAgreementService, AgreementService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IPostedTimeCalculatorService, PostedTimeCalculatorService.PostedTimeCalculatorService>();
            services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();
        }
    }
}
