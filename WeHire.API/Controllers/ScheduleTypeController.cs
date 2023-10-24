using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Application.DTOs.EmploymentType;
using WeHire.Infrastructure.Services.ScheduleTypeServices;
using WeHire.Application.DTOs.ScheduleType;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleTypeController : ControllerBase
    {
        private readonly IScheduleTypeService _scheduleTypeService;

        public ScheduleTypeController(IScheduleTypeService scheduleTypeService)
        {
            _scheduleTypeService = scheduleTypeService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<GetScheduleTypeDTO>>), StatusCodes.Status200OK)]
        public IActionResult GetAllEmploymentType()
        {
            var result = _scheduleTypeService.GetAllScheduleType();
            return Ok(new ApiResponse<List<GetScheduleTypeDTO>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
