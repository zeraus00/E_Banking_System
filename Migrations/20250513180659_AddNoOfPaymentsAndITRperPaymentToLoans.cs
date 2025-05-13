using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddNoOfPaymentsAndITRperPaymentToLoans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InterestRatePerPayment",
                schema: "FinanceSchema",
                table: "Loans",
                type: "DECIMAL(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPayments",
                schema: "FinanceSchema",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestRatePerPayment",
                schema: "FinanceSchema",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "NumberOfPayments",
                schema: "FinanceSchema",
                table: "Loans");
        }
    }
}
