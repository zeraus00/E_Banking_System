using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPayslipPictureColumnToUsersInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PayslipPicture",
                schema: "UserSchema",
                table: "UsersInfo",
                type: "VARBINARY(MAX)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayslipPicture",
                schema: "UserSchema",
                table: "UsersInfo");
        }
    }
}
