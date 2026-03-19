using GameService.Features.Games.Endpoints.Create;
using GameService.Features.Games.Entities;
using Service.Contracts.Requests;
using Service.Contracts.Shared;
using Xunit;

namespace GameService.UnitTests;

public class CreateGameMapperUnitTests
{
    [Fact]
    public void ToEntity_maps_request_to_command()
    {
        var playerId = Guid.NewGuid();
        var sut = new CreateGameMapper();

        var command = sut.ToCommand(new CreateGameRequest
        {
            PlayerId = playerId,
            PlayerName = "Alice"
        });

        Assert.Equal(playerId, command.PlayerId);
        Assert.Equal("Alice", command.PlayerName);
    }

    [Fact]
    public void FromEntity_maps_game_to_response()
    {
        var gameId = Guid.NewGuid();
        var sut = new CreateGameMapper();

        var response = sut.FromEntity(new GameEntity
        {
            Id = gameId,
            Status = GameStatus.Created,
            Player1 = new PlayerEntity
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Alice"
            }
        });

        Assert.Equal(gameId.ToString(), response.Id);
        Assert.Equal(GameStatusEnum.Created, response.Status);
        Assert.Equal("11111111-1111-1111-1111-111111111111", response.Player1.PlayerId);
        Assert.Equal("Alice", response.Player1.Name);
    }
}
