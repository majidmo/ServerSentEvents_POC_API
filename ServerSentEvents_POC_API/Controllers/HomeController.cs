using Microsoft.AspNetCore.Mvc;
using ServerSentEvents_POC_API.Services;

namespace ServerSentEvents_POC_API.Controllers
{   
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // SSE Endpoint to Stream Notifications for a Specific User
        [HttpGet("stream/{userId}")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task GetNotifications(int userId, CancellationToken cancellationToken)
        {
            Response.ContentType = "text/event-stream";

            while (!cancellationToken.IsCancellationRequested)
            {
                var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);

                if (notifications.Any())
                {
                    foreach (var notification in notifications)
                    {
                        var jsonData = System.Text.Json.JsonSerializer.Serialize(notification);
                        await Response.WriteAsync($"data: {jsonData}\n\n");
                        await Response.Body.FlushAsync();
                    }
                }

                await Task.Delay(1000, cancellationToken); // Poll every second
            }
        }
   

        // Endpoint to Mark a Notification as Read
        [HttpPost("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(id);
                return Ok(new { Message = "Notification marked as read." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // Endpoint to Get All Notifications for a User
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserNotifications(int userId)
        {
            var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
            return Ok(notifications);
        }
    }

}
