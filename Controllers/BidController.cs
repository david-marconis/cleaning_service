using CleaningService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CleaningService.Controllers;

[ApiController]
[Route("assignment/{assignmentId}/[controller]")]
public class BidController : ControllerBase
{

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
