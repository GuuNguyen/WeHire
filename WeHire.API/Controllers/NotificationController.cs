using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Notification;
using WeHire.Application.Services.NotificationServices;
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

        [HttpGet("Count/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotiCount(int userId)
        {
            var result = await _notificationService.GetNotificationCount(userId);
            return Ok(new ApiResponse<int>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
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

        [HttpGet("ByUser/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetNotiDetail>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNotiByHrAsync(int userId)
        {
            var result = await _notificationService.GetNotificationAsync(userId);
            return Ok(new ApiResponse<List<GetNotiDetail>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("Test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> TestSendNotiFirebaseAsync(string deviceToken, string title, string content,
                                                         string notificationType, int routeId)
        {
            var result = await _notificationService.TestSendNotificationFirebase(deviceToken, title, content, notificationType, routeId);
            return Ok(result);
        }

        [HttpPut("Read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ReadNotificationAsync(int notificationId, int userId)
        {
            await _notificationService.ReadNotification(notificationId, userId);
            return NoContent();
        }

        [HttpPut("UnNew")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UnNewNotificationAsync(int userId)
        {
            await _notificationService.UnNewNotification(userId);
            return NoContent();
        }
    }
}
