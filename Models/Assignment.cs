namespace CleaningService;

public readonly record struct Assignment(
        int? Id,
        String User,
        String? AssignedTo,
        DateTime Created,
        DateTime Updated
);
