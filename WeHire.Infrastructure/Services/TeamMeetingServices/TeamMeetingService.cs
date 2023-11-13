using Azure.Identity;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.TeamMeeting;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Entity.IRepositories;
using static System.Net.WebRequestMethods;

namespace WeHire.Infrastructure.Services.TeamMeetingServices
{
    public class TeamMeetingService : ITeamMeetingService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _tenantId;
        private readonly string _scope;
        private readonly string _redirectUrl;

        public TeamMeetingService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _clientId = _configuration["TeamMeeting:ClientId"];
            _clientSecret = _configuration["TeamMeeting:ClientSecret"];
            _tenantId = _configuration["TeamMeeting:TenantId"];
            _scope = _configuration["TeamMeeting:Scope"];
            _redirectUrl = _configuration["TeamMeeting:RedirectUrl"];
            _unitOfWork = unitOfWork;
        }

        private GraphServiceClient GetGraphService(string codeAuthencation)
        {
            var scopes = new[] { _scope };

            var tenantId = "common";

            var clientId = _clientId;
            var clientSecret = _clientSecret;

            var options = new AuthorizationCodeCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                RedirectUri = new Uri(_redirectUrl)
            };

            var authCodeCredential = new AuthorizationCodeCredential(
                tenantId, clientId, clientSecret, codeAuthencation, options);

            return new GraphServiceClient(authCodeCredential, scopes);
        }

        public async Task<TeamMeetingInfo> CreateOnlineMeetingService(OnlineMeetingModel model)
        {
            var service = GetGraphService(model.authenCode);

            var interview = await _unitOfWork.InterviewRepository.Get(i => i.InterviewId == model.InterviewId)
                                                                 .Include(i => i.Developer)
                                                                        .ThenInclude(d => d.User)
                                                                 .SingleOrDefaultAsync();
            var developerFullname = $"{interview.Developer.User.FirstName} {interview.Developer.User.LastName}";
            var convertedDate = ConvertDateTime.ConvertDateToStringForMeeting(interview.DateOfInterview);
            var convertedStartTime = ConvertTime.ConvertTimeToShortFormat(interview.StartTime);
            var convertedEndTime = ConvertTime.ConvertTimeToShortFormat(interview.EndTime);
            var startTime = $"{convertedDate}T{convertedStartTime}:00+07:00";
            var endTime = $"{convertedDate}T{convertedEndTime}:00+07:00";

            var requestBody = new Event
            {
                Subject = interview.Title,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = interview.Description,
                },
                Start = new DateTimeTimeZone
                {
                    DateTime = startTime,
                    TimeZone = "Asia/Ho_Chi_Minh",
                },
                End = new DateTimeTimeZone
                {
                    DateTime = endTime,
                    TimeZone = "Asia/Ho_Chi_Minh",
                },
                Location = new Location
                {
                    DisplayName = "Ho Chi Minh City",
                },
                Attendees = new List<Attendee>
                {
                    new Attendee
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = interview.Developer.User.Email,
                            Name = developerFullname,
                        },
                        Type = AttendeeType.Required,
                    },
                },
                AllowNewTimeProposals = true,
                IsOnlineMeeting = true,
                OnlineMeetingProvider = OnlineMeetingProviderType.SkypeForConsumer,
            };
            var result = await service.Me.Events.PostAsync(requestBody, (requestConfiguration) =>
            {
                requestConfiguration.Headers.Add("Prefer", "outlook.timezone=\"Asia/Ho_Chi_Minh\"");
            });

            interview.MeetingUrl = result.OnlineMeetingUrl;
            interview.OutlookUrl = result.WebLink;
            await _unitOfWork.SaveChangesAsync();

            return new TeamMeetingInfo
            {
                OnlineMeetingUrl = result.OnlineMeetingUrl ?? "",
                OutlookUrl = result.WebLink ?? ""
            };
        }
    }
}
