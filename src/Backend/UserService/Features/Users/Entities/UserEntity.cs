using SharedLibrary.Interfaces;

namespace UserService.Features.Users.Entities;

public enum UserStatus
{
    Active = 0,
    Disabled = 1
}

public sealed class UserEntity : IEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public UserStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

public sealed class UserListEntity : IEntityId
{
    public Guid Id { get; set; }
    public required IReadOnlyList<UserEntity> Users { get; set; }
}
