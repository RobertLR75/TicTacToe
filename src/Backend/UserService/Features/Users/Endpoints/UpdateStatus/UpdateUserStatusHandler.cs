using FastEndpoints;
using SharedLibrary.Interfaces;
using SharedLibrary.Services.Interfaces;
using UserService.Features.Users.Entities;
using UserService.Services;

namespace UserService.Features.Users.Endpoints.UpdateStatus;

public interface IUpdateUserStatusHandler : IRequestHandler<UpdateUserStatusCommand, UpdateUserStatusResult>;

public sealed record UpdateUserStatusCommand(Guid Id, UserStatus Status) : IRequest<UpdateUserStatusResult>;

public sealed record UpdateUserStatusResult : IEntityId
{
    public required bool Found { get; init; }
    public required bool InvalidStatus { get; init; }
    public Guid Id { get; set; }
    public UserEntity? User { get; init; }

    public static UpdateUserStatusResult NotFound() => new()
    {
        Found = false,
        InvalidStatus = false,
        Id = Guid.Empty
    };

    public static UpdateUserStatusResult Invalid() => new()
    {
        Found = false,
        InvalidStatus = true,
        Id = Guid.Empty
    };

    public static UpdateUserStatusResult Success(UserEntity user) => new()
    {
        Found = true,
        InvalidStatus = false,
        Id = user.Id,
        User = user
    };
}

public sealed class UpdateUserStatusHandler(
    IUserStorageService userStorage,
    IUserCacheService userCache) : IUpdateUserStatusHandler
{
    public async Task<UpdateUserStatusResult> HandleAsync(UpdateUserStatusCommand request, CancellationToken ct = default)
    {
        if (request.Status != UserStatus.Disabled)
        {
            return UpdateUserStatusResult.Invalid();
        }

        var user = await userStorage.GetAsync(request.Id, ct);
        if (user is null)
        {
            return UpdateUserStatusResult.NotFound();
        }

        user.Status = request.Status;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await userStorage.UpdateAsync(user, ct);
        await userCache.SetUserAsync(user, ct);
        await userCache.SetUsersAsync(await userStorage.ListAsync(ct), ct);

        await new Update.UserUpdatedEvent
        {
            User = user
        }.PublishAsync(Mode.WaitForNone, ct);

        return UpdateUserStatusResult.Success(user);
    }
}
