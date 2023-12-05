using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Domain.Entities;
using Type = WeHire.Domain.Entities.Type;

namespace WeHire.Entity.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Developer> DeveloperRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<Level> LevelRepository { get; }
        IGenericRepository<Skill> SkillRepository { get; }
        IGenericRepository<Type> TypeRepository { get; }
        IGenericRepository<CompanyPartner> CompanyRepository { get; }
        IGenericRepository<HiringRequest> RequestRepository { get; }
        IGenericRepository<Interview> InterviewRepository { get; }
        IGenericRepository<Gender> GenderRepository { get; }
        IGenericRepository<EmploymentType> EmploymentTypeRepository { get; }
        IGenericRepository<Transaction> TransactionRepository { get; }
        IGenericRepository<Education> EducationRepository { get; }
        IGenericRepository<ProfessionalExperience> ProfessionalExperienceRepository { get; }
        IGenericRepository<Notification> NotificationRepository { get; }
        IGenericRepository<NotificationType> NotificationTypeRepository { get; }
        IGenericRepository<UserNotification> UserNotificationRepository { get; }
        IGenericRepository<UserDevice> UserDeviceRepository { get; }
        IGenericRepository<Project> ProjectRepository { get; }
        IGenericRepository<HiredDeveloper> HiredDeveloperRepository { get; }
        IGenericRepository<Contract> ContractRepository { get; }
        IGenericRepository<ProjectType> ProjectTypeRepository { get; }
        IGenericRepository<PayPeriod> PayPeriodRepository { get; }
        IGenericRepository<PaySlip> PaySlipRepository { get; }
        IGenericRepository<WorkLog> WorkLogRepository { get; }
        IGenericRepository<Report> ReportRepository { get; }
        IGenericRepository<ReportType> ReportTypeRepository { get; }

        IDbTransaction BeginTransaction(); 
        Task SaveChangesAsync();
    }
}
