using Service.Contracts.Shared;

namespace Service.Contracts.Responses;

public sealed class ListGamesResponse :  List<GameModel>
{
    public void Add(List<GameModel> toList)
    {
        AddRange(toList);
    }
}

// public sealed record GameDto
// {
//     public required string Id { get; set; }
//     public required GameStatusEnum Status { get; set; }
//     public required DateTimeOffset CreatedAt { get; set; }
//     public required DateTimeOffset UpdatedAt { get; set; }
//     public required PlayerDto Player1 { get; set; }
//     public PlayerDto? Player2 { get; set; }
// }
//
// public sealed record PlayerDto
// {
//     public required string Id { get; set; }
//     public required string Name { get; set; }
// }
