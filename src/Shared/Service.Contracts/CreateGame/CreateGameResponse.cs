using Service.Contracts.Shared;

namespace Service.Contracts.CreateGame;

public record CreateGameResponse
{
    public required Guid Id { get; init; }
    public required GameStatusEnum Status { get; init; }
    public required PlayerModel Player1 { get; init; }
}