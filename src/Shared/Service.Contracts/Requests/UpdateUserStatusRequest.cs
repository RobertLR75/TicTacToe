using Service.Contracts.Shared;

namespace Service.Contracts.Requests;

public sealed record UpdateUserStatusRequest
{
    public required Guid Id { get; init; }
    public required UserStatusEnum Status { get; init; }
}
