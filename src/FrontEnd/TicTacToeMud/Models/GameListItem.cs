namespace TicTacToeMud.Models;

public record GameListItem
{
    public required Guid Id { get; init; }
    public required int Status { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset? UpdatedAt { get; init; }
    public required PlayerListItem Player1 { get; init; }
    public PlayerListItem? Player2 { get; init; }
}

public record PlayerListItem
{
    public required string Id { get; init; }
    public required string Name { get; init; }
}
