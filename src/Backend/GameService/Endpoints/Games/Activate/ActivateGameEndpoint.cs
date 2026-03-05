using FastEndpoints;
using GameService.Models;
using GameService.Services;

namespace GameService.Endpoints.Games.Activate;

public class ActivateGameEndpoint : Endpoint<ActivateGameRequest, ActivateGameResponse>
{
    private readonly GameRepository _repository;

    public ActivateGameEndpoint(GameRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/api/game-lobby/{Id}/activate");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Activate a game";
            s.Description = "Sets a game status from Created to Active and assigns Player2";
        });
    }

    public override async Task HandleAsync(ActivateGameRequest req, CancellationToken ct)
    {
        var game = await _repository.GetGameAsync(req.Id, ct);

        if (game is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (game.Status != GameStatus.Created)
        {
            AddError("Only games with status 'Created' can be activated");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        game.Player2 = new PlayerModel
        {
            Id = Guid.NewGuid().ToString(),
            Name = req.PlayerName
        };
        game.Status = GameStatus.Active;
        game.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateGameAsync(game, ct);

        Response = new ActivateGameResponse
        {
            Id = game.Id,
            Status = game.Status.ToString(),
            UpdatedAt = game.UpdatedAt
        };

        await Send.OkAsync(Response, ct);
    }
}
