using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Interview;
using WeHire.Domain.Entities;

namespace WeHire.Infrastructure.Services.GoogleCalendarServices
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        public async Task<Event> CreateGoogleCalendar(CreateGoogleCalendarDTO request)
        {
            string ApplicationName = "Google Canlendar Api";
            UserCredential credential;

            using (var stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "Cre", "cre.json"), FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { CalendarService.Scope.Calendar },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            var services = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            Event eventCalendar = new Event()
            {
                Summary = request.Summary,
                Location = request.Location,
                Start = new EventDateTime
                {
                    DateTime = request.Start,
                    TimeZone = "UTC"
                },
                End = new EventDateTime
                {
                    DateTime = request.End,
                    TimeZone = "UTC"
                },
                Description = request.Description,
                Visibility = "public",
            };

            var meetLink = "https://meet.google.com/your-meet-id";

            eventCalendar.ConferenceData = new ConferenceData
            {
                CreateRequest = new CreateConferenceRequest
                {
                    RequestId = Guid.NewGuid().ToString()
                },
                ConferenceSolution = new ConferenceSolution
                {
                    Key = new ConferenceSolutionKey
                    {
                        Type = "hangoutsMeet",
                    },
                },
                EntryPoints = new List<EntryPoint>
                {
                    new EntryPoint
                    {
                        EntryPointType = "video",
                        Uri = meetLink
                    }
                }
            };

            var eventRequest = services.Events.Insert(eventCalendar, "primary");
            var requestCreate = await eventRequest.ExecuteAsync();
            Console.WriteLine("Meeting created. Link: " + requestCreate.HtmlLink);
            return requestCreate;
        }
    }
}
