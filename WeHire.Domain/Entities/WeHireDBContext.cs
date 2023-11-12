using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class WeHireDBContext : DbContext
    {
        public WeHireDBContext()
        {
        }

        public WeHireDBContext(DbContextOptions<WeHireDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CompanyPartner> CompanyPartners { get; set; }
        public virtual DbSet<Contract> Contracts { get; set; }
        public virtual DbSet<Cv> Cvs { get; set; }
        public virtual DbSet<Developer> Developers { get; set; }
        public virtual DbSet<DeveloperSkill> DeveloperSkills { get; set; }
        public virtual DbSet<DeveloperType> DeveloperTypes { get; set; }
        public virtual DbSet<Education> Educations { get; set; }
        public virtual DbSet<EmploymentType> EmploymentTypes { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<HiredDeveloper> HiredDevelopers { get; set; }
        public virtual DbSet<HiringRequest> HiringRequests { get; set; }
        public virtual DbSet<Interview> Interviews { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }
        public virtual DbSet<ProfessionalExperience> ProfessionalExperiences { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectType> ProjectTypes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<ScheduleType> ScheduleTypes { get; set; }
        public virtual DbSet<SelectedDev> SelectedDevs { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<SkillRequire> SkillRequires { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Type> Types { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }
        public virtual DbSet<UserNotification> UserNotifications { get; set; }
        public virtual DbSet<WorkLog> WorkLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("server = wehiredb.cfe5z7p8gf69.ap-southeast-2.rds.amazonaws.com,1433; database = WeHireDB;uid=admin;pwd=wehiredatabase;TrustServerCertificate=True;");
                optionsBuilder.UseSqlServer("server = (local); database = WeHireDB;uid=sa;pwd=1;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<CompanyPartner>(entity =>
            {
                entity.HasKey(e => e.CompanyId)
                    .HasName("PK__CompanyP__2D971CACA02878C8");

                entity.ToTable("CompanyPartner");

                entity.HasIndex(e => e.PhoneNumber, "UQ__CompanyP__85FB4E380F717DA5")
                    .IsUnique();

                entity.HasIndex(e => e.CompanyEmail, "UQ__CompanyP__A1DB68DB3ACAA98C")
                    .IsUnique();

                entity.Property(e => e.Address).HasMaxLength(300);

                entity.Property(e => e.CompanyEmail)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.FacebookUrl).HasMaxLength(500);

                entity.Property(e => e.LinkedInkUrl).HasMaxLength(500);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CompanyPartners)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__CompanyPa__UserI__5FB337D6");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("Contract");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateSigned).HasColumnType("date");

                entity.Property(e => e.StartWorkingDate).HasColumnType("date");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.HiredDeveloper)
                    .WithMany(p => p.Contracts)
                    .HasForeignKey(d => d.HiredDeveloperId)
                    .HasConstraintName("FK__Contract__HiredD__05D8E0BE");
            });

            modelBuilder.Entity<Cv>(entity =>
            {
                entity.ToTable("Cv");

                entity.HasIndex(e => e.CvCode, "UQ__Cv__D5FFA996A9659A33")
                    .IsUnique();

                entity.Property(e => e.CvCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Src)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.Cvs)
                    .HasForeignKey(d => d.DeveloperId)
                    .HasConstraintName("FK__Cv__DeveloperId__160F4887");
            });

            modelBuilder.Entity<Developer>(entity =>
            {
                entity.ToTable("Developer");

                entity.HasIndex(e => e.CodeName, "UQ__Develope__404488D5723B2747")
                    .IsUnique();

                entity.Property(e => e.AverageSalary).HasColumnType("money");

                entity.Property(e => e.CodeName).HasMaxLength(50);

                entity.Property(e => e.Summary).HasMaxLength(2000);

                entity.HasOne(d => d.EmploymentType)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.EmploymentTypeId)
                    .HasConstraintName("FK__Developer__Emplo__7F2BE32F");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.GenderId)
                    .HasConstraintName("FK__Developer__Gende__7D439ABD");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.LevelId)
                    .HasConstraintName("FK__Developer__Level__7C4F7684");

                entity.HasOne(d => d.ScheduleType)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.ScheduleTypeId)
                    .HasConstraintName("FK__Developer__Sched__7E37BEF6");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Developer__UserI__7B5B524B");
            });

            modelBuilder.Entity<DeveloperSkill>(entity =>
            {
                entity.HasKey(e => new { e.DeveloperId, e.SkillId })
                    .HasName("PK__Develope__B3F245E9A6EC13D8");

                entity.ToTable("DeveloperSkill");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.DeveloperSkills)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Devel__1DB06A4F");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.DeveloperSkills)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Skill__1EA48E88");
            });

            modelBuilder.Entity<DeveloperType>(entity =>
            {
                entity.HasKey(e => new { e.DeveloperId, e.TypeId })
                    .HasName("PK__Develope__9B1EBCCA2881CFE9");

                entity.ToTable("DeveloperType");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.DeveloperTypes)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Devel__2180FB33");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.DeveloperTypes)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__TypeI__22751F6C");
            });

            modelBuilder.Entity<Education>(entity =>
            {
                entity.ToTable("Education");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.MajorName).HasMaxLength(50);

                entity.Property(e => e.SchoolName).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.Educations)
                    .HasForeignKey(d => d.DeveloperId)
                    .HasConstraintName("FK__Education__Devel__0F624AF8");
            });

            modelBuilder.Entity<EmploymentType>(entity =>
            {
                entity.ToTable("EmploymentType");

                entity.Property(e => e.EmploymentTypeName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.ToTable("Gender");

                entity.Property(e => e.GenderName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<HiredDeveloper>(entity =>
            {
                entity.ToTable("HiredDeveloper");

                entity.Property(e => e.Salary).HasColumnType("money");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.HiredDevelopers)
                    .HasForeignKey(d => d.DeveloperId)
                    .HasConstraintName("FK__HiredDeve__Devel__02FC7413");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.HiredDevelopers)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK__HiredDeve__Proje__02084FDA");
            });

            modelBuilder.Entity<HiringRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId)
                    .HasName("PK__HiringRe__33A8517A2C76255E");

                entity.ToTable("HiringRequest");

                entity.HasIndex(e => e.RequestCode, "UQ__HiringRe__CBAB82F6EA8C6A97")
                    .IsUnique();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Duration).HasColumnType("datetime");

                entity.Property(e => e.JobDescription).HasMaxLength(4000);

                entity.Property(e => e.JobTitle).HasMaxLength(200);

                entity.Property(e => e.RejectionReason).HasMaxLength(4000);

                entity.Property(e => e.RequestCode).HasMaxLength(50);

                entity.Property(e => e.SalaryPerDev).HasColumnType("money");

                entity.HasOne(d => d.EmploymentType)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.EmploymentTypeId)
                    .HasConstraintName("FK__HiringReq__Emplo__75A278F5");

                entity.HasOne(d => d.LevelRequire)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.LevelRequireId)
                    .HasConstraintName("FK__HiringReq__Level__73BA3083");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK__HiringReq__Proje__71D1E811");

                entity.HasOne(d => d.ScheduleType)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.ScheduleTypeId)
                    .HasConstraintName("FK__HiringReq__Sched__74AE54BC");

                entity.HasOne(d => d.TypeRequire)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.TypeRequireId)
                    .HasConstraintName("FK__HiringReq__TypeR__72C60C4A");
            });

            modelBuilder.Entity<Interview>(entity =>
            {
                entity.ToTable("Interview");

                entity.HasIndex(e => e.InterviewCode, "UQ__Intervie__D3D4CCDC07CEACFD")
                    .IsUnique();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateOfInterview).HasColumnType("date");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.InterviewCode).HasMaxLength(50);

                entity.Property(e => e.MeetingUrl).HasMaxLength(2000);

                entity.Property(e => e.RejectionReason).HasMaxLength(4000);

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.Interviews)
                    .HasForeignKey(d => d.DeveloperId)
                    .HasConstraintName("FK__Interview__Devel__19DFD96B");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.Interviews)
                    .HasForeignKey(d => d.RequestId)
                    .HasConstraintName("FK__Interview__Reque__1AD3FDA4");
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.ToTable("Level");

                entity.Property(e => e.LevelDescription).HasMaxLength(2000);

                entity.Property(e => e.LevelName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.SenderName).HasMaxLength(50);

                entity.HasOne(d => d.NotiType)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.NotiTypeId)
                    .HasConstraintName("FK__Notificat__NotiT__571DF1D5");
            });

            modelBuilder.Entity<NotificationType>(entity =>
            {
                entity.HasKey(e => e.NotiTypeId)
                    .HasName("PK__Notifica__54F5A301FA8A44CF");

                entity.ToTable("NotificationType");

                entity.Property(e => e.NotiTypeName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<ProfessionalExperience>(entity =>
            {
                entity.ToTable("ProfessionalExperience");

                entity.Property(e => e.CompanyName).HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.JobName).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.ProfessionalExperiences)
                    .HasForeignKey(d => d.DeveloperId)
                    .HasConstraintName("FK__Professio__Devel__123EB7A3");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Project");

                entity.HasIndex(e => e.ProjectCode, "UQ__Project__2F3A49481242483C")
                    .IsUnique();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.ProjectCode).HasMaxLength(50);

                entity.Property(e => e.ProjectName).HasMaxLength(100);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK__Project__Company__6383C8BA");

                entity.HasOne(d => d.ProjectType)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.ProjectTypeId)
                    .HasConstraintName("FK__Project__Project__6477ECF3");
            });

            modelBuilder.Entity<ProjectType>(entity =>
            {
                entity.ToTable("ProjectType");

                entity.Property(e => e.ProjectTypeName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ScheduleType>(entity =>
            {
                entity.ToTable("ScheduleType");

                entity.Property(e => e.ScheduleTypeName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SelectedDev>(entity =>
            {
                entity.HasKey(e => new { e.RequestId, e.DeveloperId })
                    .HasName("PK__Selected__3E48D5B5D729B71A");

                entity.ToTable("SelectedDev");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.SelectedDevs)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SelectedD__Devel__2A164134");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.SelectedDevs)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SelectedD__Reque__29221CFB");
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.ToTable("Skill");

                entity.Property(e => e.SkillDescription).HasMaxLength(2000);

                entity.Property(e => e.SkillName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SkillRequire>(entity =>
            {
                entity.HasKey(e => new { e.RequestId, e.SkillId })
                    .HasName("PK__SkillReq__5E5258627269B8E4");

                entity.ToTable("SkillRequire");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.SkillRequires)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SkillRequ__Reque__25518C17");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.SkillRequires)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SkillRequ__Skill__2645B050");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.PayPalTransactionId)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PayPalTransactionID");

                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

                entity.Property(e => e.State).HasMaxLength(50);

                entity.Property(e => e.Timestamp).HasColumnType("datetime");

                entity.HasOne(d => d.Payer)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.PayerId)
                    .HasConstraintName("FK__Transacti__Payer__0C85DE4D");

                entity.HasOne(d => d.WorkLog)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.WorkLogId)
                    .HasConstraintName("FK__Transacti__WorkL__0B91BA14");
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.ToTable("Type");

                entity.Property(e => e.TypeDescription).HasMaxLength(2000);

                entity.Property(e => e.TypeName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.PhoneNumber, "UQ__User__85FB4E3885DE4966")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__User__A9D10534329E9D13")
                    .IsUnique();

                entity.Property(e => e.ConfirmationCode).HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.RefreshTokenExpiryTime).HasColumnType("datetime");

                entity.Property(e => e.UserImage).HasMaxLength(4000);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__User__RoleId__5165187F");
            });

            modelBuilder.Entity<UserDevice>(entity =>
            {
                entity.ToTable("UserDevice");

                entity.Property(e => e.DeviceToken).HasMaxLength(200);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserDevices)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserDevic__UserI__5441852A");
            });

            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.HasKey(e => new { e.NotificationId, e.UserId })
                    .HasName("PK__UserNoti__F1B7A2D60974F268");

                entity.ToTable("UserNotification");

                entity.HasOne(d => d.Notification)
                    .WithMany(p => p.UserNotifications)
                    .HasForeignKey(d => d.NotificationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserNotif__Notif__59FA5E80");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserNotifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserNotif__UserI__5AEE82B9");
            });

            modelBuilder.Entity<WorkLog>(entity =>
            {
                entity.ToTable("WorkLog");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.TotalSalary).HasColumnType("money");

                entity.HasOne(d => d.HiredDeveloper)
                    .WithMany(p => p.WorkLogs)
                    .HasForeignKey(d => d.HiredDeveloperId)
                    .HasConstraintName("FK__WorkLog__HiredDe__08B54D69");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
