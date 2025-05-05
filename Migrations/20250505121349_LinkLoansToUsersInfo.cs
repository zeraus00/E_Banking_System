using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class LinkLoansToUsersInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserInfoId",
                schema: "FinanceSchema",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_UserInfoId",
                schema: "FinanceSchema",
                table: "Loans",
                column: "UserInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_UsersInfo_UserInfoId",
                schema: "FinanceSchema",
                table: "Loans",
                column: "UserInfoId",
                principalSchema: "UserSchema",
                principalTable: "UsersInfo",
                principalColumn: "UserInfoId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_UsersInfo_UserInfoId",
                schema: "FinanceSchema",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_UserInfoId",
                schema: "FinanceSchema",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "UserInfoId",
                schema: "FinanceSchema",
                table: "Loans");
        }
    }
}
