using System.Diagnostics;
using Service.Contracts.Events;
using Service.Contracts.Shared;
using UserService.Features.Users.Entities;

namespace UserService.Services;

public static class UserEventMapper
{
    public static UserCreated ToUserCreated(UserEntity user)
    {
        return new UserCreated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = EventSchemaVersion.V1,
            Id = user.Id,
            Name = user.Name,
            Status = (UserStatusEnum)user.Status,
            CreatedAt = user.CreatedAt,
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = Activity.Current?.TraceId.ToString()
        };
    }

    public static UserUpdated ToUserUpdated(UserEntity user)
    {
        return new UserUpdated
        {
            EventId = Guid.NewGuid().ToString("N"),
            SchemaVersion = EventSchemaVersion.V1,
            UserId = user.Id,
            Name = user.Name,
            Status = (UserStatusEnum)user.Status,
            UpdatedAt = user.UpdatedAt ?? DateTimeOffset.UtcNow,
            OccurredAtUtc = DateTimeOffset.UtcNow,
            CorrelationId = Activity.Current?.TraceId.ToString()
        };
    }
}

public static class EventSchemaVersion
{
    public const string V1 = "1.0";
}
