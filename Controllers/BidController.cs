using Microsoft.AspNetCore.Mvc;

namespace CleaningService.Controllers;

[ApiController]
[Route("[controller]")]
public class BidController : ControllerBase
{
    [HttpPost("{bidId}")]
    public IActionResult CreateBid(int bidId)
    {
        return StatusCode(StatusCodes.Status202Accepted);
    }

    [HttpPut("{bidId}")]
    public IActionResult AcceptBid(int bidId)
    {
        return StatusCode(StatusCodes.Status202Accepted);
    }
}
