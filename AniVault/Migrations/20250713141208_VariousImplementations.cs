using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AniVault.Migrations
{
    /// <inheritdoc />
    public partial class VariousImplementations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_telegram_media_document_file_id_access_hash",
                table: "TelegramMediaDocument");

            migrationBuilder.DropIndex(
                name: "ix_telegram_channel_chat_id_access_hash",
                table: "TelegramChannel");

            migrationBuilder.RenameColumn(
                name: "file_name",
                table: "TelegramMediaDocument",
                newName: "filename");

            migrationBuilder.AlterColumn<string>(
                name: "parameter_value",
                table: "system_configuration_parameters",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "parameter_name",
                table: "system_configuration_parameters",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "message_text",
                table: "TelegramMessage",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "message_status",
                table: "TelegramMessage",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "received_datetime",
                table: "TelegramMessage",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "update_datetime",
                table: "TelegramMessage",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "filename_from_telegram",
                table: "TelegramMediaDocument",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "filename_to_update",
                table: "TelegramMediaDocument",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "mime_type",
                table: "TelegramMediaDocument",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "interval_seconds",
                table: "ScheduledTask",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "ApiUser",
                columns: table => new
                {
                    api_user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    api_key = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_api_user", x => x.api_user_id);
                });

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

            migrationBuilder.CreateIndex(
                name: "ix_telegram_channel_chat_id",
                table: "TelegramChannel",
                column: "chat_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_api_user_api_key",
                table: "ApiUser",
                column: "api_key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiUser");

            migrationBuilder.DropIndex(
                name: "ix_telegram_message_message_id",
                table: "TelegramMessage");

            migrationBuilder.DropIndex(
                name: "ix_telegram_media_document_file_id",
                table: "TelegramMediaDocument");

            migrationBuilder.DropIndex(
                name: "ix_telegram_channel_chat_id",
                table: "TelegramChannel");

            migrationBuilder.DropColumn(
                name: "message_status",
                table: "TelegramMessage");

            migrationBuilder.DropColumn(
                name: "received_datetime",
                table: "TelegramMessage");

            migrationBuilder.DropColumn(
                name: "update_datetime",
                table: "TelegramMessage");

            migrationBuilder.DropColumn(
                name: "filename_from_telegram",
                table: "TelegramMediaDocument");

            migrationBuilder.DropColumn(
                name: "filename_to_update",
                table: "TelegramMediaDocument");

            migrationBuilder.DropColumn(
                name: "mime_type",
                table: "TelegramMediaDocument");

            migrationBuilder.RenameColumn(
                name: "filename",
                table: "TelegramMediaDocument",
                newName: "file_name");

            migrationBuilder.AlterColumn<string>(
                name: "parameter_value",
                table: "system_configuration_parameters",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "parameter_name",
                table: "system_configuration_parameters",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "message_text",
                table: "TelegramMessage",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<int>(
                name: "interval_seconds",
                table: "ScheduledTask",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_telegram_media_document_file_id_access_hash",
                table: "TelegramMediaDocument",
                columns: new[] { "file_id", "access_hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_telegram_channel_chat_id_access_hash",
                table: "TelegramChannel",
                columns: new[] { "chat_id", "access_hash" },
                unique: true);
        }
    }
}
