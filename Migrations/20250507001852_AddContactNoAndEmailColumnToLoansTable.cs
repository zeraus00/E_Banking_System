using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddContactNoAndEmailColumnToLoansTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactNo",
                schema: "FinanceSchema",
                table: "Loans",
                type: "VARCHAR(11)",
                fixedLength: true,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "FinanceSchema",
                table: "Loans",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNo",
                schema: "FinanceSchema",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "FinanceSchema",
                table: "Loans");
        }
    }
}
