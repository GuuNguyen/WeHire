using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Infrastructure.Services.JobPositionServices;
using WeHire.Application.DTOs.JobPosition;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPositionController : ControllerBase
    {
        private readonly IJobPositionService _jobPositionService;

        public JobPositionController(IJobPositionService jobPositionService)
        {
            _jobPositionService = jobPositionService;
        }

        [HttpGet("ByProject/{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetJobPosition>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetJobPositionByProjectId(int projectId)
        {
            var result = await _jobPositionService.GetJobPositionByProjectId(projectId);
            return Ok(new ApiResponse<List<GetJobPosition>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("JobPositionsWithHiringRequest/{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetJpRequestModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetJobPositionWithHiringRequest(int projectId)
        {
            var result = await _jobPositionService.GetJpWithHiringRequest(projectId);
            return Ok(new ApiResponse<List<GetJpRequestModel>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetJobPosition>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateJobPosition(CreateJobPosition requestBody)
        {
            var result = await _jobPositionService.CreateJobPosition(requestBody);
            return Created(string.Empty, new ApiResponse<GetJobPosition>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<GetJobPosition>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateJobPosition(int jobPositionId, UpdateJobPosition requestBody)
        {
            var result = await _jobPositionService.UpdateJobPosition(jobPositionId, requestBody);
            return Ok(new ApiResponse<GetJobPosition>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }


        [HttpDelete("{jobPositionId}")]
        [ProducesResponseType(typeof(ApiResponse<GetJobPosition>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveJobPosition(int jobPositionId)
        {
            var result = await _jobPositionService.DeleteJobPosition(jobPositionId);
            return Ok(new ApiResponse<GetJobPosition>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
