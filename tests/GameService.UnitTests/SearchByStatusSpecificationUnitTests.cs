using Ardalis.Specification;
using GameService.Features.Games.Endpoints.List;
using GameService.Features.Games.Entities;
using SharedLibrary.Interfaces;
using Xunit;

namespace GameService.UnitTests;

public class SearchByStatusSpecificationUnitTests
{
    [Fact]
    public void Constructor_creates_storage_specification_for_games()
    {
        var sut = new SearchByStatusSpecification(GameStatus.Active, page: 2, pageSize: 10);

        Assert.IsAssignableFrom<PersistenceSpecification<GameEntity>>(sut);
        Assert.IsAssignableFrom<ISpecification<GameEntity>>(sut);
    }
}
