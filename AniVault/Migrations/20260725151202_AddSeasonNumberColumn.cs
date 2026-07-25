using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniVault.Migrations
{
    /// <inheritdoc />
    public partial class AddSeasonNumberColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "season_number",
                table: "anime_configuration",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "season_number",
                table: "anime_configuration");
        }
    }
}
