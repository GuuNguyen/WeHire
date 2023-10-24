using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Infrastructure.Services.ProfessionalExperienceServices;
using WeHire.Application.DTOs.ProfessionalExperience;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionalExperienceController : ControllerBase
    {
        private readonly IProfessionalExperienceService _professionalExperienceService;

        public ProfessionalExperienceController(IProfessionalExperienceService professionalExperienceService)
        {
            _professionalExperienceService = professionalExperienceService;
        }

        [HttpGet("{developerId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetProfessionalExperience>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCvByIdAsync(int developerId)
        {
            var result = await _professionalExperienceService.GetProfessionalExperiencesByDevIdAsync(developerId);
            return Ok(new ApiResponse<List<GetProfessionalExperience>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetProfessionalExperience>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCvAsync(CreateProfessionalExperience requestBody)
        {
            var result = await _professionalExperienceService.CreateProfessionalExperienceAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetProfessionalExperience>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }
    }
}
