using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.HiredDeveloper;
using WeHire.Infrastructure.Services.HiredDeveloperServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HiredDeveloperController : ControllerBase
    {
        private readonly IHiredDeveloperService _hiredDeveloperService;

        public HiredDeveloperController(IHiredDeveloperService hiredDeveloperService)
        {
            _hiredDeveloperService = hiredDeveloperService;
        }

        [HttpGet("DevelopersInRequest/{requestId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetMatchingDev>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDevsInRequest(int requestId)
        {
            var result = await _hiredDeveloperService.GetDevsInHiringRequest(requestId);

            return Ok(new ApiResponse<List<GetMatchingDev>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPost("SendDevToHR")]
        [ProducesResponseType(typeof(ApiResponse<List<GetHiredDeveloperModel>>), StatusCodes.Status201Created)]
        public async Task<IActionResult> SendDevToHRAsync(SendDevDTO requestBody)
        {
            var result = await _hiredDeveloperService.SendDevToHR(requestBody);

            return Created(string.Empty, new ApiResponse<List<GetHiredDeveloperModel>>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [HttpPut("RejectDev")]
        [ProducesResponseType(typeof(ApiResponse<GetHiredDeveloperModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectDev(int requestId, int developerId)
        {
            var result = await _hiredDeveloperService.RejectDeveloperAsync(requestId, developerId);

            return Ok(new ApiResponse<GetHiredDeveloperModel>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPut("TerminateFromProject")]
        [ProducesResponseType(typeof(ApiResponse<GetHiredDeveloperModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> TerminateDev(int projectId, int developerId)
        {
            var result = await _hiredDeveloperService.TerminateDeveloperAsync(projectId, developerId);

            return Ok(new ApiResponse<GetHiredDeveloperModel>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
