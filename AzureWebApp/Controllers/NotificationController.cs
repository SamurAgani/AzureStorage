using AzureWebApp.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AzureWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        public readonly IHubContext<NotificationHub> hubContext;

        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
           this.hubContext = hubContext;  
        }

        public IActionResult ComplateWatermark(string connectionId)
        {
            hubContext.Clients.Client(connectionId).SendAsync("NotifyComplateWatermark");
            return Ok();
        }
    }
}
