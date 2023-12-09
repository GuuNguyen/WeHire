using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Domain.Entities;
using WeHire.Infrastructure.IRepositories;

namespace WeHire.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WeHireDBContext _context;
        private IGenericRepository<User> _userRepository;
        private IGenericRepository<Developer> _DeveloperRepository;
        private IGenericRepository<Role> _RoleRepository;
        private IGenericRepository<Level> _LevelRepository;
        private IGenericRepository<Skill> _SkillRepository;
        private IGenericRepository<Domain.Entities.Type> _TypeRepository;
        private IGenericRepository<CompanyPartner> _CompanyRepository;
        private IGenericRepository<HiringRequest> _RequestRepository;
        private IGenericRepository<Interview> _InterviewRepository;
        private IGenericRepository<Gender> _GenderRepository;
        private IGenericRepository<EmploymentType> _EmploymentTypeRepository;
        private IGenericRepository<Transaction> _TransactionRepository;
        private IGenericRepository<Education> _EducationRepository;
        private IGenericRepository<ProfessionalExperience> _ProfessionalExperienceRepository;
        private IGenericRepository<Notification> _NotificationRepository;
        private IGenericRepository<NotificationType> _NotificationTypeRepository;
        private IGenericRepository<UserNotification> _UserNotificationRepository;
        private IGenericRepository<UserDevice> _UserDeviceRepository;
        private IGenericRepository<Project> _ProjectRepository;
        private IGenericRepository<HiredDeveloper> _HiredDeveloperRepository;
        private IGenericRepository<Contract> _ContractRepository;
        private IGenericRepository<ProjectType> _ProjectTypeRepository;
        private IGenericRepository<PayPeriod> _PayPeriodRepository;
        private IGenericRepository<PaySlip> _PaySlipRepository;
        private IGenericRepository<WorkLog> _WorkLogRepository;
        private IGenericRepository<Report> _ReportRepository;
        private IGenericRepository<ReportType> _ReportTypeRepository;

        public UnitOfWork(WeHireDBContext context)
        {
            _context = context;
        }

        public IGenericRepository<User> UserRepository => _userRepository ??= new GenericRepository<User>(_context);
        public IGenericRepository<Developer> DeveloperRepository => _DeveloperRepository ??= new GenericRepository<Developer>(_context);
        public IGenericRepository<Role> RoleRepository => _RoleRepository ??= new GenericRepository<Role>(_context);
        public IGenericRepository<Level> LevelRepository => _LevelRepository ??= new GenericRepository<Level>(_context);
        public IGenericRepository<Skill> SkillRepository => _SkillRepository ??= new GenericRepository<Skill>(_context);
        public IGenericRepository<Domain.Entities.Type> TypeRepository => _TypeRepository ??= new GenericRepository<Domain.Entities.Type>(_context);
        public IGenericRepository<CompanyPartner> CompanyRepository => _CompanyRepository ??= new GenericRepository<CompanyPartner>(_context);
        public IGenericRepository<HiringRequest> RequestRepository => _RequestRepository ??= new GenericRepository<HiringRequest>(_context);
        public IGenericRepository<Interview> InterviewRepository => _InterviewRepository ??= new GenericRepository<Interview>(_context);
        public IGenericRepository<Gender> GenderRepository => _GenderRepository ??= new GenericRepository<Gender>(_context);
        public IGenericRepository<EmploymentType> EmploymentTypeRepository => _EmploymentTypeRepository ??= new GenericRepository<EmploymentType>(_context);
        public IGenericRepository<Transaction> TransactionRepository => _TransactionRepository ??= new GenericRepository<Transaction>(_context);
        public IGenericRepository<Education> EducationRepository => _EducationRepository ??= new GenericRepository<Education>(_context);
        public IGenericRepository<ProfessionalExperience> ProfessionalExperienceRepository => _ProfessionalExperienceRepository ??= new GenericRepository<ProfessionalExperience>(_context);
        public IGenericRepository<Notification> NotificationRepository => _NotificationRepository ??= new GenericRepository<Notification>(_context);
        public IGenericRepository<NotificationType> NotificationTypeRepository => _NotificationTypeRepository ??= new GenericRepository<NotificationType>(_context);
        public IGenericRepository<UserNotification> UserNotificationRepository => _UserNotificationRepository ??= new GenericRepository<UserNotification>(_context);
        public IGenericRepository<UserDevice> UserDeviceRepository => _UserDeviceRepository ??= new GenericRepository<UserDevice>(_context);
        public IGenericRepository<Project> ProjectRepository => _ProjectRepository ??= new GenericRepository<Project>(_context);
        public IGenericRepository<HiredDeveloper> HiredDeveloperRepository => _HiredDeveloperRepository ??= new GenericRepository<HiredDeveloper>(_context);
        public IGenericRepository<Contract> ContractRepository => _ContractRepository ??= new GenericRepository<Contract>(_context);
        public IGenericRepository<ProjectType> ProjectTypeRepository => _ProjectTypeRepository ??= new GenericRepository<ProjectType>(_context);
        public IGenericRepository<PayPeriod> PayPeriodRepository => _PayPeriodRepository ??= new GenericRepository<PayPeriod>(_context);
        public IGenericRepository<PaySlip> PaySlipRepository => _PaySlipRepository ??= new GenericRepository<PaySlip>(_context);
        public IGenericRepository<WorkLog> WorkLogRepository => _WorkLogRepository ??= new GenericRepository<WorkLog>(_context);
        public IGenericRepository<Report> ReportRepository => _ReportRepository ??= new GenericRepository<Report>(_context);
        public IGenericRepository<ReportType> ReportTypeRepository => _ReportTypeRepository ??= new GenericRepository<ReportType>(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public IDbTransaction BeginTransaction()
        {
            var _transaction = _context.Database.BeginTransaction();
            return _transaction.GetDbTransaction();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
