using SharedLibrary.Interfaces;

namespace Service.Contracts.Responses;

public sealed record UpdateGameStatusResponse
{
    public Guid Id { get; set; }
    public required string Status { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
}
