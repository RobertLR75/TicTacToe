using GameService.Models;

namespace GameService.Endpoints.Games.List;

public record ListGamesResponse
{
    public required List<GameDto> Games { get; init; }
}

public record GameDto
{
    public required Guid Id { get; init; }
    public required GameStatus Status { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset? UpdatedAt { get; init; }
    public required PlayerDto Player1 { get; init; }
    public PlayerDto? Player2 { get; init; }
}

public record PlayerDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
}
