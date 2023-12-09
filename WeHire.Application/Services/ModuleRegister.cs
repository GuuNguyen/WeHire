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
using WeHire.Application.Services.AccountServices;
using WeHire.Application.Services.ComapnyPartnerServices;
using WeHire.Application.Services.ContractServices;
using WeHire.Application.Services.DashboardServices;
using WeHire.Application.Services.DeveloperServices;
using WeHire.Application.Services.EducationServices;
using WeHire.Application.Services.EmailServices;
using WeHire.Application.Services.EmploymentTypeServices;
using WeHire.Application.Services.ExcelServices;
using WeHire.Application.Services.FileServices;
using WeHire.Application.Services.GenderServices;
using WeHire.Application.Services.HiredDeveloperServices;
using WeHire.Application.Services.HiringRequestServices;
using WeHire.Application.Services.InterviewServices;
using WeHire.Application.Services.LevelServices;
using WeHire.Application.Services.NotificationServices;
using WeHire.Application.Services.PaymentServices;
using WeHire.Application.Services.PayPeriodServices;
using WeHire.Application.Services.PaySlipServices;
using WeHire.Application.Services.PercentCalculatServices;
using WeHire.Application.Services.ProfessionalExperienceServices;
using WeHire.Application.Services.ProjectServices;
using WeHire.Application.Services.ProjectTypeServices;
using WeHire.Application.Services.ReportServices;
using WeHire.Application.Services.ReportTypeServices;
using WeHire.Application.Services.RequestStatusServices;
using WeHire.Application.Services.RoleServices;
using WeHire.Application.Services.SkillServices;
using WeHire.Application.Services.TeamMeetingServices;
using WeHire.Application.Services.TransactionServices;
using WeHire.Application.Services.TypeServices;
using WeHire.Application.Services.UserDeviceServices;
using WeHire.Application.Services.UserServices;
using WeHire.Application.Services.WorkLogServices;
using WeHire.Infrastructure.Repositories;
using WeHire.Infrastructure.IRepositories;

namespace WeHire.Application.Services
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
            services.AddScoped<IPaySlipService, PaySlipService>();
            services.AddScoped<IPayPeriodService, PayPeriodService>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IWorkLogService, WorkLogService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IReportTypeService, ReportTypeService>();
        }
    }
}
