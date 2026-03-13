using Service.Contracts.Responses;
using TicTacToeMud.Models;

namespace TicTacToeMud.Services;

internal static class GameListItemMapper
{
    public static GameListItem ToGameListItem(this GameDto game)
    {
        ArgumentNullException.ThrowIfNull(game);

        return new GameListItem
        {
            Id = game.Id,
            Status = (int)game.Status,
            CreatedAt = game.CreatedAt,
            UpdatedAt = game.UpdatedAt,
            Player1 = game.Player1.ToPlayerListItem(),
            Player2 = game.Player2?.ToPlayerListItem()
        };
    }

    private static PlayerListItem ToPlayerListItem(this PlayerDto player)
    {
        ArgumentNullException.ThrowIfNull(player);

        return new PlayerListItem
        {
            Id = player.Id,
            Name = player.Name
        };
    }
}

