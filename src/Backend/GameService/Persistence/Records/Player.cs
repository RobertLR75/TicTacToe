using SharedLibrary.Interfaces;

namespace GameService.Persistence.Records;

public sealed class PlayerRecord : IDatabaseRecord
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
