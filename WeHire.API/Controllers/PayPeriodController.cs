using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Infrastructure.Services.PayPeriodServices;
using WeHire.Infrastructure.Services.ExcelServices;
using WeHire.Application.DTOs.File;
using WeHire.Application.DTOs.PayPeriod;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayPeriodController : ControllerBase
    {
        private readonly IPayPeriodService _payPeriodService;
        private readonly IExcelService _excelService;

        public PayPeriodController(IPayPeriodService payPeriodService, IExcelService excelService)
        {
            _payPeriodService = payPeriodService;
            _excelService = excelService;
        }

        [HttpGet("{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<GetPayPeriodBill>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPayPeriodByProject(int projectId, DateTime inputDate)
        {
            var result = await _payPeriodService.GetPayPeriodByProject(projectId, inputDate);

            return Ok(new ApiResponse<GetPayPeriodBill>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("PayPeriodDuration/{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<List<PayPeriodInMonth>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPayPeriodDuration(int projectId)
        {
            var result = await _payPeriodService.GetPayPeriodsInProjectDurationAsync(projectId);

            return Ok(new ApiResponse<List<PayPeriodInMonth>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("ExportToExcel/{projectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportToExcelAsync(int projectId, DateTime inputDate)
        {
            var excelFile = await _payPeriodService.GeneratePayPeriodExcelFile(projectId, inputDate);
            return File(excelFile.ExcelByteArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFile.FileName + ".xlsx");
        }

        [HttpPost("ImportExcel/{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
        public async Task<IActionResult> ImportExcel(int projectId, [FromForm] FileDTO excelFile)
        {
            if (excelFile == null || excelFile.File == null || excelFile.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var fileExtension = Path.GetExtension(excelFile.File.FileName).ToLowerInvariant();

            if (fileExtension != ".xlsx" && fileExtension != ".xls")
            {
                return BadRequest("Please upload a valid Excel file.");
            }
            var importExcelModel = _excelService.ImportExcelFile(excelFile.File);

            var month = await _payPeriodService.InsertPayPeriodFromExcel(projectId, importExcelModel);

            return Created(string.Empty, new ApiResponse<string>()
            {
                Code = StatusCodes.Status201Created,
                Data = month
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetPayPeriodModel>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePayPeriod(CreatePayPeriodModel requestBody)
        {           
            var result = await _payPeriodService.CreatePayPeriod(requestBody);

            return Created(string.Empty, new ApiResponse<GetPayPeriodModel>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }
    }
}
