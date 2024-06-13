using CleaningService.Api.Data;
using CleaningService.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace CleaningService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AssignmentController : ControllerBase
{
    private readonly ILogger logger;
    private readonly Database database;

    public AssignmentController(ILogger<AssignmentController> logger, Database database)
    {
        this.logger = logger;
        this.database = database;
    }

    [HttpGet("list")]
    public ActionResult<IEnumerable<Assignment>> GetAssignments()
    {
        try
        {
            return Ok(database.GetAssignments());
        }
        catch (System.Data.Common.DbException e)
        {
            logger.LogError(e, "Unable to fetch assignments from database");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    public ActionResult<Assignment> CreateAssignment(Assignment assignment)
    {
        try
        {
            return Ok(database.InsertAssignment(assignment));
        }
        catch (System.Data.Common.DbException e)
        {
            logger.LogError(e, "Unable to insert assignment {assignment} to database", assignment);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
