using GameService.Models;

namespace GameService.Endpoints.Games.List;

public sealed class ListGamesRequest
{
    public GameStatus Status { get; init; } = GameStatus.Created;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
