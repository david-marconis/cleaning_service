namespace CleaningService.Api.Models;

public readonly record struct Assignment(
        int? Id,
        String User,
        String Description,
        int? BidIdAssigned,
        DateTime Created,
        DateTime Updated
);
