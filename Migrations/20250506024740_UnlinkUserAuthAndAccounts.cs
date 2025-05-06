using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class UnlinkUserAuthAndAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAuthAccount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAuthAccount",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    UserAuthId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAuthAccount", x => new { x.AccountId, x.UserAuthId });
                    table.ForeignKey(
                        name: "FK_UserAuthAccount_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "FinanceSchema",
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAuthAccount_UsersAuth_UserAuthId",
                        column: x => x.UserAuthId,
                        principalSchema: "AuthSchema",
                        principalTable: "UsersAuth",
                        principalColumn: "UserAuthId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAuthAccount_UserAuthId",
                table: "UserAuthAccount",
                column: "UserAuthId");
        }
    }
}
