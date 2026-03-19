using GameService.Features.Games.Entities;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.Services;

public interface IGameStorageService : IPostgresSqlStorageService<GameEntity>
{
	Task<PlayerEntity?> GetPlayerAsync(Guid playerId, CancellationToken ct = default);
}

public sealed class GameStorageService : EntityFrameworkPostgresSqlStorageBase<GameEntity>, IGameStorageService
{
	private readonly DbContext _context;

	public GameStorageService(DbContext context) : base(context)
	{
		_context = context;
	}

	public async Task<PlayerEntity?> GetPlayerAsync(Guid playerId, CancellationToken ct = default)
	{
		ArgumentOutOfRangeException.ThrowIfEqual(playerId, Guid.Empty);
		

		return await _context
			.Set<PlayerEntity>()
			.SingleOrDefaultAsync(player => player.Id == playerId, ct);
	}

	
	public override async Task<Guid> CreateAsync(GameEntity gameEntity, CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(gameEntity);
		ArgumentNullException.ThrowIfNull(gameEntity.Player1);

		await EnsureExistingPlayerIsTrackedAsync(gameEntity.Player1, ct);

		if (gameEntity.Player2 is not null)
		{
			await EnsureExistingPlayerIsTrackedAsync(gameEntity.Player2, ct);
		}

		gameEntity.Id = gameEntity.Id == Guid.Empty ? Guid.CreateVersion7() : gameEntity.Id;
		gameEntity.CreatedAt = DateTimeOffset.UtcNow;

		await ExecuteInTransactionAsync(async () =>
		{
			_context.Set<GameEntity>().Add(gameEntity);
			await _context.SaveChangesAsync(ct);
		}, ct);
		
		return gameEntity.Id;
	}
	
	private async Task EnsureExistingPlayerIsTrackedAsync(PlayerEntity playerEntity, CancellationToken ct)
	{
		var entry = _context.Entry(playerEntity);

		if (entry.State != EntityState.Detached)
		{
			return;
		}

		var playerExists = await _context
			.Set<PlayerEntity>()
			.AnyAsync(existingPlayer => existingPlayer.Id == playerEntity.Id, ct);

		if (playerExists)
		{
			entry.State = EntityState.Unchanged;
		}
	}
}
