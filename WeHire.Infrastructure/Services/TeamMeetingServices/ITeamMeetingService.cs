using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Infrastructure.Services.TeamMeetingServices
{
    public interface ITeamMeetingService
    {
        //Task<TokenResponse> GetAccessTokenAsync();
        //Task<MeetingResponse> CreateMeetingAsync();
        public Task<Event> CreateOnlineMeetingService(OnlineMeetingModel model);
    }
}
