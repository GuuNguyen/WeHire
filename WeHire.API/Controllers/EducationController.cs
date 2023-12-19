using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Application.Services.EducationServices;
using WeHire.Application.DTOs.Education;
using WeHire.Application.Utilities.Helper.Pagination;
using Microsoft.AspNetCore.Authorization;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationController : ControllerBase
    {
        private readonly IEducationService _educationService;

        public EducationController(IEducationService educationService)
        {
            _educationService = educationService;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<GetEducationByAdmin>>), StatusCodes.Status200OK)]
        public IActionResult GetEducationByAdminAsync([FromQuery] PagingQuery query)
        {
            var result = _educationService.GetEducationsByAdmin(query);

            return Ok(new ApiResponse<List<GetEducationByAdmin>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("{developerId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetEducationDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEducationByIdAsync(int developerId)
        {
            var result = await _educationService.GetEducationsByDevIdAsync(developerId);
            return Ok(new ApiResponse<List<GetEducationDTO>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetEducationDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateEducationAsync(CreateEducationDTO requestBody)
        {
            var result = await _educationService.CreateEducationAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetEducationDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [Authorize]
        [HttpPut("{educationId}")]
        [ProducesResponseType(typeof(ApiResponse<GetEducationDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateEducationAsync(int educationId, UpdateEducationModel requestBody)
        {
            var result = await _educationService.UpdateEducationAsync(educationId, requestBody);
            return Ok(new ApiResponse<GetEducationDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize]
        [HttpDelete("{educationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteEducationAsync(int educationId)
        {
            await _educationService.DeleteEducationAsync(educationId);
            return NoContent();
        }
    }
}
