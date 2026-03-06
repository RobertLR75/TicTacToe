using GameStateService.Services;

namespace GameStateService.Endpoints.Games.Create;

public sealed record CreateGameCommand : IRequest<CreateGameResponse>;
