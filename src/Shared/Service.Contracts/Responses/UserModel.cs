using Service.Contracts.Shared;
using SharedLibrary.Interfaces;

namespace Service.Contracts.Responses;

public sealed record UserModel
{
    public required string UserId { get; set; }
    public required string Name { get; set; }
    public required UserStatusEnum Status { get; set; }
}
