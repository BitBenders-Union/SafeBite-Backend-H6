using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeBite_Backend_H6.API.Data.Migrations.App
{
    /// <inheritdoc />
    public partial class CustomAllergy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomAllergies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomAllergies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomAllergies_UserId_Name",
                table: "CustomAllergies",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomAllergies");
        }
    }
}
