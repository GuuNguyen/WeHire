using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Dashboard
{
    public class HiringRequestDashboard
    {
        public int TotalHiringRequest { get; set; }
        public int TotalSaved { get; set; }
        public int TotalWaitingApproval { get; set; }
        public int TotalInProcess { get; set; }
        public int TotalRejected { get; set; }
        public int TotalCompleted { get; set; }
        public int TotalClosed { get; set; }
        public int TotalExpired { get; set; }
    }
}
