namespace Service.Contracts.Requests;

public record UserRequest
{
    public required string Name { get; init; }
}
