using GameService.Models;

namespace GameService.Endpoints.Games.Create;

public record CreateGameRequest
{
    public required string PlayerName { get; init; }
}

public record CreateGameResponse
{
    public required string Id { get; init; }
    public required GameStatus Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required PlayerDto Player1 { get; init; }
}

public record PlayerDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
}
