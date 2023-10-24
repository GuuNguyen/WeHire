using Google.Apis.Calendar.v3.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Interview;
using WeHire.Domain.Entities;

namespace WeHire.Infrastructure.Services.GoogleCalendarServices
{
    public interface IGoogleCalendarService
    {
        public Task<Event> CreateGoogleCalendar(CreateGoogleCalendarDTO request);
    }
}
