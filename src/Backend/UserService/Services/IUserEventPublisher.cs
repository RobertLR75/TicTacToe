using Service.Contracts.Shared;
using SharedLibrary.Interfaces;

namespace UserService.Services;

public interface IUserEventPublisher
{
    Task PublishEventAsync<T>(T @event, CancellationToken ct = default) where T : class, ISharedEvent;
}
