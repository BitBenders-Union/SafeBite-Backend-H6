using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeBite_Backend_H6.API.Data.Migrations.App
{
    /// <inheritdoc />
    public partial class NormalizedNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomAllergies_UserId_Name",
                table: "CustomAllergies");

            migrationBuilder.DropIndex(
                name: "IX_Allergies_Name",
                table: "Allergies");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "CustomAllergies",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Allergies",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CustomAllergies_UserId_NormalizedName",
                table: "CustomAllergies",
                columns: new[] { "UserId", "NormalizedName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Allergies_NormalizedName",
                table: "Allergies",
                column: "NormalizedName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomAllergies_UserId_NormalizedName",
                table: "CustomAllergies");

            migrationBuilder.DropIndex(
                name: "IX_Allergies_NormalizedName",
                table: "Allergies");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "CustomAllergies");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Allergies");

            migrationBuilder.CreateIndex(
                name: "IX_CustomAllergies_UserId_Name",
                table: "CustomAllergies",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Allergies_Name",
                table: "Allergies",
                column: "Name",
                unique: true);
        }
    }
}
