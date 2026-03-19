using FastEndpoints;
using SharedLibrary.Services.Interfaces;
using UserService.Features.Users.Entities;
using UserService.Services;

namespace UserService.Features.Users.Endpoints.Update;

public interface IUpdateUserHandler : IRequestHandler<UpdateUserCommand, UserEntity>;

public sealed record UpdateUserCommand(Guid Id, string Name) : IRequest<UserEntity>, IRequest<Service.Contracts.Responses.UserModel>;

public sealed class UpdateUserHandler(
    IUserStorageService userStorage,
    IUserCacheService userCache) : IUpdateUserHandler
{
    public async Task<UserEntity> HandleAsync(UpdateUserCommand request, CancellationToken ct = default)
    {
        var user = await userStorage.GetAsync(request.Id, ct);

        if (user is null)
        {
            return new UserEntity
            {
                Id = Guid.Empty,
                Name = string.Empty,
                Status = UserStatus.Active,
                CreatedAt = DateTimeOffset.MinValue
            };
        }

        user.Name = request.Name.Trim();
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await userStorage.UpdateAsync(user, ct);
        await userCache.SetUserAsync(user, ct);
        await userCache.SetUsersAsync(await userStorage.ListAsync(ct), ct);

        await new UserUpdatedEvent
        {
            User = user
        }.PublishAsync(Mode.WaitForNone, ct);

        return user;
    }
}
