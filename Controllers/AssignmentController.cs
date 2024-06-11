using Microsoft.AspNetCore.Mvc;

namespace CleaningService.Controllers;

[ApiController]
[Route("[controller]")]
public class AssignmentController : ControllerBase
{
    [HttpGet("list")]
    public List<Assignment> GetAssignments()
    {
        return [new Assignment(1, "user", null, DateTime.Now, DateTime.Now)];
    }

    [HttpPost]
    public Assignment CreateAssignment(Assignment assignment)
    {
        return assignment with {Id = 1000};
    }
}
