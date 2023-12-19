using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.DTOs.User;
using WeHire.Application.Utilities.GlobalVariables;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Services.DeveloperServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize(Roles = "Admin")]
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

        [Authorize]
        [HttpGet("ByProject")]
        [ProducesResponseType(typeof(ApiResponse<List<GetDeveloperInProject>>), StatusCodes.Status200OK)]
        public IActionResult GetDevByProjectId([FromQuery] DevInProjectRequestModel requestBody)
        {
            var result = _devService.GetDevsByProjectId(requestBody);
            return Ok(new ApiResponse<List<GetDeveloperInProject>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetDevDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateDeveloperAsync(CreateDevDTO requestBody)
        {
            var result = await _devService.CreateDevAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetDevDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result   
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("ByAdmin/{developerId}")]
        [ProducesResponseType(typeof(ApiResponse<GetDevDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDeveloperByAdminAsync(int developerId, [FromForm] UpdateDevByAdmin requestBody)
        {
            var result = await _devService.UpdateDevProfileByAdminAsync(developerId, requestBody);
            return Ok(new ApiResponse<GetDevDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize(Roles = "Admin, Developer")]
        [HttpPut("{developerId}")]
        [ProducesResponseType(typeof(ApiResponse<GetDevDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDeveloperAsync(int developerId, [FromForm] UpdateDevModel requestBody)
        {
            var result = await _devService.UpdateDevProfileAsync(developerId, requestBody);
            return Ok(new ApiResponse<GetDevDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateUserStatusOfDeveloper/{developerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeStatusDevAsync(ChangeStatusDeveloper requestBody)
        {
            await _devService.ChangStatusDeveloperAsync(requestBody);
            return Ok("Update status success!");
        }
    }
}
