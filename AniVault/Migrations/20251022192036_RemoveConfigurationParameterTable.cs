using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AniVault.Migrations
{
    /// <inheritdoc />
    public partial class RemoveConfigurationParameterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "system_configuration_parameters");

            migrationBuilder.RenameColumn(
                name: "anime_folder_path",
                table: "AnimeConfiguration",
                newName: "anime_folder_relative_path");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "anime_folder_relative_path",
                table: "AnimeConfiguration",
                newName: "anime_folder_path");

            migrationBuilder.CreateTable(
                name: "system_configuration_parameters",
                columns: table => new
                {
                    configuration_parameter_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    parameter_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    parameter_type = table.Column<int>(type: "integer", nullable: false),
                    parameter_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_configuration_parameters", x => x.configuration_parameter_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_system_configuration_parameters_parameter_name",
                table: "system_configuration_parameters",
                column: "parameter_name",
                unique: true);
        }
    }
}
