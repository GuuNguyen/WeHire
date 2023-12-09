using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.User;
using WeHire.Application.Utilities.GlobalVariables;
using WeHire.Application.Utilities.Helper.CheckNullProperties;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Services.UserServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetUserDetail>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUserAsync([FromQuery] int roleId,
                                                         [FromQuery] PagingQuery query, 
                                                         [FromQuery] SearchUserDTO searchKey)
        {
            var result = _userService.GetAllUser(roleId, query, searchKey);
            var total = searchKey.AreAllPropertiesNull() ? await _userService.GetTotalItemAsync()
                                                         : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total,
            };
            return Ok(new PagedApiResponse<GetUserDetail>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserLoginAsync(int id)
        {
            var result = await _userService.GetUserLoginAsync(id);

            return Ok(new ApiResponse<object>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }


        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetUserDetail>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateEmployeeAsync(CreateEmployeeDTO requestBody)
        {
            var result = await _userService.CreateEmployeeAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetUserDetail>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GetUserDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromForm] UpdateUserDTO requestBody)
        {
            var result = await _userService.UpdateUserAsync(id, requestBody);

            return Ok( new ApiResponse<GetUserDetail>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPut("ByAdmin/{id}")]
        [ProducesResponseType(typeof(ApiResponse<GetUserDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserByAdminAsync(int id, [FromForm] UpdateUserAdminDTO requestBody)
        {
            var result = await _userService.UpdateUserByAdminAsync(id, requestBody);

            return Ok(new ApiResponse<GetUserDetail>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPut("UpdatePassword/{id}")]
        [ProducesResponseType(typeof(ApiResponse<GetUserDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePasswordAsync(int id, UpdatePassword requestBody)
        {
            var result = await _userService.UpdatePasswordAsync(id, requestBody);

            return Ok(new ApiResponse<GetUserDetail>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpDelete("ChangeStatus/{id}")]
        [ProducesResponseType(typeof(ApiResponse<GetUserDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeStatusAsync(int id)
        {
            var result = await _userService.ChangeStatusAsync(id);

            return Ok(new ApiResponse<GetUserDetail>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
