using GameStateService.Models;
using GameStateService.Services;

namespace GameStateService.GameState;

public sealed record CheckDraw(Board Board) : IRequest<CheckDrawResult>;

public sealed record CheckDrawResult
{
    public required bool IsDraw { get; init; }

    public static CheckDrawResult True() => new() { IsDraw = true };

    public static CheckDrawResult False() => new() { IsDraw = false };
}

public sealed class CheckDrawHandler : IRequestHandler<CheckDraw, CheckDrawResult>
{
    public Task<CheckDrawResult> HandleAsync(CheckDraw command, CancellationToken ct = default)
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (command.Board.GetCell(row, col).Mark == PlayerMark.None)
                {
                    return Task.FromResult(CheckDrawResult.False());
                }
            }
        }

        return Task.FromResult(CheckDrawResult.True());
    }
}
