using SharedLibrary.Interfaces;

namespace GameNotificationService.Services;

public interface INotificationStorageService : IPersistenceService<NotificationRecord>
{
    Task<bool> TryAddAsync(NotificationWriteModel notification, CancellationToken ct = default);

    Task<IReadOnlyList<NotificationRecord>> ListAsync(NotificationQuery query, CancellationToken ct = default);
}
