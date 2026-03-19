using GameNotificationService.Configuration;
using GameNotificationService.Services;
using Microsoft.Extensions.Options;
using SharedLibrary.Services.Interfaces;

namespace GameNotificationService.Features.Notifications.Endpoints.List;

public interface IListNotificationsHandler : IRequestHandler<ListNotificationsQuery, IReadOnlyList<NotificationRecord>>;

public sealed record ListNotificationsQuery(int Page, int PageSize, string? GameId) : IRequest<IReadOnlyList<NotificationRecord>>;

public sealed class ListNotificationsHandler(
    INotificationStorageService storage,
    IOptions<NotificationQueryOptions> options) : IListNotificationsHandler
{
    public Task<IReadOnlyList<NotificationRecord>> HandleAsync(ListNotificationsQuery request, CancellationToken ct = default)
    {
        var config = options.Value;
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? config.DefaultPageSize : Math.Min(request.PageSize, config.MaxPageSize);

        return storage.ListAsync(new NotificationQuery(page, pageSize, request.GameId), ct);
    }
}
