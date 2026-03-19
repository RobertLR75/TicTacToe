using SharedLibrary.Interfaces;

namespace GameService.Persistence.Records;

public sealed class Game : IDatabaseRecord
{
    public required string Id { get; set; }
    public required string Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public required string Player1Id { get; init; }
    public string? Player2Id { get; set; }

    public required PlayerRecord Player1 { get; init; }
    public PlayerRecord? Player2 { get; set; }
}
