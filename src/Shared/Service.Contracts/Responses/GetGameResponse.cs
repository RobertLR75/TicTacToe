using Service.Contracts.Shared;
using SharedLibrary.Interfaces;

namespace Service.Contracts.Responses;

public sealed record GetGameResponse : GameModel
{
    
}

public record CellResponse(int Row, int Col, PlayerMarkEnum MarkEnum);