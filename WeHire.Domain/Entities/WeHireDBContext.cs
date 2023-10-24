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

        public virtual DbSet<Agreement> Agreements { get; set; }
        public virtual DbSet<AssignTask> AssignTasks { get; set; }
        public virtual DbSet<CompanyPartner> CompanyPartners { get; set; }
        public virtual DbSet<Cv> Cvs { get; set; }
        public virtual DbSet<Developer> Developers { get; set; }
        public virtual DbSet<DeveloperSkill> DeveloperSkills { get; set; }
        public virtual DbSet<DeveloperTaskAssignment> DeveloperTaskAssignments { get; set; }
        public virtual DbSet<DeveloperType> DeveloperTypes { get; set; }
        public virtual DbSet<Education> Educations { get; set; }
        public virtual DbSet<EmploymentType> EmploymentTypes { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<HiringRequest> HiringRequests { get; set; }
        public virtual DbSet<Interview> Interviews { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<NotificationType> NotificationTypes { get; set; }
        public virtual DbSet<ProfessionalExperience> ProfessionalExperiences { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<ReportType> ReportTypes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<ScheduleType> ScheduleTypes { get; set; }
        public virtual DbSet<SelectedDev> SelectedDevs { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<SkillRequire> SkillRequires { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Type> Types { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server = (local); database = WeHireDB;uid=sa;pwd=1;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Agreement>(entity =>
            {
                entity.ToTable("Agreement");

                entity.Property(e => e.CommissionRate).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.CommissionStructure).HasColumnType("text");

                entity.Property(e => e.CompanyPartnerName).HasMaxLength(255);

                entity.Property(e => e.CompanyPartnerSignature).HasMaxLength(255);

                entity.Property(e => e.ConfidentialInformation).HasColumnType("text");

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.Property(e => e.DateSigned).HasColumnType("date");

                entity.Property(e => e.EffectiveDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentPeriod).HasColumnType("datetime");

                entity.Property(e => e.ServicesProvided).HasColumnType("text");

                entity.Property(e => e.TermsAndConditions).HasColumnType("text");

                entity.Property(e => e.TotalCommission).HasColumnType("money");

                entity.Property(e => e.WeHireSignature).HasMaxLength(255);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.Agreements)
                    .HasForeignKey(d => d.RequestId)
                    .HasConstraintName("FK__Agreement__Reque__74AE54BC");
            });

            modelBuilder.Entity<AssignTask>(entity =>
            {
                entity.HasKey(e => e.TaskId)
                    .HasName("PK__AssignTa__7C6949B11CAEA875");

                entity.ToTable("AssignTask");

                entity.Property(e => e.Deadline).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.TaskTitle)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AssignTasks)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__AssignTas__UserI__0D7A0286");
            });

            modelBuilder.Entity<CompanyPartner>(entity =>
            {
                entity.HasKey(e => e.CompanyId)
                    .HasName("PK__CompanyP__2D971CACEFBE92CA");

                entity.ToTable("CompanyPartner");

                entity.HasIndex(e => e.PhoneNumber, "UQ__CompanyP__85FB4E380FE86AE0")
                    .IsUnique();

                entity.HasIndex(e => e.CompanyEmail, "UQ__CompanyP__A1DB68DBD35573AF")
                    .IsUnique();

                entity.Property(e => e.Address).HasMaxLength(300);

                entity.Property(e => e.CompanyEmail)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyImage).HasMaxLength(4000);

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CompanyPartners)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__CompanyPa__UserI__5DCAEF64");
            });

            modelBuilder.Entity<Cv>(entity =>
            {
                entity.ToTable("Cv");

                entity.Property(e => e.CvCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Src)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.Cvs)
                    .HasForeignKey(d => d.DeveloperId)
                    .HasConstraintName("FK__Cv__DeveloperId__0A9D95DB");
            });

            modelBuilder.Entity<Developer>(entity =>
            {
                entity.ToTable("Developer");

                entity.HasIndex(e => e.CodeName, "UQ__Develope__404488D5317905BF")
                    .IsUnique();

                entity.Property(e => e.AverageSalary).HasColumnType("money");

                entity.Property(e => e.CodeName).HasMaxLength(50);

                entity.Property(e => e.Summary).HasMaxLength(2000);

                entity.HasOne(d => d.EmploymentType)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.EmploymentTypeId)
                    .HasConstraintName("FK__Developer__Emplo__02084FDA");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.GenderId)
                    .HasConstraintName("FK__Developer__Gende__00200768");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.LevelId)
                    .HasConstraintName("FK__Developer__Level__7F2BE32F");

                entity.HasOne(d => d.ScheduleType)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.ScheduleTypeId)
                    .HasConstraintName("FK__Developer__Sched__01142BA1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Developer__UserI__7E37BEF6");
            });

            modelBuilder.Entity<DeveloperSkill>(entity =>
            {
                entity.HasKey(e => new { e.DeveloperId, e.SkillId })
                    .HasName("PK__Develope__B3F245E92C22D06E");

                entity.ToTable("DeveloperSkill");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.DeveloperSkills)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Devel__18EBB532");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.DeveloperSkills)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Skill__19DFD96B");
            });

            modelBuilder.Entity<DeveloperTaskAssignment>(entity =>
            {
                entity.HasKey(e => new { e.DeveloperId, e.TaskId })
                    .HasName("PK__Develope__D9CED86A90BE2CD0");

                entity.ToTable("DeveloperTaskAssignment");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.DeveloperTaskAssignments)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Devel__10566F31");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.DeveloperTaskAssignments)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__TaskI__114A936A");
            });

            modelBuilder.Entity<DeveloperType>(entity =>
            {
                entity.HasKey(e => new { e.DeveloperId, e.TypeId })
                    .HasName("PK__Develope__9B1EBCCA1ADAA208");

                entity.ToTable("DeveloperType");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.DeveloperTypes)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Devel__1CBC4616");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.DeveloperTypes)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__TypeI__1DB06A4F");
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
                    .HasConstraintName("FK__Education__Devel__04E4BC85");
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

            modelBuilder.Entity<HiringRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId)
                    .HasName("PK__HiringRe__33A8517AF3E2B751");

                entity.ToTable("HiringRequest");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Duration).HasColumnType("datetime");

                entity.Property(e => e.JobDescription).HasMaxLength(4000);

                entity.Property(e => e.JobTitle).HasMaxLength(200);

                entity.Property(e => e.RejectionReason).HasMaxLength(4000);

                entity.Property(e => e.SalaryPerDev).HasColumnType("money");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK__HiringReq__Compa__6E01572D");

                entity.HasOne(d => d.EmploymentType)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.EmploymentTypeId)
                    .HasConstraintName("FK__HiringReq__Emplo__71D1E811");

                entity.HasOne(d => d.LevelRequire)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.LevelRequireId)
                    .HasConstraintName("FK__HiringReq__Level__6FE99F9F");

                entity.HasOne(d => d.ScheduleType)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.ScheduleTypeId)
                    .HasConstraintName("FK__HiringReq__Sched__70DDC3D8");

                entity.HasOne(d => d.TypeRequire)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.TypeRequireId)
                    .HasConstraintName("FK__HiringReq__TypeR__6EF57B66");
            });

            modelBuilder.Entity<Interview>(entity =>
            {
                entity.ToTable("Interview");

                entity.Property(e => e.DateOfInterview).HasColumnType("date");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.MeetingLink).HasMaxLength(2000);

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.HasOne(d => d.AssignStaff)
                    .WithMany(p => p.InterviewAssignStaffs)
                    .HasForeignKey(d => d.AssignStaffId)
                    .HasConstraintName("FK__Interview__Assig__151B244E");

                entity.HasOne(d => d.Interviewer)
                    .WithMany(p => p.InterviewInterviewers)
                    .HasForeignKey(d => d.InterviewerId)
                    .HasConstraintName("FK__Interview__Inter__14270015");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.Interviews)
                    .HasForeignKey(d => d.RequestId)
                    .HasConstraintName("FK__Interview__Reque__160F4887");
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

                entity.Property(e => e.CreateAt).HasColumnType("datetime");

                entity.HasOne(d => d.NotiType)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.NotiTypeId)
                    .HasConstraintName("FK__Notificat__NotiT__571DF1D5");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.NotificationReceivers)
                    .HasForeignKey(d => d.ReceiverId)
                    .HasConstraintName("FK__Notificat__Recei__5629CD9C");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.NotificationSenders)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK__Notificat__Sende__5535A963");
            });

            modelBuilder.Entity<NotificationType>(entity =>
            {
                entity.HasKey(e => e.NotiTypeId)
                    .HasName("PK__Notifica__54F5A301482B0455");

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
                    .HasConstraintName("FK__Professio__Devel__07C12930");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Report");

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK__Report__CompanyI__619B8048");

                entity.HasOne(d => d.ReportType)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.ReportTypeId)
                    .HasConstraintName("FK__Report__ReportTy__60A75C0F");
            });

            modelBuilder.Entity<ReportType>(entity =>
            {
                entity.ToTable("ReportType");

                entity.Property(e => e.ReportTypeTitle)
                    .IsRequired()
                    .HasMaxLength(100);
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
                    .HasName("PK__Selected__3E48D5B510DF3FA5");

                entity.ToTable("SelectedDev");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.SelectedDevs)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SelectedD__Devel__25518C17");

                entity.HasOne(d => d.Interview)
                    .WithMany(p => p.SelectedDevs)
                    .HasForeignKey(d => d.InterviewId)
                    .HasConstraintName("FK__SelectedD__Inter__2645B050");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.SelectedDevs)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SelectedD__Reque__245D67DE");
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
                    .HasName("PK__SkillReq__5E52586287765B44");

                entity.ToTable("SkillRequire");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.SkillRequires)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SkillRequ__Reque__208CD6FA");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.SkillRequires)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SkillRequ__Skill__2180FB33");
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

                entity.HasOne(d => d.Agreement)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.AgreementId)
                    .HasConstraintName("FK__Transacti__Agree__787EE5A0");

                entity.HasOne(d => d.Payer)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.PayerId)
                    .HasConstraintName("FK__Transacti__Payer__778AC167");
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

                entity.HasIndex(e => e.PhoneNumber, "UQ__User__85FB4E386C49C795")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__User__A9D1053483F408F6")
                    .IsUnique();

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

                entity.Property(e => e.UserImage).HasMaxLength(4000);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__User__RoleId__4F7CD00D");
            });

            modelBuilder.Entity<UserDevice>(entity =>
            {
                entity.ToTable("UserDevice");

                entity.Property(e => e.DeviceToken).HasMaxLength(200);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserDevices)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserDevic__UserI__52593CB8");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
