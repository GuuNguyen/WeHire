using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Contract;
using WeHire.Application.DTOs.Dashboard;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Infrastructure.Services.DashboardServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<DashboardByAdmin>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboardByAdmin()
        {
            var result = await _dashboardService.GetDashboardByAdminAsync();
            return Ok(new ApiResponse<DashboardByAdmin>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("Project")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<DayOfWeek, int>>), StatusCodes.Status200OK)]
        public IActionResult GetProjectDashboard(DateTime dateInWeek)
        {
            var result = _dashboardService.GetProjectDashboard(dateInWeek);
            return Ok(new ApiResponse<Dictionary<DayOfWeek, int>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("HiringRequest")]
        [ProducesResponseType(typeof(ApiResponse<Dictionary<DayOfWeek, int>>), StatusCodes.Status200OK)]
        public IActionResult GetHiringRequestDashboard(DateTime dateInWeek)
        {
            var result = _dashboardService.GetHiringRequestDashboard(dateInWeek);
            return Ok(new ApiResponse<Dictionary<DayOfWeek, int>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("RecentHiringRequest")]
        [ProducesResponseType(typeof(ApiResponse<List<GetListHiringRequest>>), StatusCodes.Status200OK)]
        public IActionResult GetRecentHiringRequestDashboard()
        {
            var result = _dashboardService.GetRecentRequest();
            return Ok(new ApiResponse<List<GetListHiringRequest>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
