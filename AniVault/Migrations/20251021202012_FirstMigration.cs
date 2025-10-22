using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AniVault.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnimeConfiguration",
                columns: table => new
                {
                    anime_configuration_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    anime_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    my_anime_list_id = table.Column<int>(type: "integer", nullable: true),
                    file_name_template = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    anime_folder_path = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    auto_download_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    episodes_number_offset = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_anime_configuration", x => x.anime_configuration_id);
                });

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

            migrationBuilder.CreateTable(
                name: "ScheduledTask",
                columns: table => new
                {
                    scheduled_task_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    task_name = table.Column<string>(type: "text", nullable: false),
                    last_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_finish = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    enabled = table.Column<bool>(type: "boolean", nullable: false),
                    interval_seconds = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_scheduled_task", x => x.scheduled_task_id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramChannel",
                columns: table => new
                {
                    telegram_channel_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chat_id = table.Column<long>(type: "bigint", nullable: false),
                    access_hash = table.Column<long>(type: "bigint", nullable: false),
                    channel_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    is_anime_channel = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_channel", x => x.telegram_channel_id);
                });

            migrationBuilder.CreateTable(
                name: "system_configuration_parameters",
                columns: table => new
                {
                    configuration_parameter_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parameter_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    parameter_value = table.Column<string>(type: "text", nullable: true),
                    parameter_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_configuration_parameters", x => x.configuration_parameter_id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramMessage",
                columns: table => new
                {
                    telegram_message_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message_id = table.Column<int>(type: "integer", nullable: false),
                    message_text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    received_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    message_status = table.Column<int>(type: "integer", nullable: false),
                    telegram_channel_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_message", x => x.telegram_message_id);
                    table.ForeignKey(
                        name: "fk_telegram_message_telegram_channel_telegram_channel_id",
                        column: x => x.telegram_channel_id,
                        principalTable: "TelegramChannel",
                        principalColumn: "telegram_channel_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelegramMediaDocument",
                columns: table => new
                {
                    telegram_media_document_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_id = table.Column<long>(type: "bigint", nullable: false),
                    access_hash = table.Column<long>(type: "bigint", nullable: false),
                    file_reference = table.Column<byte[]>(type: "bytea", nullable: false),
                    telegram_message_id = table.Column<int>(type: "integer", nullable: false),
                    anime_configuration_id = table.Column<int>(type: "integer", nullable: false),
                    filename = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    filename_from_telegram = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    filename_to_update = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    mime_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    data_transmitted = table.Column<long>(type: "bigint", nullable: false),
                    creation_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_update_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    download_status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_media_document", x => x.telegram_media_document_id);
                    table.ForeignKey(
                        name: "fk_telegram_media_document_anime_configuration_anime_configuratio",
                        column: x => x.anime_configuration_id,
                        principalTable: "AnimeConfiguration",
                        principalColumn: "anime_configuration_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_telegram_media_document_telegram_message_telegram_message_id",
                        column: x => x.telegram_message_id,
                        principalTable: "TelegramMessage",
                        principalColumn: "telegram_message_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_anime_configuration_anime_name",
                table: "AnimeConfiguration",
                column: "anime_name");

            migrationBuilder.CreateIndex(
                name: "ix_anime_configuration_my_anime_list_id",
                table: "AnimeConfiguration",
                column: "my_anime_list_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_api_user_api_key",
                table: "ApiUser",
                column: "api_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_telegram_channel_chat_id",
                table: "TelegramChannel",
                column: "chat_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_telegram_media_document_anime_configuration_id",
                table: "TelegramMediaDocument",
                column: "anime_configuration_id");

            migrationBuilder.CreateIndex(
                name: "ix_telegram_media_document_file_id_telegram_message_id",
                table: "TelegramMediaDocument",
                columns: new[] { "file_id", "telegram_message_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_telegram_media_document_telegram_message_id",
                table: "TelegramMediaDocument",
                column: "telegram_message_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_telegram_message_message_id_telegram_channel_id",
                table: "TelegramMessage",
                columns: new[] { "message_id", "telegram_channel_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_telegram_message_telegram_channel_id",
                table: "TelegramMessage",
                column: "telegram_channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_configuration_parameters_parameter_name",
                table: "system_configuration_parameters",
                column: "parameter_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiUser");

            migrationBuilder.DropTable(
                name: "ScheduledTask");

            migrationBuilder.DropTable(
                name: "TelegramMediaDocument");

            migrationBuilder.DropTable(
                name: "system_configuration_parameters");

            migrationBuilder.DropTable(
                name: "AnimeConfiguration");

            migrationBuilder.DropTable(
                name: "TelegramMessage");

            migrationBuilder.DropTable(
                name: "TelegramChannel");
        }
    }
}
