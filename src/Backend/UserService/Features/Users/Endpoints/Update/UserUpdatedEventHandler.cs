using FastEndpoints;
using UserService.Features.Users.Entities;
using UserService.Services;

namespace UserService.Features.Users.Endpoints.Update;

public sealed record UserUpdatedEvent : IEvent
{
    public required UserEntity User { get; init; }

    public sealed class UserUpdatedEventHandler(IUserEventPublisher eventPublisher) : IEventHandler<UserUpdatedEvent>
    {
        public Task HandleAsync(UserUpdatedEvent model, CancellationToken ct)
            => eventPublisher.PublishEventAsync(UserEventMapper.ToUserUpdated(model.User), ct);
    }
}
