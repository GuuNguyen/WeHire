using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Dashboard
{
    public class AccountDashboard
    {
        public int TotalUser { get; set; }
        public int TotalAdmin { get; set; }
        public int TotalManager { get; set; }
        public int TotalStaff { get; set; }
        public int TotalDeveloper { get; set; }
        public int TotalHumanResource { get; set; }
    }
}
