using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.CV;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.DTOs.User;
using WeHire.Application.Utilities.GlobalVariables;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Infrastructure.Services.DeveloperServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeveloperController : ControllerBase
    {
        private readonly IDeveloperService _devService;

        public DeveloperController(IDeveloperService devService)
        {
            _devService = devService;
        }


        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetAllFieldDev>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllDevAsync([FromQuery] PagingQuery query,
                                                         [FromQuery] SearchDeveloperDTO searchKey)
        {
            var result = _devService.GetAllDev(query, searchKey);
            var total = await _devService.GetTotalItemAsync();
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total,
            };
            return Ok(new PagedApiResponse<GetAllFieldDev>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }


        [HttpGet("Unofficial")]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetDevDTO>>), StatusCodes.Status200OK)]
        public IActionResult GetAllUnofficial([FromQuery] PagingQuery query)
        {
            var result = _devService.GetUnofficialDev(query);
            var total = result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total,
            };
            return Ok(new PagedApiResponse<GetDevDTO>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [HttpGet("DevWaitingInterview/{requestId}")]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetAllFieldDev>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDevsWaitingInterview([FromQuery] PagingQuery query, int requestId)
        {
            var result = _devService.GetDevsWaitingInterview(query, requestId);
            var total = await _devService.GetTotalDevWaitingInterviewAsync(requestId);
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total,
            };
            return Ok(new PagedApiResponse<GetAllFieldDev>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [HttpGet("AssignTask/{taskId}")]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetAllFieldDev>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllDevAsync(int taskId)
        {
            var result = await _devService.GetAllDevByTaskIdAsync(taskId);

            return Ok(new PagedApiResponse<GetAllFieldDev>()
            {
                Code = StatusCodes.Status200OK,
                Paging = null,
                Data = result
            });
        }

        [HttpGet("{devId}")]
        [ProducesResponseType(typeof(ApiResponse<GetDevDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDevByIdAsync(int devId)
        {
            var result = await _devService.GetDevByIdAsync(devId);

            return Ok(new ApiResponse<GetDevDetail>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }


        [HttpGet("DevMatching/{requestId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetMatchingDev>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMatchingDev(int requestId)
        {
            var result = await _devService.GetDevMatchingWithRequest(requestId);

            return Ok(new ApiResponse<List<GetMatchingDev>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }      

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetDevDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateEmployeeAsync(CreateDevDTO requestBody)
        {
            var result = await _devService.CreateDevAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetDevDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result   
            });
        }

        [HttpPut("Active/{developerId}")]
        [ProducesResponseType(typeof(ApiResponse<GetDevDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeStatusDevToActiveAsync(int developerId)
        {
            var result = await _devService.ActiveDeveloperAsync(developerId);
            return Ok(new ApiResponse<GetDevDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
