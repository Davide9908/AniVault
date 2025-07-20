using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniVault.Migrations
{
    /// <inheritdoc />
    public partial class FixUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_telegram_message_message_id",
                table: "TelegramMessage");

            migrationBuilder.DropIndex(
                name: "ix_telegram_media_document_file_id",
                table: "TelegramMediaDocument");

            migrationBuilder.CreateIndex(
                name: "ix_telegram_message_message_id_telegram_channel_id",
                table: "TelegramMessage",
                columns: new[] { "message_id", "telegram_channel_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_telegram_media_document_file_id_telegram_message_id",
                table: "TelegramMediaDocument",
                columns: new[] { "file_id", "telegram_message_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_telegram_message_message_id_telegram_channel_id",
                table: "TelegramMessage");

            migrationBuilder.DropIndex(
                name: "ix_telegram_media_document_file_id_telegram_message_id",
                table: "TelegramMediaDocument");

            migrationBuilder.CreateIndex(
                name: "ix_telegram_message_message_id",
                table: "TelegramMessage",
                column: "message_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_telegram_media_document_file_id",
                table: "TelegramMediaDocument",
                column: "file_id",
                unique: true);
        }
    }
}
