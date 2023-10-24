using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Infrastructure.Services.EducationServices;
using WeHire.Application.DTOs.Education;

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

        [HttpGet("{developerId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetEducationDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCvByIdAsync(int developerId)
        {
            var result = await _educationService.GetEducationsByDevIdAsync(developerId);
            return Ok(new ApiResponse<List<GetEducationDTO>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetEducationDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCvAsync(CreateEducationDTO requestBody)
        {
            var result = await _educationService.CreateEducationAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetEducationDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }
    }
}
