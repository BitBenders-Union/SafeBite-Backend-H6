using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeBite_Backend_H6.API.Data.Migrations.App
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Allergies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allergies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ScannedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AllergyUsers",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    AllergyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllergyUsers", x => new { x.UserId, x.AllergyId });
                    table.ForeignKey(
                        name: "FK_AllergyUsers_Allergies_AllergyId",
                        column: x => x.AllergyId,
                        principalTable: "Allergies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScanDetectedAllergies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScanId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllergyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScanDetectedAllergies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScanDetectedAllergies_Allergies_AllergyId",
                        column: x => x.AllergyId,
                        principalTable: "Allergies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScanDetectedAllergies_Scans_ScanId",
                        column: x => x.ScanId,
                        principalTable: "Scans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetectedIngredientMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScanDetectedAllergyId = table.Column<Guid>(type: "uuid", nullable: false),
                    IngredientText = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetectedIngredientMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetectedIngredientMatches_ScanDetectedAllergies_ScanDetecte~",
                        column: x => x.ScanDetectedAllergyId,
                        principalTable: "ScanDetectedAllergies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Allergies_Name",
                table: "Allergies",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AllergyUsers_AllergyId",
                table: "AllergyUsers",
                column: "AllergyId");

            migrationBuilder.CreateIndex(
                name: "IX_AllergyUsers_UserId",
                table: "AllergyUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DetectedIngredientMatches_ScanDetectedAllergyId_IngredientT~",
                table: "DetectedIngredientMatches",
                columns: new[] { "ScanDetectedAllergyId", "IngredientText" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScanDetectedAllergies_AllergyId",
                table: "ScanDetectedAllergies",
                column: "AllergyId");

            migrationBuilder.CreateIndex(
                name: "IX_ScanDetectedAllergies_ScanId_AllergyId",
                table: "ScanDetectedAllergies",
                columns: new[] { "ScanId", "AllergyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scans_ScannedAt",
                table: "Scans",
                column: "ScannedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Scans_UserId",
                table: "Scans",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllergyUsers");

            migrationBuilder.DropTable(
                name: "DetectedIngredientMatches");

            migrationBuilder.DropTable(
                name: "ScanDetectedAllergies");

            migrationBuilder.DropTable(
                name: "Allergies");

            migrationBuilder.DropTable(
                name: "Scans");
        }
    }
}
