namespace Service.Contracts.Requests;

public class UpdateGameStateRequest
{
    public required string GameId { get; init; }
    public required int Row { get; init; }
    public required int Col { get; init; }

   
}