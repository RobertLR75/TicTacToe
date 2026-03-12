namespace TicTacToeMud.Models;

public record HomeGameListItemViewModel
{
    public required string GameId { get; init; }
    public required string StatusLabel { get; init; }
    public required string PlayerName { get; init; }
    public required string CreatedAtLabel { get; init; }
}
