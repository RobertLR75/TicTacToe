using FastEndpoints;
using GameService.Models;
using GameService.Services;

namespace GameService.Endpoints.Games.Complete;

public class CompleteGameEndpoint : Endpoint<CompleteGameRequest, CompleteGameResponse>
{
    private readonly GameRepository _repository;

    public CompleteGameEndpoint(GameRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/api/game-lobby/{Id}/complete");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Complete a game";
            s.Description = "Sets a game status from Active to Completed";
        });
    }

    public override async Task HandleAsync(CompleteGameRequest req, CancellationToken ct)
    {
        var game = await _repository.GetGameAsync(req.Id, ct);

        if (game is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (game.Status != GameStatus.Active)
        {
            AddError("Only games with status 'Active' can be completed");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        game.Status = GameStatus.Completed;
        game.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateGameAsync(game, ct);

        Response = new CompleteGameResponse
        {
            Id = game.Id,
            Status = game.Status.ToString(),
            UpdatedAt = game.UpdatedAt
        };

        await Send.OkAsync(Response, ct);
    }
}
