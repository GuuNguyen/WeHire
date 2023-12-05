using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Contract;
using WeHire.Application.DTOs.Report;
using WeHire.Application.Utilities.Helper.CheckNullProperties;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Infrastructure.Services.ReportServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetReportModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllReport([FromQuery] PagingQuery query, [FromQuery] SearchReportModel searchKey)
        {
            var result = _reportService.GetAllReport(query, searchKey);
            var total = searchKey.AreAllPropertiesNull() ? await _reportService.GetTotalItem()
                                                         : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetReportModel>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }
        
        [HttpGet("ByCompany/{companyId}")]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetReportModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllReportByProject( int companyId,
                                                               [FromQuery] PagingQuery query, 
                                                               [FromQuery] SearchReportModel searchKey)
        {
            var result = _reportService.GetReportByProject(companyId, query, searchKey);
            var total = searchKey.AreAllPropertiesNull() ? await _reportService.GetTotalItem(companyId)
                                                  : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetReportModel>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [HttpGet("{reportId}")]
        [ProducesResponseType(typeof(ApiResponse<GetReportModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetReportById(int reportId)
        {
            var result = await _reportService.GetReportById(reportId);
            return Ok(new ApiResponse<GetReportModel>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetReport>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateReport(CreateReportModel requestBody)
        {
            var result = await _reportService.CreateReport(requestBody);
            return Created(string.Empty, new ApiResponse<GetReport>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [HttpPost("Reply")]
        [ProducesResponseType(typeof(ApiResponse<GetReport>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReplyReport(ReplyReport requestBody)
        {
            var result = await _reportService.ReplyReport(requestBody);
            return Ok(new ApiResponse<GetReport>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPut("Confirm/{reportId}")]
        [ProducesResponseType(typeof(ApiResponse<GetReport>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveReport(int reportId)
        {
            var result = await _reportService.ApproveReport(reportId);
            return Created(string.Empty, new ApiResponse<GetReport>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
