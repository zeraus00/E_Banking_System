using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationDateToDateTimeDataTypeLoansTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ApplicationDate",
                schema: "FinanceSchema",
                table: "Loans",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldDefaultValueSql: "CAST(GETDATE() AS DATE)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ApplicationDate",
                schema: "FinanceSchema",
                table: "Loans",
                type: "DATE",
                nullable: false,
                defaultValueSql: "CAST(GETDATE() AS DATE)",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");
        }
    }
}
