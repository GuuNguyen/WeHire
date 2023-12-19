using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.UserDevice;
using WeHire.Application.Services.UserDeviceServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDeviceController : ControllerBase
    {
        private readonly IUserDeviceService _userDeviceService;

        public UserDeviceController(IUserDeviceService userDeviceService)
        {
            _userDeviceService = userDeviceService;
        }

        [HttpGet("User/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetUserDevice>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserDeviceByUserId(int userId)
        {
            var result = await _userDeviceService.GetUserDeviceByUserId(userId);
            return Ok(new ApiResponse<List<GetUserDevice>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetUserDevice>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateDeviceTokenAsync(CreateUserDevice requestBody)
        {
            var result = await _userDeviceService.CreateUserDevice(requestBody);

            return Created(string.Empty, new ApiResponse<GetUserDevice>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }


        [HttpDelete("{userDeviceId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveDeviceToken(int userDeviceId)
        {
            var result = await _userDeviceService.RemoveUserDevice(userDeviceId);

            return Created(string.Empty, new ApiResponse<string>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
