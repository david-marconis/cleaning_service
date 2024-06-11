using Microsoft.AspNetCore.Mvc;

namespace CleaningService.Controllers;

[ApiController]
[Route("assignment/{assignmentId}/[controller]")]
public class BidController : ControllerBase
{
    public readonly record struct Bid(int? BidId, decimal Price, string Description);

    [HttpPost]
    public IActionResult CreateBid(Bid bid)
    {
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPut]
    public IActionResult AcceptBid(int bidId)
    {
        return StatusCode(StatusCodes.Status202Accepted);
    }
}
