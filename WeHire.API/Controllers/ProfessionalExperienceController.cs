using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Infrastructure.Services.ProfessionalExperienceServices;
using WeHire.Application.DTOs.ProfessionalExperience;
using WeHire.Application.Utilities.Helper.Pagination;

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

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<GetPEByAdmin>>), StatusCodes.Status200OK)]
        public IActionResult GetProfessionExperience([FromQuery] PagingQuery query)
        {
            var result = _professionalExperienceService.GetProfessionalExperiencesAsync(query);
            return Ok(new ApiResponse<List<GetPEByAdmin>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("{developerId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetProfessionalExperience>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfessionExperienceByIdAsync(int developerId)
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
        public async Task<IActionResult> CreateProfessionExperienceAsync(CreateProfessionalExperience requestBody)
        {
            var result = await _professionalExperienceService.CreateProfessionalExperienceAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetProfessionalExperience>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [HttpPut("{professionalId}")]
        [ProducesResponseType(typeof(ApiResponse<GetProfessionalExperience>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProfessionExperienceAsync(int professionalId, UpdateProfessionalExperience requestBody)
        {
            var result = await _professionalExperienceService.UpdateProfessionalExperienceAsync(professionalId, requestBody);

            return Ok(new ApiResponse<GetProfessionalExperience>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpDelete("{professionalId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteProfessionExperienceAsync(int professionalId)
        {
            await _professionalExperienceService.DeleteProfessionalExperienceAsync(professionalId);

            return NoContent();
        }
    }
}
