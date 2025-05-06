using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMinimumLoanAmountColumnToLoanTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinimumLoanAmount",
                schema: "FinanceSchema",
                table: "LoanTypes",
                type: "Decimal(9,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumLoanAmount",
                schema: "FinanceSchema",
                table: "LoanTypes");
        }
    }
}
