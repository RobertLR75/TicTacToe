using GameService.Endpoints.Games.UpdateStatus;
using GameService.Models;
using Service.Contracts.Shared;
using Service.Contracts.UpdateGameStatus;
using Xunit;

namespace GameService.Tests;

public class UpdateGameStatusMapperUnitTests
{
    [Fact]
    public void ToEntity_maps_request_to_command()
    {
        var gameId = Guid.NewGuid();
        var sut = new UpdateGameStatusMapper();

        var command = sut.ToEntity(new UpdateGameStatusRequest
        {
            Id = gameId,
            Status = GameStatusEnum.Active
        });

        Assert.Equal(gameId, command.GameId);
        Assert.Equal(GameStatus.Active, command.Status);
    }

    [Fact]
    public void ToEntity_throws_for_null_request()
    {
        var sut = new UpdateGameStatusMapper();

        Assert.Throws<ArgumentNullException>(() => sut.ToEntity(null!));
    }
}
