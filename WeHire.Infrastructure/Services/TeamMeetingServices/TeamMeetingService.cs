using Azure.Identity;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
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
using static System.Net.WebRequestMethods;

namespace WeHire.Infrastructure.Services.TeamMeetingServices
{
    public class TeamMeetingService : ITeamMeetingService
    {
        private readonly IConfiguration _configuration;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _tenantId;
        private readonly string _scope;
        private readonly string _redirectUrl;

        public TeamMeetingService(IConfiguration configuration)
        {
            _configuration = configuration;
            _clientId = _configuration["TeamMeeting:ClientId"];
            _clientSecret = _configuration["TeamMeeting:ClientSecret"];
            _tenantId = _configuration["TeamMeeting:TenantId"];
            _scope = _configuration["TeamMeeting:Scope"];
            _redirectUrl = _configuration["TeamMeeting:RedirectUrl"];
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

        public async Task<Event> CreateOnlineMeetingService(OnlineMeetingModel model)
        {
            var service = GetGraphService(model.authenCode);

            var requestBody = new Event
            {
                Subject = "Let's go for lunch",
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = "Does next month work for you?",
                },
                Start = new DateTimeTimeZone
                {
                    DateTime = "2023-11-05T12:00:00+07:00",
                    TimeZone = "Asia/Ho_Chi_Minh",
                },
                End = new DateTimeTimeZone
                {
                    DateTime = "2023-11-05T14:00:00+07:00",
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
                            Address = "adelev@contoso.onmicrosoft.com",
                            Name = "Adele Vance",
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
            return result;
        }
    }
}
