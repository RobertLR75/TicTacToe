namespace GameNotificationService.Persistence;

public interface INotificationRepository
{
    Task<bool> TryAddAsync(NotificationWriteModel notification, CancellationToken ct = default);

    Task<IReadOnlyList<NotificationRecord>> ListAsync(NotificationQuery query, CancellationToken ct = default);
}
