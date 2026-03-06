using GameService.Contracts;
using GameService.Endpoints.Games.Create;
using GameService.Endpoints.Games.UpdateStatus;
using Xunit;

namespace GameService.Tests;

public class ValidatorsUnitTests
{
    [Fact]
    public void CreateGameValidator_rejects_empty_player_name()
    {
        var sut = new CreateGameValidator();

        var result = sut.Validate(new CreateGameRequest
        {
            PlayerId = Guid.NewGuid(),
            PlayerName = string.Empty
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateGameRequest.PlayerName));
    }

    [Fact]
    public void CreateGameValidator_rejects_name_longer_than_50_characters()
    {
        var sut = new CreateGameValidator();

        var result = sut.Validate(new CreateGameRequest
        {
            PlayerId = Guid.NewGuid(),
            PlayerName = new string('a', 51)
        });

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpdateGameStatusValidator_accepts_active_and_completed_only()
    {
        var sut = new UpdateGameStatusValidator();
        var id = Guid.NewGuid();

        var active = sut.Validate(new UpdateGameStatusRequest { Id = id, Status = GameStatusEnum.Active });
        var completed = sut.Validate(new UpdateGameStatusRequest { Id = id, Status = GameStatusEnum.Completed });
        var created = sut.Validate(new UpdateGameStatusRequest { Id = id, Status = GameStatusEnum.Created });

        Assert.True(active.IsValid);
        Assert.True(completed.IsValid);
        Assert.False(created.IsValid);
    }
}
