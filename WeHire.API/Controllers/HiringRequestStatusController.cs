using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Infrastructure.Services.RequestStatusServices;
using static WeHire.Application.DTOs.HiringRequest.ChangeStatusDTO;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HiringRequestStatusController : ControllerBase
    {
        private readonly IRequestStatusService _requestStatusService;

        public HiringRequestStatusController(IRequestStatusService requestStatusService)
        {
            _requestStatusService = requestStatusService;
        }

        [HttpPut("ChangeWaitingStatus")]
        [ProducesResponseType(typeof(ApiResponse<GetRequestDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeWaitingStatusAsync(WaitingStatus requestBody)
        {
            var result = await _requestStatusService.HandleWaitingStatusAsync(requestBody);
            return Ok(new ApiResponse<GetRequestDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPut("ExtendDuration")]
        [ProducesResponseType(typeof(ApiResponse<GetRequestDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeExpiredStatusAsync(ExpiredStatus requestBody)
        {
            var result = await _requestStatusService.HandleExpiredStatusAsync(requestBody);
            return Ok(new ApiResponse<GetRequestDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPut("Closed")]
        [ProducesResponseType(typeof(ApiResponse<GetRequestDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CloseRequestAsync(CloseRequestModel requestBody)
        {
            var result = await _requestStatusService.ClosedRequestAsync(requestBody);
            return Ok(new ApiResponse<GetRequestDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
