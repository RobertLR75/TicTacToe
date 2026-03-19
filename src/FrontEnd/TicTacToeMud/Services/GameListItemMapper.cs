using Service.Contracts.Responses;
using Service.Contracts.Shared;
using TicTacToeMud.Models;

namespace TicTacToeMud.Services;

internal static class GameListItemMapper
{
    public static GameListItem ToGameListItem(this GameModel game)
    {
        ArgumentNullException.ThrowIfNull(game);

        return new GameListItem
        {
            Id = Guid.Parse(game.GameId),
            Status = (int)game.Status,
            CreatedAt = game.CreatedAt,
            UpdatedAt = game.UpdatedAt,
            Player1 = game.Player1.ToPlayerListItem(),
            Player2 = game.Player2?.ToPlayerListItem()
        };
    }

    private static PlayerListItem ToPlayerListItem(this PlayerModel player)
    {
        ArgumentNullException.ThrowIfNull(player);

        return new PlayerListItem
        {
            Id = player.PlayerId,
            Name = player.Name
        };
    }
}
