using GameService.Endpoints.Games.List;
using GameService.Models;
using Service.Contracts.Requests;
using Service.Contracts.Shared;
using Xunit;

namespace GameService.UnitTests;

public class ListGamesMapperUnitTests
{
    [Fact]
    public void ToEntity_maps_request_to_query()
    {
        var sut = new ListGamesMapper();

        var query = sut.ToEntity(new ListGamesRequest
        {
            Status = GameStatusEnum.Active,
            Page = 3,
            PageSize = 7
        });

        Assert.Equal(GameStatus.Active, query.Status);
        Assert.Equal(3, query.Page);
        Assert.Equal(7, query.PageSize);
    }

    [Fact]
    public void FromEntity_maps_games_and_uses_created_at_when_updated_at_is_missing()
    {
        var createdAt = DateTimeOffset.UtcNow.AddMinutes(-10);
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Status = GameStatus.Created,
            CreatedAt = createdAt,
            Player1 = new Player { Id = "p1", Name = "Alice" }
        };

        var sut = new ListGamesMapper();

        var response = sut.FromEntity([game]);

        var dto = Assert.Single(response.Games);
        Assert.Equal(game.Id, dto.Id);
        Assert.Equal(GameStatusEnum.Created, dto.Status);
        Assert.Equal(createdAt, dto.CreatedAt);
        Assert.Equal(createdAt, dto.UpdatedAt);
        Assert.Equal("p1", dto.Player1.Id);
        Assert.Equal("Alice", dto.Player1.Name);
        Assert.Null(dto.Player2);
    }

    [Fact]
    public void FromEntity_maps_second_player_when_present()
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Status = GameStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
            UpdatedAt = DateTimeOffset.UtcNow,
            Player1 = new Player { Id = "p1", Name = "Alice" },
            Player2 = new Player { Id = "p2", Name = "Bob" }
        };

        var sut = new ListGamesMapper();

        var response = sut.FromEntity([game]);

        var dto = Assert.Single(response.Games);
        Assert.NotNull(dto.Player2);
        Assert.Equal("p2", dto.Player2!.Id);
        Assert.Equal("Bob", dto.Player2.Name);
        Assert.Equal(GameStatusEnum.Active, dto.Status);
        Assert.Equal(game.UpdatedAt, dto.UpdatedAt);
    }
}

