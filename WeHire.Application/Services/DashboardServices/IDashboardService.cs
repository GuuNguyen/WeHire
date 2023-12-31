﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Dashboard;
using WeHire.Application.DTOs.HiringRequest;

namespace WeHire.Application.Services.DashboardServices
{
    public interface IDashboardService
    {
        public Task<DashboardByAdmin> GetDashboardByAdminAsync();
        public Dictionary<DayOfWeek, int> GetProjectDashboard(DateTime dateInWeek);
        public Dictionary<DayOfWeek, int> GetHiringRequestDashboard(DateTime dateInWeek);
        public Task<DashboardByProject> GetDashboardByProjectAsync(int projectId);
        public List<GetListHiringRequest> GetRecentRequest();
    }
}
