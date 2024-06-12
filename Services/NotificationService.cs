using CleaningService.Data;
using CleaningService.Models;

namespace CleaningService.Services;

public class NotificationService
{
    private readonly ILogger<NotificationService> logger;
    private readonly HttpClient httpClient;
    private readonly Database database;

    public NotificationService(
            ILogger<NotificationService> logger,
            HttpClient httpClient,
            Database database)
    {
        this.logger = logger;
        this.httpClient = httpClient;
        this.database = database;
        database.NewAssignment += OnNewAssignment;
    }

    public void OnNewAssignment(object? sender, Database.NewAssignmentEventArgs args)
    {
        Task.Run(() =>NotifyAllSubscriptions(args.Assignment));
    }

    private void NotifyAllSubscriptions(Assignment assignment)
    {
        IEnumerable<Subscription> subscriptions;
        try
        {
            subscriptions = database.GetSubscriptions();
        }
        catch (System.Data.Common.DbException e)
        {
            logger.LogWarning("Unable to fetch subscriptions from databasse", e);
            return;
        }
        foreach (var subscription in subscriptions)
        {
            // Intentionally don't await async method to send requests concurrently
            _ = NotifySubscription(subscription, assignment);
        }
    }

    private async Task NotifySubscription(Subscription subscription, Assignment assignment)
    {
        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsJsonAsync(subscription.WebHook, assignment);
        }
        catch (System.Exception e)
        {
            logger.LogWarning(
                    $"Notification of assignment {assignment} failed to send to subscription {subscription})",
                   e
            );
            return;
        }
        if (response.IsSuccessStatusCode)
        {
            logger.LogDebug(
                    $"Notification of assignment {assignment} sent to subscription {subscription})"
            );
        }
        else
        {
            logger.LogWarning(
                    $"Notification of assignment {assignment} failed to send to subscription {subscription})"
            );
        }
    }
}
