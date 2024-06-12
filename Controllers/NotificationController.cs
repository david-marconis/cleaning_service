using CleaningService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CleaningService.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    [HttpPost]
    public IActionResult Subscribe(Subscription subscription)
    {
        return StatusCode(StatusCodes.Status201Created);
    }
}
