using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeBite_Backend_H6.API.Data.Migrations.App
{
    /// <inheritdoc />
    public partial class AddScannedIngredientsTextToScan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScannedIngredientsText",
                table: "Scans",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScannedIngredientsText",
                table: "Scans");
        }
    }
}
