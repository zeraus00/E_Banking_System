using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class renameInterestRateColumnInLoanTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InterestRatePerAnnum",
                schema: "FinanceSchema",
                table: "LoanTypes",
                newName: "InterestRate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InterestRate",
                schema: "FinanceSchema",
                table: "LoanTypes",
                newName: "InterestRatePerAnnum");
        }
    }
}
