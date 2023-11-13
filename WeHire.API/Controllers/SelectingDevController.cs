using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Infrastructure.Services.HiringRequestServices;
using WeHire.Infrastructure.Services.SelectingDevServices;
using WeHire.Application.DTOs.SelectingDev;
using WeHire.Application.DTOs.Developer;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SelectingDevController : ControllerBase
    {
        private readonly ISelectingDevService _selectingDevService;

        public SelectingDevController(ISelectingDevService selectingDevService)
        {
            _selectingDevService = selectingDevService;
        }

        [HttpGet("SelectedDevByManager/{requestId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetMatchingDev>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDevsAccepted(int requestId)
        {
            var result = await _selectingDevService.GetSelectedDevsForManager(requestId);

            return Ok(new ApiResponse<List<GetMatchingDev>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("SelectedDevByHR/{requestId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetMatchingDev>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSelectedDevByHR(int requestId)
        {
            var result = await _selectingDevService.GetSelectedDevsForHR(requestId);

            return Ok(new ApiResponse<List<GetMatchingDev>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        //[HttpPost]
        //[ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
        //public async Task<IActionResult> CreateSelectedDevAsync(CreateSelectedDev requestBody)
        //{
        //    var result = await _selectingDevService.CreateSelectDevForRequest(requestBody);

        //    return Created(string.Empty, new ApiResponse<string>()
        //    {
        //        Code = StatusCodes.Status201Created,
        //        Data = result
        //    });
        //}

        [HttpPost("SendDevToHR")]
        [ProducesResponseType(typeof(ApiResponse<List<GetSelectingDevDTO>>), StatusCodes.Status201Created)]
        public async Task<IActionResult> SendDevToHRAsync(SendDevDTO requestBody)
        {
            var result = await _selectingDevService.SendDevToHR(requestBody);

            return Created(string.Empty, new ApiResponse<List<GetSelectingDevDTO>>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [HttpPut("ApprovalByHR")]
        [ProducesResponseType(typeof(ApiResponse<GetSelectingDevDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeStatusApprovalByHR(ChangeSelectingDevStatusDTO requestBody)
        {
            var result = await _selectingDevService.ChangeStatusApprovalByHRAsync(requestBody);

            return Ok(new ApiResponse<GetSelectingDevDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

       
        [HttpPut("Onboarding")]
        [ProducesResponseType(typeof(ApiResponse<GetSelectingDevDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeStatusToOnboarding(int requestId, int developerId)
        {
            var result = await _selectingDevService.ChangeStatusToOnboardingAsync(requestId, developerId);

            return Ok(new ApiResponse<GetSelectingDevDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPut("RejectDev")]
        [ProducesResponseType(typeof(ApiResponse<GetSelectingDevDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RejectDev(int requestId, int developerId)
        {
            var result = await _selectingDevService.RejectDeveloperAsync(requestId, developerId);

            return Ok(new ApiResponse<GetSelectingDevDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
