using FastEndpoints;
using UserService.Features.Users.Entities;
using UserService.Services;

namespace UserService.Features.Users.Endpoints.Create;

public sealed record UserCreatedEvent : IEvent
{
    public required UserEntity User { get; init; }

    public sealed class UserCreatedEventHandler(IUserEventPublisher eventPublisher) : IEventHandler<UserCreatedEvent>
    {
        public Task HandleAsync(UserCreatedEvent model, CancellationToken ct)
            => eventPublisher.PublishEventAsync(UserEventMapper.ToUserCreated(model.User), ct);
    }
}
