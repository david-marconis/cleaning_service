using CleaningService.Api.Data;
using CleaningService.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CleaningService.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class NotificationController : ControllerBase
{
    private readonly ILogger<NotificationController> logger;
    private readonly Database database;

    public NotificationController(ILogger<NotificationController> logger, Database database)
    {
        this.logger = logger;
        this.database = database;
    }

    [HttpPost]
    public IActionResult Subscribe(Subscription subscription)
    {
        try
        {
            database.InsertSubscription(subscription);
            return Created();
        }
        catch (System.Data.Common.DbException e)
        {
            logger.LogError(e, "Unable to insert subscription {subscription}", subscription);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
