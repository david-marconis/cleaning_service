namespace CleaningService;

public record Assignment(
        int? id,
        String user,
        String? assignedTo,
        DateTime created,
        DateTime updated
);
