using Service.Contracts.Shared;
using SharedLibrary.Interfaces;

namespace Service.Contracts.Responses;

public sealed record CreateGameResponse
{
    public string Id { get; set; }
    public required GameStatusEnum Status { get; set; }
    public required PlayerModel Player1 { get; set; }
}
