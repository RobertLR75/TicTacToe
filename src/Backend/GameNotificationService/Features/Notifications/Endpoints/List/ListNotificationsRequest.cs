namespace GameNotificationService.Features.Notifications.Endpoints.List;

public sealed record ListNotificationsRequest
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 50;

    public string? GameId { get; init; }
}
