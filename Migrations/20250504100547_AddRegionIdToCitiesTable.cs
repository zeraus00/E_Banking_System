using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddRegionIdToCitiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                schema: "PlaceSchema",
                table: "Cities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_RegionId",
                schema: "PlaceSchema",
                table: "Cities",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Regions_RegionId",
                schema: "PlaceSchema",
                table: "Cities",
                column: "RegionId",
                principalSchema: "PlaceSchema",
                principalTable: "Regions",
                principalColumn: "RegionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Regions_RegionId",
                schema: "PlaceSchema",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_RegionId",
                schema: "PlaceSchema",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "RegionId",
                schema: "PlaceSchema",
                table: "Cities");
        }
    }
}
