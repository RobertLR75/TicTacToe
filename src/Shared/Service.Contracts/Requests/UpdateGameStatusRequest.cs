using Service.Contracts.Shared;

namespace Service.Contracts.Requests;

public sealed record UpdateGameStatusRequest
{
    public required Guid Id { get; init; }
    public required GameStatusEnum Status { get; init; }
}