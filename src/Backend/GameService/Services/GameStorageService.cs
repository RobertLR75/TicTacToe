using GameService.Models;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.PostgreSql.EntityFramework;

namespace GameService.Services;

public interface IGameStorageService : IPostgresSqlStorageService<Game>
{
	Task<Player?> GetPlayerAsync(string playerId, CancellationToken ct = default);
	Task CreateGameAsync(Game game, CancellationToken ct = default);
}

public sealed class GameStorageService : BasePostgresSqlStorageService<Game>, IGameStorageService
{
	private readonly DbContext _context;

	public GameStorageService(DbContext context) : base(context)
	{
		_context = context;
	}

	public async Task<Player?> GetPlayerAsync(string playerId, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(playerId);

		return await _context
			.Set<Player>()
			.SingleOrDefaultAsync(player => player.Id == playerId, ct);
	}

	public async Task CreateGameAsync(Game game, CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(game);
		ArgumentNullException.ThrowIfNull(game.Player1);

		await EnsureExistingPlayerIsTrackedAsync(game.Player1, ct);

		if (game.Player2 is not null)
		{
			await EnsureExistingPlayerIsTrackedAsync(game.Player2, ct);
		}

		_context.Set<Game>().Add(game);
		await _context.SaveChangesAsync(ct);
	}

	private async Task EnsureExistingPlayerIsTrackedAsync(Player player, CancellationToken ct)
	{
		var entry = _context.Entry(player);

		if (entry.State != EntityState.Detached)
		{
			return;
		}

		var playerExists = await _context
			.Set<Player>()
			.AnyAsync(existingPlayer => existingPlayer.Id == player.Id, ct);

		if (playerExists)
		{
			entry.State = EntityState.Unchanged;
		}
	}
}
