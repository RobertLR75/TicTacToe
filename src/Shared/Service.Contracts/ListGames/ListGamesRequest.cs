using Service.Contracts.Shared;

namespace Service.Contracts.ListGames;

public sealed class ListGamesRequest
{
    public GameStatusEnum Status { get; init; } = GameStatusEnum.Created;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
