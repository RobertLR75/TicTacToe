using GameService.Features.Games.Endpoints.List;
using GameService.Features.Games.Entities;
using Service.Contracts.Requests;
using Service.Contracts.Shared;
using Service.Contracts.Responses;
using Xunit;

namespace GameService.UnitTests;

public class ListGamesMapperUnitTests
{
    [Fact]
    public void ToEntity_maps_request_to_query()
    {
        var sut = new ListGamesMapper();

        var query = sut.ToQuery(new ListGamesRequest
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
    public void FromEntity_maps_games_and_preserves_null_updated_at_when_missing()
    {
        var createdAt = DateTimeOffset.UtcNow.AddMinutes(-10);
        var game = new GameEntity
        {
            Id = Guid.NewGuid(),
            Status = GameStatus.Created,
            CreatedAt = createdAt,
            Player1 = new PlayerEntity { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Alice" }
        };

        var sut = new ListGamesMapper();

        var response = sut.FromEntity([game]);

        var dto = Assert.Single(response);
        Assert.Equal(game.Id.ToString(), dto.GameId);
        Assert.Equal(GameStatusEnum.Created, dto.Status);
        Assert.Equal(createdAt, dto.CreatedAt);
        Assert.Null(dto.UpdatedAt);
        Assert.Equal(game.Player1.Id.ToString(), dto.Player1.PlayerId);
        Assert.Equal("Alice", dto.Player1.Name);
        Assert.Null(dto.Player2);
    }

    [Fact]
    public void FromEntity_maps_second_player_when_present()
    {
        var game = new GameEntity
        {
            Id = Guid.NewGuid(),
            Status = GameStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
            UpdatedAt = DateTimeOffset.UtcNow,
            Player1 = new PlayerEntity { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Alice" },
            Player2 = new PlayerEntity { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Bob" }
        };

        var sut = new ListGamesMapper();

        var response = sut.FromEntity([game]);

        var dto = Assert.Single(response);
        Assert.NotNull(dto.Player2);
        Assert.Equal(game.Player2!.Id.ToString(), dto.Player2!.PlayerId);
        Assert.Equal("Bob", dto.Player2.Name);
        Assert.Equal(GameStatusEnum.Active, dto.Status);
        Assert.Equal(game.UpdatedAt, dto.UpdatedAt);
    }
}
