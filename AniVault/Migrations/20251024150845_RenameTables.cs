using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniVault.Migrations
{
    /// <inheritdoc />
    public partial class RenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_telegram_media_document_anime_configuration_anime_configuratio",
                table: "TelegramMediaDocument");

            migrationBuilder.RenameTable(
                name: "TelegramMessage",
                newName: "telegram_message");

            migrationBuilder.RenameTable(
                name: "TelegramMediaDocument",
                newName: "telegram_media_document");

            migrationBuilder.RenameTable(
                name: "TelegramChannel",
                newName: "telegram_channel");

            migrationBuilder.RenameTable(
                name: "ScheduledTask",
                newName: "scheduled_task");

            migrationBuilder.RenameTable(
                name: "ApiUser",
                newName: "api_user");

            migrationBuilder.RenameTable(
                name: "AnimeConfiguration",
                newName: "anime_configuration");

            migrationBuilder.AddForeignKey(
                name: "fk_telegram_media_document_anime_configuration_anime_configura",
                table: "telegram_media_document",
                column: "anime_configuration_id",
                principalTable: "anime_configuration",
                principalColumn: "anime_configuration_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_telegram_media_document_anime_configuration_anime_configura",
                table: "telegram_media_document");

            migrationBuilder.RenameTable(
                name: "telegram_message",
                newName: "TelegramMessage");

            migrationBuilder.RenameTable(
                name: "telegram_media_document",
                newName: "TelegramMediaDocument");

            migrationBuilder.RenameTable(
                name: "telegram_channel",
                newName: "TelegramChannel");

            migrationBuilder.RenameTable(
                name: "scheduled_task",
                newName: "ScheduledTask");

            migrationBuilder.RenameTable(
                name: "api_user",
                newName: "ApiUser");

            migrationBuilder.RenameTable(
                name: "anime_configuration",
                newName: "AnimeConfiguration");

            migrationBuilder.AddForeignKey(
                name: "fk_telegram_media_document_anime_configuration_anime_configuratio",
                table: "TelegramMediaDocument",
                column: "anime_configuration_id",
                principalTable: "AnimeConfiguration",
                principalColumn: "anime_configuration_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
