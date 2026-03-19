using FastEndpoints;

namespace GameNotificationService.Features.Notifications.Endpoints.List;

public sealed class ListNotificationsEndpoint(IListNotificationsHandler handler) : Endpoint<ListNotificationsRequest, IReadOnlyList<Services.NotificationRecord>>
{
    public override void Configure()
    {
        Get("/api/notifications");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List stored notifications";
            s.Description = "Returns stored game notifications ordered by newest first.";
        });
    }

    public override async Task HandleAsync(ListNotificationsRequest req, CancellationToken ct)
    {
        var notifications = await handler.HandleAsync(new ListNotificationsQuery(req.Page, req.PageSize, req.GameId), ct);
        await Send.OkAsync(notifications, ct);
    }
}
