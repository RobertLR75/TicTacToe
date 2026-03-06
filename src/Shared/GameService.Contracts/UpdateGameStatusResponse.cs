namespace GameService.Contracts;

public sealed record UpdateGameStatusResponse
{
    public required Guid Id { get; init; }
    public required string Status { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}