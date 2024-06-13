namespace CleaningService.Api.Models;

public readonly record struct Bid(
        int? Id,
        int AssignmentId,
        string Cleaner,
        decimal Price,
        string Description
);
