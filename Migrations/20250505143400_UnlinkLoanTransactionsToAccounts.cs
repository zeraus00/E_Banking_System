using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class UnlinkLoanTransactionsToAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanTransactions_Accounts_AccountId",
                schema: "FinanceSchema",
                table: "LoanTransactions");

            migrationBuilder.DropIndex(
                name: "IX_LoanTransactions_AccountId",
                schema: "FinanceSchema",
                table: "LoanTransactions");

            migrationBuilder.DropColumn(
                name: "AccountId",
                schema: "FinanceSchema",
                table: "LoanTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                schema: "FinanceSchema",
                table: "LoanTransactions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                schema: "FinanceSchema",
                table: "LoanTransactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                schema: "FinanceSchema",
                table: "LoanTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LoanTransactions_AccountId",
                schema: "FinanceSchema",
                table: "LoanTransactions",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanTransactions_Accounts_AccountId",
                schema: "FinanceSchema",
                table: "LoanTransactions",
                column: "AccountId",
                principalSchema: "FinanceSchema",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
