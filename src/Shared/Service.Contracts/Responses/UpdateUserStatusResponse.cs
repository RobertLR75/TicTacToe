using Service.Contracts.Shared;
using SharedLibrary.Interfaces;

namespace Service.Contracts.Responses;

public sealed record UpdateUserStatusResponse
{
    public string Id { get; set; }
    public required UserStatusEnum Status { get; set; }
}
