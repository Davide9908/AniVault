using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniVault.Migrations
{
    /// <inheritdoc />
    public partial class NewColumnRetries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "retries",
                table: "telegram_media_document",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "retries",
                table: "telegram_media_document");
        }
    }
}
