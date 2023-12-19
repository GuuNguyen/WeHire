using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Dashboard
{
    public class DeveloperDashboard
    {
        public int TotalHiredDeveloper { get; set; }
        public int TotalWorkingDeveloper { get; set; }
        public int TotalTerminatedDeveloper { get; set; }
        public int TotalCompletedDeveloper { get; set; }
    }
}
