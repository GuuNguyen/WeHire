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
using WeHire.Infrastructure.Services.AccountServices;
using WeHire.Infrastructure.Services.ComapnyPartnerServices;
using WeHire.Infrastructure.Services.ContractServices;
using WeHire.Infrastructure.Services.DeveloperServices;
using WeHire.Infrastructure.Services.EducationServices;
using WeHire.Infrastructure.Services.EmailServices;
using WeHire.Infrastructure.Services.EmploymentTypeServices;
using WeHire.Infrastructure.Services.ExcelServices;
using WeHire.Infrastructure.Services.FileServices;
using WeHire.Infrastructure.Services.GenderServices;
using WeHire.Infrastructure.Services.HiredDeveloperServices;
using WeHire.Infrastructure.Services.HiringRequestServices;
using WeHire.Infrastructure.Services.InterviewServices;
using WeHire.Infrastructure.Services.JobPositionServices;
using WeHire.Infrastructure.Services.LevelServices;
using WeHire.Infrastructure.Services.NotificationServices;
using WeHire.Infrastructure.Services.PaymentServices;
using WeHire.Infrastructure.Services.PayPeriodServices;
using WeHire.Infrastructure.Services.PaySlipServices;
using WeHire.Infrastructure.Services.PercentCalculatServices;
using WeHire.Infrastructure.Services.ProfessionalExperienceServices;
using WeHire.Infrastructure.Services.ProjectServices;
using WeHire.Infrastructure.Services.ProjectTypeServices;
using WeHire.Infrastructure.Services.RequestStatusServices;
using WeHire.Infrastructure.Services.RoleServices;
using WeHire.Infrastructure.Services.SelectingDevServices;
using WeHire.Infrastructure.Services.SkillServices;
using WeHire.Infrastructure.Services.TeamMeetingServices;
using WeHire.Infrastructure.Services.TransactionServices;
using WeHire.Infrastructure.Services.TypeServices;
using WeHire.Infrastructure.Services.UserDeviceServices;
using WeHire.Infrastructure.Services.UserServices;
using WeHire.Infrastructure.Services.WorkLogServices;

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
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRequestStatusService, RequestStatusService>();
            services.AddScoped<ISelectingDevService, SelectingDevService>();
            services.AddScoped<IInterviewService, InterviewService>();
            services.AddScoped<IPercentCalculateService, PercentCalculateService>();
            services.AddScoped<IGenderService, GenderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEmploymentTypeService, EmploymentTypeService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEducationService, EducationService>();
            services.AddScoped<IProfessionalExperienceService, ProfessionalExperienceService>();
            services.AddScoped<IUserDeviceService, UserDeviceService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITeamMeetingService, TeamMeetingService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectTypeService, ProjectTypeService>();
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<IHiredDeveloperService, HiredDeveloperService>();
            services.AddScoped<IJobPositionService, JobPositionService>();
            services.AddScoped<IPaySlipService, PaySlipService>();
            services.AddScoped<IPayPeriodService, PayPeriodService>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IWorkLogService, WorkLogService>();
        }
    }
}
