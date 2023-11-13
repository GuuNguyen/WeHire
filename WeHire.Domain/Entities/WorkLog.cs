using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class WorkLog
    {
        public WorkLog()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int WorkLogId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? HoursWorked { get; set; }
        public decimal? TotalSalary { get; set; }
        public int Status { get; set; }
        public int? HiredDeveloperId { get; set; }

        public virtual HiredDeveloper HiredDeveloper { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
