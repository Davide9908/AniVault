using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AniVault.Migrations
{
    /// <inheritdoc />
    public partial class FirstCreateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    interval_seconds = table.Column<int>(type: "integer", nullable: false)
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
                    channel_name = table.Column<string>(type: "text", nullable: false),
                    auto_download_enabled = table.Column<bool>(type: "boolean", nullable: false),
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
                    parameter_name = table.Column<string>(type: "text", nullable: false),
                    parameter_value = table.Column<string>(type: "text", nullable: false),
                    parameter_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_configuration_parameters", x => x.configuration_parameter_id);
                });

            migrationBuilder.CreateTable(
                name: "AnimeEpisodesSetting",
                columns: table => new
                {
                    mal_anime_id = table.Column<int>(type: "integer", nullable: false),
                    telegram_channel_id = table.Column<int>(type: "integer", nullable: false),
                    file_name_template = table.Column<string>(type: "text", nullable: true),
                    anime_folder_path = table.Column<string>(type: "text", nullable: true),
                    download_last_episode = table.Column<bool>(type: "boolean", nullable: false),
                    cour_episode_number_gap = table.Column<short>(type: "smallint", nullable: true),
                    use_gap_for_ep_num = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_anime_episodes_setting", x => x.mal_anime_id);
                    table.ForeignKey(
                        name: "fk_anime_episodes_setting_telegram_channel_telegram_channel_id",
                        column: x => x.telegram_channel_id,
                        principalTable: "TelegramChannel",
                        principalColumn: "telegram_channel_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelegramMessage",
                columns: table => new
                {
                    telegram_message_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message_id = table.Column<int>(type: "integer", nullable: false),
                    message_text = table.Column<string>(type: "text", nullable: false),
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
                    telegram_message_id = table.Column<int>(type: "integer", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    data_transmitted = table.Column<long>(type: "bigint", nullable: false),
                    last_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    download_status = table.Column<int>(type: "integer", nullable: false),
                    error_type = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_telegram_media_document", x => x.telegram_media_document_id);
                    table.ForeignKey(
                        name: "fk_telegram_media_document_telegram_message_telegram_message_id",
                        column: x => x.telegram_message_id,
                        principalTable: "TelegramMessage",
                        principalColumn: "telegram_message_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_anime_episodes_setting_telegram_channel_id",
                table: "AnimeEpisodesSetting",
                column: "telegram_channel_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_telegram_channel_chat_id_access_hash",
                table: "TelegramChannel",
                columns: new[] { "chat_id", "access_hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_telegram_media_document_file_id_access_hash",
                table: "TelegramMediaDocument",
                columns: new[] { "file_id", "access_hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_telegram_media_document_telegram_message_id",
                table: "TelegramMediaDocument",
                column: "telegram_message_id",
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
                name: "AnimeEpisodesSetting");

            migrationBuilder.DropTable(
                name: "ScheduledTask");

            migrationBuilder.DropTable(
                name: "TelegramMediaDocument");

            migrationBuilder.DropTable(
                name: "system_configuration_parameters");

            migrationBuilder.DropTable(
                name: "TelegramMessage");

            migrationBuilder.DropTable(
                name: "TelegramChannel");
        }
    }
}
