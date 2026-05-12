using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeBite_Backend_H6.API.Data.Migrations.App
{
    /// <inheritdoc />
    public partial class CustomAllergyForScan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "AllergyId",
                table: "ScanDetectedAllergies",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomAllergyId",
                table: "ScanDetectedAllergies",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScanDetectedAllergies_CustomAllergyId",
                table: "ScanDetectedAllergies",
                column: "CustomAllergyId");

            migrationBuilder.CreateIndex(
                name: "IX_ScanDetectedAllergies_ScanId_CustomAllergyId",
                table: "ScanDetectedAllergies",
                columns: new[] { "ScanId", "CustomAllergyId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScanDetectedAllergies_CustomAllergies_CustomAllergyId",
                table: "ScanDetectedAllergies",
                column: "CustomAllergyId",
                principalTable: "CustomAllergies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScanDetectedAllergies_CustomAllergies_CustomAllergyId",
                table: "ScanDetectedAllergies");

            migrationBuilder.DropIndex(
                name: "IX_ScanDetectedAllergies_CustomAllergyId",
                table: "ScanDetectedAllergies");

            migrationBuilder.DropIndex(
                name: "IX_ScanDetectedAllergies_ScanId_CustomAllergyId",
                table: "ScanDetectedAllergies");

            migrationBuilder.DropColumn(
                name: "CustomAllergyId",
                table: "ScanDetectedAllergies");

            migrationBuilder.AlterColumn<Guid>(
                name: "AllergyId",
                table: "ScanDetectedAllergies",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
