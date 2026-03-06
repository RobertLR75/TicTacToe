using FluentMigrator;

namespace GameService.Persistence.Migrations;

[Migration(2026030601)]
public sealed class CreateGameModelAndPlayerModelTables : Migration
{
    public override void Up()
    {
        Create.Table("player_model")
            .WithColumn("id").AsString(36).PrimaryKey().NotNullable()
            .WithColumn("name").AsString(50).NotNullable()
            .WithColumn("created_at_utc").AsDateTime().NotNullable()
            .WithColumn("updated_at_utc").AsDateTime().Nullable();

        Create.Table("game_model")
            .WithColumn("id").AsString(36).PrimaryKey().NotNullable()
            .WithColumn("status").AsString(20).NotNullable()
            .WithColumn("created_at_utc").AsDateTime().NotNullable()
            .WithColumn("updated_at_utc").AsDateTime().Nullable()
            .WithColumn("player1_id").AsString(36).NotNullable()
            .WithColumn("player2_id").AsString(36).Nullable();

        Create.ForeignKey("fk_game_model_player1")
            .FromTable("game_model").ForeignColumn("player1_id")
            .ToTable("player_model").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.None);

        Create.ForeignKey("fk_game_model_player2")
            .FromTable("game_model").ForeignColumn("player2_id")
            .ToTable("player_model").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.None);

        Create.Index("ix_game_model_status")
            .OnTable("game_model")
            .OnColumn("status").Ascending();
    }

    public override void Down()
    {
        Delete.Table("game_model");
        Delete.Table("player_model");
    }
}
