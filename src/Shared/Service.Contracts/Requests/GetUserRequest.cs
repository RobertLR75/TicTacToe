namespace Service.Contracts.Requests;

public sealed record GetUserRequest
{
    public required Guid Id { get; init; }
}
