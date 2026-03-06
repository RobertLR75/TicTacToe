using GameStateService.Models;
using GameStateService.Services;

namespace GameStateService.Endpoints.Games.MakeMove;

public sealed record CheckDrawRequest(Board Board) : IRequest<CheckDrawResult>;

public sealed record CheckDrawResult
{
    public required bool IsDraw { get; init; }

    public static CheckDrawResult True() => new() { IsDraw = true };

    public static CheckDrawResult False() => new() { IsDraw = false };
}

public sealed class CheckDrawRequestHandler : IRequestHandler<CheckDrawRequest, CheckDrawResult>
{
    public Task<CheckDrawResult> HandleAsync(CheckDrawRequest request, CancellationToken ct = default)
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (request.Board.GetCell(row, col).Mark == PlayerMark.None)
                {
                    return Task.FromResult(CheckDrawResult.False());
                }
            }
        }

        return Task.FromResult(CheckDrawResult.True());
    }
}
