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
        public virtual DbSet<UserNotification> UserNotifications { get; set; }

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
                    .HasConstraintName("FK__Agreement__Reque__778AC167");
            });

            modelBuilder.Entity<AssignTask>(entity =>
            {
                entity.HasKey(e => e.TaskId)
                    .HasName("PK__AssignTa__7C6949B13207F2DA");

                entity.ToTable("AssignTask");

                entity.Property(e => e.Deadline).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.TaskTitle)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AssignTasks)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__AssignTas__UserI__10566F31");
            });

            modelBuilder.Entity<CompanyPartner>(entity =>
            {
                entity.HasKey(e => e.CompanyId)
                    .HasName("PK__CompanyP__2D971CACD9C4E788");

                entity.ToTable("CompanyPartner");

                entity.HasIndex(e => e.PhoneNumber, "UQ__CompanyP__85FB4E381673B2B3")
                    .IsUnique();

                entity.HasIndex(e => e.CompanyEmail, "UQ__CompanyP__A1DB68DB2A422535")
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
                    .HasConstraintName("FK__CompanyPa__UserI__60A75C0F");
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
                    .HasConstraintName("FK__Cv__DeveloperId__0D7A0286");
            });

            modelBuilder.Entity<Developer>(entity =>
            {
                entity.ToTable("Developer");

                entity.HasIndex(e => e.CodeName, "UQ__Develope__404488D5D6568B07")
                    .IsUnique();

                entity.Property(e => e.AverageSalary).HasColumnType("money");

                entity.Property(e => e.CodeName).HasMaxLength(50);

                entity.Property(e => e.Summary).HasMaxLength(2000);

                entity.HasOne(d => d.EmploymentType)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.EmploymentTypeId)
                    .HasConstraintName("FK__Developer__Emplo__04E4BC85");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.GenderId)
                    .HasConstraintName("FK__Developer__Gende__02FC7413");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.LevelId)
                    .HasConstraintName("FK__Developer__Level__02084FDA");

                entity.HasOne(d => d.ScheduleType)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.ScheduleTypeId)
                    .HasConstraintName("FK__Developer__Sched__03F0984C");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Developers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Developer__UserI__01142BA1");
            });

            modelBuilder.Entity<DeveloperSkill>(entity =>
            {
                entity.HasKey(e => new { e.DeveloperId, e.SkillId })
                    .HasName("PK__Develope__B3F245E9C9B93896");

                entity.ToTable("DeveloperSkill");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.DeveloperSkills)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Devel__1BC821DD");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.DeveloperSkills)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Skill__1CBC4616");
            });

            modelBuilder.Entity<DeveloperTaskAssignment>(entity =>
            {
                entity.HasKey(e => new { e.DeveloperId, e.TaskId })
                    .HasName("PK__Develope__D9CED86AB26FE0C0");

                entity.ToTable("DeveloperTaskAssignment");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.DeveloperTaskAssignments)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Devel__1332DBDC");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.DeveloperTaskAssignments)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__TaskI__14270015");
            });

            modelBuilder.Entity<DeveloperType>(entity =>
            {
                entity.HasKey(e => new { e.DeveloperId, e.TypeId })
                    .HasName("PK__Develope__9B1EBCCA53050B96");

                entity.ToTable("DeveloperType");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.DeveloperTypes)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__Devel__1F98B2C1");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.DeveloperTypes)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Developer__TypeI__208CD6FA");
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
                    .HasConstraintName("FK__Education__Devel__07C12930");
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
                    .HasName("PK__HiringRe__33A8517A62A6336B");

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
                    .HasConstraintName("FK__HiringReq__Compa__70DDC3D8");

                entity.HasOne(d => d.EmploymentType)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.EmploymentTypeId)
                    .HasConstraintName("FK__HiringReq__Emplo__74AE54BC");

                entity.HasOne(d => d.LevelRequire)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.LevelRequireId)
                    .HasConstraintName("FK__HiringReq__Level__72C60C4A");

                entity.HasOne(d => d.ScheduleType)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.ScheduleTypeId)
                    .HasConstraintName("FK__HiringReq__Sched__73BA3083");

                entity.HasOne(d => d.TypeRequire)
                    .WithMany(p => p.HiringRequests)
                    .HasForeignKey(d => d.TypeRequireId)
                    .HasConstraintName("FK__HiringReq__TypeR__71D1E811");
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
                    .HasConstraintName("FK__Interview__Assig__17F790F9");

                entity.HasOne(d => d.Interviewer)
                    .WithMany(p => p.InterviewInterviewers)
                    .HasForeignKey(d => d.InterviewerId)
                    .HasConstraintName("FK__Interview__Inter__17036CC0");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.Interviews)
                    .HasForeignKey(d => d.RequestId)
                    .HasConstraintName("FK__Interview__Reque__18EBB532");
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
                    .HasConstraintName("FK__Notificat__NotiT__5629CD9C");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("FK__Notificat__Sende__5535A963");
            });

            modelBuilder.Entity<NotificationType>(entity =>
            {
                entity.HasKey(e => e.NotiTypeId)
                    .HasName("PK__Notifica__54F5A3019BA76A59");

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
                    .HasConstraintName("FK__Professio__Devel__0A9D95DB");
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
                    .HasConstraintName("FK__Report__CompanyI__6477ECF3");

                entity.HasOne(d => d.ReportType)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.ReportTypeId)
                    .HasConstraintName("FK__Report__ReportTy__6383C8BA");
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
                    .HasName("PK__Selected__3E48D5B524C24117");

                entity.ToTable("SelectedDev");

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.SelectedDevs)
                    .HasForeignKey(d => d.DeveloperId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SelectedD__Devel__282DF8C2");

                entity.HasOne(d => d.Interview)
                    .WithMany(p => p.SelectedDevs)
                    .HasForeignKey(d => d.InterviewId)
                    .HasConstraintName("FK__SelectedD__Inter__29221CFB");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.SelectedDevs)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SelectedD__Reque__2739D489");
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
                    .HasName("PK__SkillReq__5E525862512F636C");

                entity.ToTable("SkillRequire");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.SkillRequires)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SkillRequ__Reque__236943A5");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.SkillRequires)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SkillRequ__Skill__245D67DE");
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
                    .HasConstraintName("FK__Transacti__Agree__7B5B524B");

                entity.HasOne(d => d.Payer)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.PayerId)
                    .HasConstraintName("FK__Transacti__Payer__7A672E12");
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

                entity.HasIndex(e => e.PhoneNumber, "UQ__User__85FB4E385DABE57A")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__User__A9D10534BE1254C7")
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

            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.HasKey(e => new { e.NotificationId, e.UserId })
                    .HasName("PK__UserNoti__F1B7A2D6950E14E2");

                entity.ToTable("UserNotification");

                entity.HasOne(d => d.Notification)
                    .WithMany(p => p.UserNotifications)
                    .HasForeignKey(d => d.NotificationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserNotif__Notif__59063A47");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserNotifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserNotif__UserI__59FA5E80");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
