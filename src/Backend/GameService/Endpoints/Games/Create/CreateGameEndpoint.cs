using FastEndpoints;
using GameService.Contracts;
using GameService.Models;
using SharedLibrary.PostgreSql.EntityFramework;
using PlayerModel = GameService.Models.PlayerModel;

namespace GameService.Endpoints.Games.Create;

public class CreateGameEndpoint : Endpoint<CreateGameRequest, CreateGameResponse>
{
    private readonly IPostgresSqlStorageService<GameModel> _gameStore;

    public CreateGameEndpoint(IPostgresSqlStorageService<GameModel> gameStore)
    {
        _gameStore = gameStore;
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
            Id = req.PlayerId.ToString("D"),
            Name = req.PlayerName
        };

        var game = new GameModel
        {
            Id = Guid.NewGuid(),
            Player1 = player
        };

        await _gameStore.CreateAsync(game, ct);

        Response = new CreateGameResponse
        {
            Id = game.Id,
            Player1 = new Contracts.PlayerModel()
            {
                Id = player.Id,
                Name = player.Name
            },
            Status = GameStatusEnum.Created
        };

        await Send.CreatedAtAsync<CreateGameEndpoint>(null, Response, cancellation: ct);
    }
}
