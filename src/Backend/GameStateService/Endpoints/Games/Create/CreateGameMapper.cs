using FastEndpoints;

namespace GameStateService.Endpoints.Games.Create;

public sealed class CreateGameMapper : ResponseMapper<CreateGameResponse, Models.GameState>
{
    public override CreateGameResponse FromEntity(Models.GameState e)
    {
        return new CreateGameResponse
        {
            GameId = e.GameId
        };
    }
}

