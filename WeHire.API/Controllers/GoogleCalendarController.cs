using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Interview;
using WeHire.Infrastructure.Services.GoogleCalendarServices;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleCalendarController : ControllerBase
    {
        private readonly IGoogleCalendarService _googleCalendarService;

        public GoogleCalendarController(IGoogleCalendarService googleCalendarService)
        {
            _googleCalendarService = googleCalendarService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGoogleCalendar([FromBody] CreateGoogleCalendarDTO request)
        {
            return Ok(await _googleCalendarService.CreateGoogleCalendar(request));
        }
    }
}
