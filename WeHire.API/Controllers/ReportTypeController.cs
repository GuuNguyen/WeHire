using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Contract;
using WeHire.Application.DTOs.ReportType;
using WeHire.Application.Services.ReportTypeServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportTypeController : ControllerBase
    {
        private readonly IReportTypeService _reportTypeService;

        public ReportTypeController(IReportTypeService reportTypeService)
        {
            _reportTypeService = reportTypeService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<GetReportTypeModel>>), StatusCodes.Status200OK)]
        public IActionResult GetReportType()
        {
            var result = _reportTypeService.GetReportTypes();
            return Ok(new ApiResponse<List<GetReportTypeModel>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
