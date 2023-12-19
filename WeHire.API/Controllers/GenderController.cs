using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Application.DTOs.CompanyPartner;
using WeHire.Application.Services.GenderServices;
using WeHire.Application.DTOs.Gender;
using Microsoft.AspNetCore.Authorization;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenderController : ControllerBase
    {
        private readonly IGenderService _genderService;

        public GenderController(IGenderService genderService)
        {
            _genderService = genderService;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<GetGenderDTO>>), StatusCodes.Status200OK)]
        public IActionResult GetAllGender()
        {
            var result = _genderService.GetAllGender();

            return Ok(new ApiResponse<List<GetGenderDTO>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result,
            });
        }
    }
}
