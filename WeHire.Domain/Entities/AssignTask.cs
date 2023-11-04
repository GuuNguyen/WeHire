using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class AssignTask
    {
        public AssignTask()
        {
            DeveloperTaskAssignments = new HashSet<DeveloperTaskAssignment>();
        }

        public int TaskId { get; set; }
        public string TaskTitle { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public int? NumOfDeveloper { get; set; }
        public DateTime? CreateAt { get; set; }
        public string RejectionReason { get; set; }
        public int Status { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<DeveloperTaskAssignment> DeveloperTaskAssignments { get; set; }
    }
}
