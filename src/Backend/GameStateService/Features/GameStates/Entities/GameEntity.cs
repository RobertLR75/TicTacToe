using SharedLibrary.Interfaces;

namespace GameStateService.Features.GameStates.Entities;

public sealed class GameEntity : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string GameId
    {
        get => Id.ToString("D");
        set => Id = Guid.TryParse(value, out var gameId) ? gameId : throw new ArgumentException("GameId must be a valid GUID.", nameof(value));
    }

    public Board Board { get; set; } = new();
    public PlayerMark CurrentPlayer { get; set; } = PlayerMark.X;
    public PlayerMark Winner { get; set; } = PlayerMark.None;
    public bool IsDraw { get; set; }
    public bool IsOver => Winner != PlayerMark.None || IsDraw;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
}
