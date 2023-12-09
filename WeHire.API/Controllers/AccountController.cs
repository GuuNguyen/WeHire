using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WeHire.Application.DTOs.Level;
using WeHire.Application.DTOs.Skill;
using WeHire.Application.DTOs.User;
using WeHire.Application.Services.AccountServices;
using WeHire.Application.Services.UserServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("SignUp")]
        [ProducesResponseType(typeof(ApiResponse<GetUserDetail>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUserAsync(CreateUserDTO requestBody)
        {
            var result = await _accountService.HrSignUpAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetUserDetail>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }


        [HttpPost("Login")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginAsync(LoginDTO requestBody)
        {
            var result = await _accountService.LoginAsync(requestBody);

            return Ok(new ApiResponse<object>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPost("Refresh")]
        [ProducesResponseType(typeof(ApiResponse<RefreshTokenModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshAsync(RefreshTokenModel requestBody)
        {
            var result = await _accountService.RefreshAsync(requestBody);

            return Ok(new ApiResponse<RefreshTokenModel>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("Confirm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<RedirectResult> ConfirmEmailAsync([FromQuery]ConfirmEmailDTO requestBody)
        {
            await _accountService.ConfirmEmailAsync(requestBody);
            return RedirectPermanent("https://frontend-hiring-dev.vercel.app/signin");
        }

        [HttpDelete("Revoke")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> RevokeAsync(int userId)
        {
            await _accountService.RevokeAsync(userId);
            return NoContent();
        }
    }
}
