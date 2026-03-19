using GameStateService.Features.GameStates.Entities;
using SharedLibrary.Interfaces;

namespace GameStateService.Tests.Testing;

public sealed class InMemoryGamePersistenceService : IPersistenceService<GameEntity>
{
    private readonly Dictionary<Guid, GameEntity> _games = new();

    public string Name => "test-games";

    public Task<Guid> CreateAsync(GameEntity entity, CancellationToken cancellationToken = default)
    {
        _games[entity.Id] = entity;
        return Task.FromResult(entity.Id);
    }

    public Task UpdateAsync(GameEntity entity, CancellationToken cancellationToken = default)
    {
        _games[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _games.Remove(id);
        return Task.CompletedTask;
    }

    public Task<GameEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_games.TryGetValue(id, out var game))
        {
            return Task.FromResult(game);
        }

        return Task.FromResult<GameEntity?>(null);
    }

    public Task<List<GameEntity>> SearchAsync(IPersistenceSpecification<GameEntity> specification, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<GameEntity> GetAsync(IPersistenceSpecification<GameEntity> specification, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
}
