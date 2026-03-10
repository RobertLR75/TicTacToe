using Ardalis.Specification;
using GameService.Endpoints.Games.List;
using GameService.Models;
using SharedLibrary.Interfaces;
using Xunit;

namespace GameService.UnitTests;

public class SearchByStatusSpecificationUnitTests
{
    [Fact]
    public void Constructor_creates_storage_specification_for_games()
    {
        var sut = new SearchByStatusSpecification(GameStatus.Active, page: 2, pageSize: 10);

        Assert.IsAssignableFrom<StorageSpecification<Game>>(sut);
        Assert.IsAssignableFrom<ISpecification<Game>>(sut);
    }
}

