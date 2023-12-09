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
        public virtual DbSet<PayPeriod> PayPeriods { get; set; }
        public virtual DbSet<PaySlip> PaySlips { get; set; }
        public virtual DbSet<ProfessionalExperience> ProfessionalExperiences { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectType> ProjectTypes { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<ReportType> ReportTypes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
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
                optionsBuilder.UseSqlServer("server = wehiredb.cfe5z7p8gf69.ap-southeast-2.rds.amazonaws.com,1433; database = WeHireDB;uid=admin;pwd=wehiredatabase;TrustServerCertificate=True;");
                //optionsBuilder.UseSqlServer("server = (local); database = WeHireDB;uid=sa;pwd=1;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<CompanyPartner>(entity =>
            {
                entity.HasKey(e => e.CompanyId)
                    .HasName("PK__CompanyP__2D971CACC5521682");

                entity.ToTable("CompanyPartner");

                entity.HasIndex(e => e.PhoneNumber, "UQ__CompanyP__85FB4E381C1F5962")
                    .IsUnique();

                entity.HasIndex(e => e.CompanyEmail, "UQ__CompanyP__A1DB68DB6C6288B9")
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

                entity.HasIndex(e => e.ContractCode, "UQ__Contract__CBECF833172B71B1")
                    .IsUnique();

                entity.Property(e => e.BasicSalary).HasColumnType("money");

                entity.Property(e => e.ContractCode).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateSigned).HasColumnType("date");

                entity.Property(e => e.EmployementType).HasMaxLength(50);

                entity.Property(e => e.FromDate).HasColumnType("date");

                entity.Property(e => e.LegalRepresentation).HasMaxLength(50);

                entity.Property(e => e.LegalRepresentationPosition).HasMaxLength(50);

                entity.Property(e => e.OvertimePayMultiplier).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.ToDate).HasColumnType("date");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Developer>(entity =>
            {
                entity.ToTable("Developer");

                entity.HasIndex(e => e.CodeName, "UQ__Develope__404488D515B84060")
                    .IsUnique();

                entity.Property(e => e.AverageSalary).HasColumnType("money");

                entity.Property(e => e.CodeName).HasMaxLength(50);

                entity.Property(e => e.Summary).HasMaxLength(2000);

                entity.HasOne(d => d.EmploymentType)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.EmploymentTypeId)
                    .HasConstraintName("FK__Developer__Emplo__7C4F7684");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.GenderId)
                    .HasConstraintName("FK__Developer__Gende__7B5B524B");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.LevelId)
                    .HasConstraintName("FK__Developer__Level__7A672E12");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Developer__UserI__797309D9");
            });

            modelBuilder.Entity<DeveloperSkill>(entity =>
            {
                entity.HasKey(e => new { e.DeveloperId, e.SkillId })
                    .HasName("PK__Develope__B3F245E974A68994");

                entity.ToTable("DeveloperSkill");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.DeveloperSkills)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Devel__282DF8C2");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.DeveloperSkills)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Skill__29221CFB");
            });

            modelBuilder.Entity<DeveloperType>(entity =>
            {
                entity.HasKey(e => new { e.DeveloperId, e.TypeId })
                    .HasName("PK__Develope__9B1EBCCA865BEEC6");

                entity.ToTable("DeveloperType");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.DeveloperTypes)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Devel__2BFE89A6");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.DeveloperTypes)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__TypeI__2CF2ADDF");
            });

            modelBuilder.Entity<Education>(entity =>
            {
                entity.ToTable("Education");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.MajorName).HasMaxLength(50);

                entity.Property(e => e.SchoolName).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.Educations)
                    .HasForeignKey(d => d.DeveloperId)
                    .HasConstraintName("FK__Education__Devel__160F4887");
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

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.HiredDevelopers)
                    .HasForeignKey(d => d.ContractId)
                    .HasConstraintName("FK__HiredDeve__Contr__03F0984C");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.HiredDevelopers)
                    .HasForeignKey(d => d.DeveloperId)
                    .HasConstraintName("FK__HiredDeve__Devel__04E4BC85");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.HiredDevelopers)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK__HiredDeve__Proje__02084FDA");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.HiredDevelopers)
                    .HasForeignKey(d => d.RequestId)
                    .HasConstraintName("FK__HiredDeve__Reque__02FC7413");
            });

            modelBuilder.Entity<HiringRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId)
                    .HasName("PK__HiringRe__33A8517A3FF935EC");

                entity.ToTable("HiringRequest");

                entity.HasIndex(e => e.RequestCode, "UQ__HiringRe__CBAB82F67E7214DD")
                    .IsUnique();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Duration).HasColumnType("datetime");

                entity.Property(e => e.ExpiredAt).HasColumnType("datetime");

                entity.Property(e => e.JobDescription).HasMaxLength(4000);

                entity.Property(e => e.JobTitle).HasMaxLength(200);

                entity.Property(e => e.RejectionReason).HasMaxLength(4000);

                entity.Property(e => e.RequestCode).HasMaxLength(50);

                entity.Property(e => e.SalaryPerDev).HasColumnType("money");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK__HiringReq__Compa__6FE99F9F");

                entity.HasOne(d => d.EmploymentType)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.EmploymentTypeId)
                    .HasConstraintName("FK__HiringReq__Emplo__73BA3083");

                entity.HasOne(d => d.LevelRequire)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.LevelRequireId)
                    .HasConstraintName("FK__HiringReq__Level__72C60C4A");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK__HiringReq__Proje__70DDC3D8");

                entity.HasOne(d => d.TypeRequire)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.TypeRequireId)
                    .HasConstraintName("FK__HiringReq__TypeR__71D1E811");
            });

            modelBuilder.Entity<Interview>(entity =>
            {
                entity.ToTable("Interview");

                entity.HasIndex(e => e.EventId, "UQ__Intervie__7944C8118FFAFFF8")
                    .IsUnique();

                entity.HasIndex(e => e.InterviewCode, "UQ__Intervie__D3D4CCDCD4B0D7D9")
                    .IsUnique();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateOfInterview).HasColumnType("date");

                entity.Property(e => e.EventId).HasMaxLength(1000);

                entity.Property(e => e.InterviewCode).HasMaxLength(50);

                entity.Property(e => e.MeetingUrl).HasMaxLength(2000);

                entity.Property(e => e.RejectionReason).HasMaxLength(4000);

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.HasOne(d => d.HiredDeveloper)
                    .WithMany(p => p.Interviews)
                    .HasForeignKey(d => d.HiredDeveloperId)
                    .HasConstraintName("FK__Interview__Hired__1DB06A4F");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.Interviews)
                    .HasForeignKey(d => d.RequestId)
                    .HasConstraintName("FK__Interview__Reque__1EA48E88");
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
                    .HasName("PK__Notifica__54F5A3010D300C8B");

                entity.ToTable("NotificationType");

                entity.Property(e => e.NotiTypeName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<PayPeriod>(entity =>
            {
                entity.ToTable("PayPeriod");

                entity.HasIndex(e => e.PayPeriodCode, "UQ__PayPerio__DE9A3945C3C3775B")
                    .IsUnique();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.PayPeriodCode).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.TotalAmount).HasColumnType("money");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.PayPeriods)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK__PayPeriod__Proje__08B54D69");
            });

            modelBuilder.Entity<PaySlip>(entity =>
            {
                entity.ToTable("PaySlip");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.TotalActualWorkedHours).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.TotalEarnings).HasColumnType("money");

                entity.Property(e => e.TotalOvertimeHours).HasColumnType("decimal(5, 2)");

                entity.HasOne(d => d.HiredDeveloper)
                    .WithMany(p => p.PaySlips)
                    .HasForeignKey(d => d.HiredDeveloperId)
                    .HasConstraintName("FK__PaySlip__HiredDe__0C85DE4D");

                entity.HasOne(d => d.PayPeriod)
                    .WithMany(p => p.PaySlips)
                    .HasForeignKey(d => d.PayPeriodId)
                    .HasConstraintName("FK__PaySlip__PayPeri__0B91BA14");
            });

            modelBuilder.Entity<ProfessionalExperience>(entity =>
            {
                entity.ToTable("ProfessionalExperience");

                entity.Property(e => e.CompanyName).HasMaxLength(50);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.JobName).HasMaxLength(50);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.ProfessionalExperiences)
                    .HasForeignKey(d => d.DeveloperId)
                    .HasConstraintName("FK__Professio__Devel__18EBB532");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Project");

                entity.HasIndex(e => e.ProjectCode, "UQ__Project__2F3A49483E64037A")
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

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Report");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.ReportTitle).HasMaxLength(200);

                entity.HasOne(d => d.HiredDeveloper)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.HiredDeveloperId)
                    .HasConstraintName("FK__Report__HiredDev__245D67DE");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK__Report__ProjectI__25518C17");

                entity.HasOne(d => d.ReportType)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.ReportTypeId)
                    .HasConstraintName("FK__Report__ReportTy__236943A5");
            });

            modelBuilder.Entity<ReportType>(entity =>
            {
                entity.ToTable("ReportType");

                entity.Property(e => e.ReportTypeTitle).HasMaxLength(200);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
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
                    .HasName("PK__SkillReq__5E525862394E7EA8");

                entity.ToTable("SkillRequire");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.SkillRequires)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SkillRequ__Reque__2FCF1A8A");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.SkillRequires)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SkillRequ__Skill__30C33EC3");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("Transaction");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PayPalTransactionId)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PayPalTransactionID");

                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

                entity.Property(e => e.State).HasMaxLength(50);

                entity.Property(e => e.Timestamp).HasColumnType("datetime");

                entity.HasOne(d => d.PayPeriod)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.PayPeriodId)
                    .HasConstraintName("FK__Transacti__PayPe__123EB7A3");

                entity.HasOne(d => d.Payer)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.PayerId)
                    .HasConstraintName("FK__Transacti__Payer__1332DBDC");
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

                entity.HasIndex(e => e.PhoneNumber, "UQ__User__85FB4E38D4E6FEBD")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__User__A9D10534AA7B1A04")
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
                    .HasName("PK__UserNoti__F1B7A2D61A87D868");

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

                entity.Property(e => e.WorkDate).HasColumnType("date");

                entity.HasOne(d => d.PaySlip)
                    .WithMany(p => p.WorkLogs)
                    .HasForeignKey(d => d.PaySlipId)
                    .HasConstraintName("FK__WorkLog__PaySlip__0F624AF8");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
