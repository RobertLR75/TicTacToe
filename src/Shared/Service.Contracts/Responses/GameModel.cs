using Service.Contracts.Shared;

namespace Service.Contracts.Responses;

public record GameModel
{
    
    public required string GameId { get; set; }
    public GameStatusEnum Status { get; set; } 
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public required PlayerModel Player1 { get; init; }
    public PlayerModel? Player2 { get; set; }
    
    
}