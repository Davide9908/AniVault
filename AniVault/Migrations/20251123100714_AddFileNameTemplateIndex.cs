using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniVault.Migrations
{
    /// <inheritdoc />
    public partial class AddFileNameTemplateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_anime_configuration_file_name_template",
                table: "anime_configuration",
                column: "file_name_template");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_anime_configuration_file_name_template",
                table: "anime_configuration");
        }
    }
}
