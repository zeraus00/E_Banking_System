using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddInterestAmountAndTransactionsLinkToLoansTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoanId",
                schema: "FinanceSchema",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestAmount",
                schema: "FinanceSchema",
                table: "Loans",
                type: "DECIMAL(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LoanId",
                schema: "FinanceSchema",
                table: "Transactions",
                column: "LoanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Loans_LoanId",
                schema: "FinanceSchema",
                table: "Transactions",
                column: "LoanId",
                principalSchema: "FinanceSchema",
                principalTable: "Loans",
                principalColumn: "LoanId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Loans_LoanId",
                schema: "FinanceSchema",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_LoanId",
                schema: "FinanceSchema",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "LoanId",
                schema: "FinanceSchema",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "InterestAmount",
                schema: "FinanceSchema",
                table: "Loans");
        }
    }
}
