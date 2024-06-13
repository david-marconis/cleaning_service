namespace CleaningService.Api.Models;

public readonly record struct Subscription(
        String Name,
        Uri WebHook
);
