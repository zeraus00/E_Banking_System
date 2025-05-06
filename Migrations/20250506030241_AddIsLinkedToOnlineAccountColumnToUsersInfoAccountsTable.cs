using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddIsLinkedToOnlineAccountColumnToUsersInfoAccountsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLinkedToOnlineAccount",
                schema: "JoinEntitySchema",
                table: "UsersInfoAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLinkedToOnlineAccount",
                schema: "JoinEntitySchema",
                table: "UsersInfoAccounts");
        }
    }
}
