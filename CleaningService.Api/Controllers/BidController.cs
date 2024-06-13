using CleaningService.Api.Data;
using CleaningService.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace CleaningService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BidController : ControllerBase
{
    private readonly ILogger<BidController> logger;
    private readonly Database database;

    public BidController(ILogger<BidController> logger, Database database)
    {
        this.logger = logger;
        this.database = database;
    }

    [HttpPost]
    public ActionResult<Bid> CreateBid(Bid bid)
    {
        try
        {
            return Ok(database.InsertBid(bid));
        }
        catch (SqliteException e) when (e.SqliteErrorCode == 19)
        {
            return NotFound($"Assignment with id {bid.AssignmentId} doesn't exist");
        }
        catch (System.Data.Common.DbException e)
        {
            logger.LogError(e, "Unable to insert bid {bid} to database", bid);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut]
    [Route("{bidId}")]
    public IActionResult AcceptBid(int bidId)
    {
        try
        {
            var isBidAccepted = database.AcceptBid(bidId);
            return isBidAccepted ? Accepted() : NotFound($"Unassigned bid with id {bidId} not found");
        }
        catch (System.Data.Common.DbException e)
        {
            logger.LogError(e, "Unable to accept bid with id {bidId}", bidId);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
