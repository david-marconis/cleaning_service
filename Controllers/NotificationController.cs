using Microsoft.AspNetCore.Mvc;

namespace CleaningService.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    public record Subscription(String name, Uri webHook);

    [HttpPost]
    public IActionResult Subscribe(Subscription subscription)
    {
        return StatusCode(StatusCodes.Status201Created);
    }
}
