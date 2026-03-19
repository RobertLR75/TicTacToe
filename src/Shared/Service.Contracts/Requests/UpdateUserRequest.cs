namespace Service.Contracts.Requests;

public sealed record UpdateUserRequest : UserRequest
{
    public required Guid Id { get; init; }
}
