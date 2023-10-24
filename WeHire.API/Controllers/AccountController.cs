using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Level;
using WeHire.Application.DTOs.Skill;
using WeHire.Application.DTOs.User;
using WeHire.Infrastructure.Services.UserServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("SignUp")]
        [ProducesResponseType(typeof(ApiResponse<GetUserDetail>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUserAsync(CreateUserDTO requestBody)
        {
            var result = await _userService.CreateUserAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetUserDetail>(){
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }


        [HttpPost("Login")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginAsync(LoginDTO requestBody)
        {
            var result = await _userService.LoginAsync(requestBody);

            return Ok(new ApiResponse<object>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
