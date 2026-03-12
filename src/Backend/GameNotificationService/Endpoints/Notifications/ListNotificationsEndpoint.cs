using FastEndpoints;
using GameNotificationService.Configuration;
using GameNotificationService.Persistence;
using Microsoft.Extensions.Options;

namespace GameNotificationService.Endpoints.Notifications;

public sealed class ListNotificationsRequest
{
    public int? Page { get; init; }

    public int? PageSize { get; init; }

    public string? GameId { get; init; }
}

public sealed record ListNotificationsResponse
{
    public required string EventId { get; init; }

    public required string GameId { get; init; }

    public required string EventType { get; init; }

    public required string PayloadSummary { get; init; }

    public required DateTimeOffset OccurredAtUtc { get; init; }

    public required DateTimeOffset ReceivedAtUtc { get; init; }
}

public sealed class ListNotificationsEndpoint(
    INotificationRepository notificationRepository,
    NotificationPersistenceReadinessState readinessState,
    IOptions<NotificationQueryOptions> queryOptions) : Endpoint<ListNotificationsRequest, List<ListNotificationsResponse>>
{
    public override void Configure()
    {
        Get("/api/notifications");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ListNotificationsRequest request, CancellationToken ct)
    {
        if (!readinessState.IsReady)
        {
            await Send.StringAsync(
                "Notification persistence is temporarily unavailable. Please retry once PostgreSQL initialization succeeds.",
                StatusCodes.Status503ServiceUnavailable,
                cancellation: ct);
            return;
        }

        var options = queryOptions.Value;
        var page = request.Page.GetValueOrDefault(1);
        if (page < 1)
        {
            page = 1;
        }

        var pageSize = request.PageSize.GetValueOrDefault(options.DefaultPageSize);
        pageSize = Math.Clamp(pageSize, 1, options.MaxPageSize);

        var query = new NotificationQuery(page, pageSize, string.IsNullOrWhiteSpace(request.GameId) ? null : request.GameId);
        var notifications = await notificationRepository.ListAsync(query, ct);

        Response = notifications.Select(notification => new ListNotificationsResponse
        {
            EventId = notification.EventId,
            GameId = notification.GameId,
            EventType = notification.EventType,
            PayloadSummary = notification.PayloadSummary,
            OccurredAtUtc = notification.OccurredAtUtc,
            ReceivedAtUtc = notification.ReceivedAtUtc
        }).ToList();

        await Send.OkAsync(Response, ct);
    }
}
