namespace GameService.Contracts;

public sealed record UpdateGameStatusRequest
{
    public required Guid Id { get; init; }
    public required GameStatusEnum Status { get; init; }
}