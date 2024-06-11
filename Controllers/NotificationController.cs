using Microsoft.AspNetCore.Mvc;

namespace CleaningService.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    public readonly record struct Subscription(String Name, Uri WebHook);

    [HttpPost]
    public IActionResult Subscribe(Subscription subscription)
    {
        return StatusCode(StatusCodes.Status201Created);
    }
}
