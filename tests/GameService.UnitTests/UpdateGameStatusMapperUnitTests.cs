using GameService.Features.Games.Endpoints.UpdateStatus;
using GameService.Features.Games.Entities;
using Service.Contracts.Requests;
using Service.Contracts.Shared;
using Xunit;

namespace GameService.UnitTests;

public class UpdateGameStatusMapperUnitTests
{
    [Fact]
    public void ToEntity_maps_request_to_command()
    {
        var gameId = Guid.NewGuid();
        var sut = new UpdateGameStatusMapper();

        var command = sut.ToCommand(new UpdateGameStatusRequest
        {
            Id = gameId,
            Status = GameStatusEnum.Active
        });

        Assert.Equal(gameId, command.GameId);
        Assert.Equal(GameStatus.Active, command.Status);
    }

    [Fact]
    public void ToEntity_maps_completed_status_to_command()
    {
        var gameId = Guid.NewGuid();
        var sut = new UpdateGameStatusMapper();

        var command = sut.ToCommand(new UpdateGameStatusRequest
        {
            Id = gameId,
            Status = GameStatusEnum.Completed
        });

        Assert.Equal(gameId, command.GameId);
        Assert.Equal(GameStatus.Completed, command.Status);
    }

    [Fact]
    public void ToEntity_throws_for_created_status()
    {
        var sut = new UpdateGameStatusMapper();

        var ex = Assert.Throws<ArgumentException>(() => sut.ToCommand(new UpdateGameStatusRequest
        {
            Id = Guid.NewGuid(),
            Status = GameStatusEnum.Created
        }));

        Assert.Contains("Invalid status value", ex.Message);
    }

    [Fact]
    public void ToEntity_throws_for_null_request()
    {
        var sut = new UpdateGameStatusMapper();

        Assert.Throws<ArgumentNullException>(() => sut.ToCommand(null!));
    }

    [Fact]
    public void FromEntity_maps_success_result_to_response()
    {
        var updatedAt = DateTimeOffset.UtcNow;
        var sut = new UpdateGameStatusMapper();

        var response = sut.FromEntity(GameStatusUpdateResult.SuccessResult(Guid.NewGuid(), GameStatus.Active, updatedAt));

        Assert.Equal(GameStatus.Active.ToString(), response.Status);
        Assert.Equal(updatedAt, response.UpdatedAt);
    }
}
