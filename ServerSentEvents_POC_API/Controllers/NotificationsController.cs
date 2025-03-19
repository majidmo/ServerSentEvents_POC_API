using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using ServerSentEvents_POC_API.Models;
using System.Collections.Concurrent;

namespace ServerSentEvents_POC_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationContext _context;
        
        //This is where we store all open stream responses being sent to a userId
        private static readonly ConcurrentDictionary<int, List<HttpResponse>> _clients = new();

        public NotificationController(NotificationContext context)
        {
            _context = context;
        }

        [HttpGet("subscribe/{userId}")]
        public async Task Subscribe(int userId)
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            if (!_clients.ContainsKey(userId))
                _clients[userId] = new List<HttpResponse>();

            _clients[userId].Add(Response);

            try
            {
                while (!HttpContext.RequestAborted.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                }
            }
            finally
            {
                _clients[userId].Remove(Response);
            }
        }

        // Trigger an event when a notification is created
        private async Task NotifyClients(Notification notification)
        {
            if (_clients.TryGetValue(notification.UserId, out var clients))
            {
                foreach (var client in clients)
                {
                    try
                    {
                        if (!client.HttpContext.RequestAborted.IsCancellationRequested)
                        {
                            var message = $"data: {notification.Message}\n\n";
                            await client.Body.WriteAsync(System.Text.Encoding.UTF8.GetBytes(message));
                            await client.Body.FlushAsync();
                        }
                    }
                    catch
                    {
                        clients.Remove(client);
                    }
                }
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateNotification(Notification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Notify clients about the new notification
            await NotifyClients(notification);

            return Ok(notification);
        }
    }
}


