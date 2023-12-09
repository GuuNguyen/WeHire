using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.EmploymentType;
using WeHire.Application.Services.EmploymentTypeServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmploymentTypeController : ControllerBase
    {
        private readonly IEmploymentTypeService _employmentTypeService;

        public EmploymentTypeController(IEmploymentTypeService employmentTypeService)
        {
            _employmentTypeService = employmentTypeService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<GetEmploymentTypeDTO>>), StatusCodes.Status200OK)]
        public IActionResult GetAllEmploymentType()
        {
            var result = _employmentTypeService.GetAllEmployments();
            return Ok(new ApiResponse<List<GetEmploymentTypeDTO>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
