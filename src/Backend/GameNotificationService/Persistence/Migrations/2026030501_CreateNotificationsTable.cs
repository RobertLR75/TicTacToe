using FluentMigrator;

namespace GameNotificationService.Persistence.Migrations;

[Migration(2026030501)]
public sealed class CreateNotificationsTable : Migration
{
    public override void Up()
    {
        Create.Table("notifications")
            .WithColumn("id").AsString(32).PrimaryKey().NotNullable()
            .WithColumn("event_id").AsString(128).NotNullable().Unique()
            .WithColumn("game_id").AsString(128).NotNullable()
            .WithColumn("event_type").AsString(64).NotNullable()
            .WithColumn("payload_summary").AsCustom("jsonb").NotNullable()
            .WithColumn("occurred_at_utc").AsDateTimeOffset().NotNullable()
            .WithColumn("received_at_utc").AsDateTimeOffset().NotNullable();

        Create.Index("ix_notifications_game_id_occurred")
            .OnTable("notifications")
            .OnColumn("game_id").Ascending()
            .OnColumn("occurred_at_utc").Descending();

        Create.Index("ix_notifications_occurred")
            .OnTable("notifications")
            .OnColumn("occurred_at_utc").Descending();
    }

    public override void Down()
    {
        Delete.Table("notifications");
    }
}
