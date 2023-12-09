using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.TeamMeeting;

namespace WeHire.Application.Services.TeamMeetingServices
{
    public interface ITeamMeetingService
    {
        public Task<TeamMeetingInfo> CreateOnlineMeetingAsync(OnlineMeetingModel model);
        public Task UpdateOnlineMeetingAsync(OnlineMeetingModel model);
        public Task DeleteMeetingAsync(OnlineMeetingModel model);
    }
}
