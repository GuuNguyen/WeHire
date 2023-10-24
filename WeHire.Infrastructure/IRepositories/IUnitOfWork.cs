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
        IGenericRepository<Cv> CvRepository { get; }
        IGenericRepository<SelectedDev> SelectedDevRepository { get; }
        IGenericRepository<Interview> InterviewRepository { get; }
        IGenericRepository<AssignTask> AssignTaskRepository { get; }
        IGenericRepository<DeveloperTaskAssignment> DeveloperTaskAssignmentRepository { get; }
        IGenericRepository<Gender> GenderRepository { get; }
        IGenericRepository<EmploymentType> EmploymentTypeRepository { get; }
        IGenericRepository<ScheduleType> ScheduleTypeRepository { get; }
        IGenericRepository<Agreement> AgreementRepository { get; }
        IGenericRepository<Transaction> TransactionRepository { get; }
        IGenericRepository<Education> EducationRepository { get; }
        IGenericRepository<ProfessionalExperience> ProfessionalExperienceRepository { get; }

        IDbTransaction BeginTransaction(); 
        Task SaveChangesAsync();
    }
}
