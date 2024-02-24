using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixelPantry.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ChangeShit2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HardDriveId",
                table: "Games",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HardDriveId",
                table: "Franchises",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_HardDriveId",
                table: "Games",
                column: "HardDriveId");

            migrationBuilder.CreateIndex(
                name: "IX_Franchises_HardDriveId",
                table: "Franchises",
                column: "HardDriveId");

            migrationBuilder.AddForeignKey(
                name: "FK_Franchises_HardDrives_HardDriveId",
                table: "Franchises",
                column: "HardDriveId",
                principalTable: "HardDrives",
                principalColumn: "HardDriveId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_HardDrives_HardDriveId",
                table: "Games",
                column: "HardDriveId",
                principalTable: "HardDrives",
                principalColumn: "HardDriveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Franchises_HardDrives_HardDriveId",
                table: "Franchises");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_HardDrives_HardDriveId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_HardDriveId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Franchises_HardDriveId",
                table: "Franchises");

            migrationBuilder.DropColumn(
                name: "HardDriveId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HardDriveId",
                table: "Franchises");
        }
    }
}
