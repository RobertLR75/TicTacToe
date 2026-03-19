namespace Service.Contracts.Responses;

public sealed record ListUsersResponse
{
    public required List<UserModel> Users { get; set; }
}
