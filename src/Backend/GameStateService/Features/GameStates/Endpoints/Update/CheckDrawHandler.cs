// using GameStateService.Features.GameStates.Entities;
// using SharedLibrary.Services.Interfaces;
//
// namespace GameStateService.Features.GameStates.Endpoints.Update;
//
// public sealed record CheckDraw(Board Board) : IRequest<CheckDrawResult>;
//
//
// public sealed class CheckDrawHandler : IRequestHandler<CheckDraw, CheckDrawResult>
// {
//     public Task<CheckDrawResult> HandleAsync(CheckDraw command, CancellationToken ct = default)
//     {
//         for (var row = 0; row < 3; row++)
//         {
//             for (var col = 0; col < 3; col++)
//             {
//                 if (command.Board.GetCell(row, col).Mark == PlayerMark.None)
//                 {
//                     return Task.FromResult(CheckDrawResult.False());
//                 }
//             }
//         }
//
//         return Task.FromResult(CheckDrawResult.True());
//     }
// }