using GameService.Models;
using Microsoft.EntityFrameworkCore;

namespace GameService.Persistence;

public sealed class GamePersistenceDbContext(DbContextOptions<GamePersistenceDbContext> options) : DbContext(options)
{
    public DbSet<GameModel> Games => Set<GameModel>();
    public DbSet<PlayerModel> Players => Set<PlayerModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerModel>(entity =>
        {
            entity.ToTable("player_model");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").HasMaxLength(36);
            entity.Property(x => x.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<GameModel>(entity =>
        {
            entity.ToTable("game_model");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id").HasMaxLength(36);
            entity.Property(x => x.Status).HasConversion<string>().HasColumnName("status").HasMaxLength(20).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at_utc").IsRequired();
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at_utc");
            entity.Property<string>("player1_id").HasColumnName("player1_id").HasMaxLength(36).IsRequired();
            entity.Property<string?>("player2_id").HasColumnName("player2_id").HasMaxLength(36);

            entity.HasOne(x => x.Player1)
                .WithMany()
                .HasForeignKey("player1_id")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Player2)
                .WithMany()
                .HasForeignKey("player2_id")
                .OnDelete(DeleteBehavior.Restrict);

            entity.Navigation(x => x.Player1).AutoInclude();
            entity.Navigation(x => x.Player2).AutoInclude();

            entity.HasIndex(x => x.Status).HasDatabaseName("ix_game_model_status");
        });
    }
}
