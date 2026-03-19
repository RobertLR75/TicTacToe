using SharedLibrary.Interfaces;

namespace GameService.Features.Games.Entities;

public class PlayerEntity : IEntityId
{
    public required Guid Id { get; set; }
    public required string Name { get; init; }
}


