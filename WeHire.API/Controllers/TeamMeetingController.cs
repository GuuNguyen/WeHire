using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Services.TeamMeetingServices;
using WeHire.Application.DTOs.TeamMeeting;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMeetingObject(OnlineMeetingModel model)
        {
            var result = await _teamMeetingService.CreateOnlineMeetingAsync(model);

            return Ok(result);
        }

        [Authorize]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMeeting(OnlineMeetingModel model)
        {
            await _teamMeetingService.UpdateOnlineMeetingAsync(model);
            return Ok("Update successful!");
        }

        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveMeeting(OnlineMeetingModel model)
        {
            await _teamMeetingService.DeleteMeetingAsync(model);
            return Ok("Delete successful!");
        }
    }
}
