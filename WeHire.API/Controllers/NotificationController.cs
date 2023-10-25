using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Notification;
using WeHire.Infrastructure.Services.NotificationServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("ByManager")]
        [ProducesResponseType(typeof(ApiResponse<List<GetNotiDetail>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotiByManagerAsync()
        {
            var result = await _notificationService.GetNotificationByManagerAsync();
            return Ok(new ApiResponse<List<GetNotiDetail>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
