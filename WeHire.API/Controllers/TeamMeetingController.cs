using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Application.DTOs.CV;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Infrastructure.Services.TeamMeetingServices;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamMeetingController : ControllerBase
    {
        private readonly ITeamMeetingService _teamMeetingService;
        public TeamMeetingController(ITeamMeetingService teamMeetingService)
        {
            _teamMeetingService = teamMeetingService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMeetingObject(OnlineMeetingModel model)
        {
            var result = await _teamMeetingService.CreateOnlineMeetingService(model);

            return Ok(result);
        }
    }
}
