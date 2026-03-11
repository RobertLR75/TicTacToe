using GameService.Endpoints.Games.Create;
using GameService.Models;
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

        var command = sut.ToEntity(new CreateGameRequest
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

        var response = sut.FromEntity(new Game
        {
            Id = gameId,
            Status = GameStatus.Created,
            Player1 = new Player
            {
                Id = "p1",
                Name = "Alice"
            }
        });

        Assert.Equal(gameId, response.Id);
        Assert.Equal(GameStatusEnum.Created, response.Status);
        Assert.Equal("p1", response.Player1.Id);
        Assert.Equal("Alice", response.Player1.Name);
    }
}

