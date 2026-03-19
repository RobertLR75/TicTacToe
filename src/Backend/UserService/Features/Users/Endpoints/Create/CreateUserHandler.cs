using FastEndpoints;
using SharedLibrary.Services.Interfaces;
using UserService.Features.Users.Entities;
using UserService.Services;

namespace UserService.Features.Users.Endpoints.Create;

public interface ICreateUserHandler : IRequestHandler<CreateUserCommand, UserEntity>;

public sealed record CreateUserCommand(string Name) : IRequest<UserEntity>;

public sealed class CreateUserHandler(
    IUserStorageService userStorage,
    IUserCacheService userCache) : ICreateUserHandler
{
    public async Task<UserEntity> HandleAsync(CreateUserCommand request, CancellationToken ct = default)
    {
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Status = UserStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await userStorage.CreateAsync(user, ct);
        await userCache.SetUserAsync(user, ct);
        await userCache.SetUsersAsync(await userStorage.ListAsync(ct), ct);

        await new UserCreatedEvent
        {
            User = user
        }.PublishAsync(Mode.WaitForNone, ct);

        return user;
    }
}
