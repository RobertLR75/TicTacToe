using FastEndpoints;
using GameService.Models;
using GameService.Services;

namespace GameService.Endpoints.Games.Create;

public class CreateGameEndpoint : Endpoint<CreateGameRequest, CreateGameResponse>
{
    private readonly GameRepository _repository;

    public CreateGameEndpoint(GameRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("/api/game-lobby");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Create a new game";
            s.Description = "Creates a new game with status Created and the requesting player as Player1";
        });
    }

    public override async Task HandleAsync(CreateGameRequest req, CancellationToken ct)
    {
        var player = new PlayerModel
        {
            Id = Guid.NewGuid().ToString(),
            Name = req.PlayerName
        };

        var game = new GameModel
        {
            Id = Guid.NewGuid().ToString(),
            Player1 = player
        };

        await _repository.CreateGameAsync(game, ct);

        Response = new CreateGameResponse
        {
            Id = game.Id,
            Status = game.Status,
            CreatedAt = game.CreatedAt,
            UpdatedAt = game.UpdatedAt,
            Player1 = new PlayerDto { Id = player.Id, Name = player.Name }
        };

        await Send.CreatedAtAsync<CreateGameEndpoint>(null, Response, cancellation: ct);
    }
}
