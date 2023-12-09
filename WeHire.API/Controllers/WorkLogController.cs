using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Contract;
using WeHire.Application.DTOs.WorkLog;
using WeHire.Application.Services.WorkLogServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkLogController : ControllerBase
    {
        private readonly IWorkLogService _workLogService;

        public WorkLogController(IWorkLogService workLogService)
        {
            _workLogService = workLogService;
        }

        [Authorize(Roles = "Admin, Manager, HR")]
        [HttpGet("ByPaySlip/{paySlipId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetWorkLogModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWorkLogByPaySlipId(int paySlipId)
        {
            var result = await _workLogService.GetWorkLogByPaySlipIdAsync(paySlipId);
            return Ok(new ApiResponse<List<GetWorkLogModel>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize(Roles = "Admin, Manager, HR")]
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<WorkLogResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateWorkLog(UpdateWorkLogModel requestBody)
        {
            var result = await _workLogService.UpdateWorkLogAsync(requestBody);
            return Ok(new ApiResponse<WorkLogResponseModel>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
