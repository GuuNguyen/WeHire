using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class User
    {
        public User()
        {
            AssignTasks = new HashSet<AssignTask>();
            CompanyPartners = new HashSet<CompanyPartner>();
            Developers = new HashSet<Developer>();
            InterviewAssignStaffs = new HashSet<Interview>();
            InterviewInterviewers = new HashSet<Interview>();
            Notifications = new HashSet<Notification>();
            Transactions = new HashSet<Transaction>();
            UserDevices = new HashSet<UserDevice>();
            UserNotifications = new HashSet<UserNotification>();
        }

        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string UserImage { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int Status { get; set; }
        public int? RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<AssignTask> AssignTasks { get; set; }
        public virtual ICollection<CompanyPartner> CompanyPartners { get; set; }
        public virtual ICollection<Developer> Developers { get; set; }
        public virtual ICollection<Interview> InterviewAssignStaffs { get; set; }
        public virtual ICollection<Interview> InterviewInterviewers { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<UserDevice> UserDevices { get; set; }
        public virtual ICollection<UserNotification> UserNotifications { get; set; }
    }
}
