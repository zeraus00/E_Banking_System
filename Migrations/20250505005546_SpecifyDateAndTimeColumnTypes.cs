using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class SpecifyDateAndTimeColumnTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TransactionTime",
                schema: "FinanceSchema",
                table: "Transactions",
                type: "TIME",
                nullable: false,
                defaultValueSql: "CAST(GETDATE() AS TIME)",
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldDefaultValueSql: "CAST(GETDATE() AS TIME)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                schema: "FinanceSchema",
                table: "Transactions",
                type: "DATE",
                nullable: false,
                defaultValueSql: "CAST(GETDATE() AS DATE)",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TransactionTime",
                schema: "FinanceSchema",
                table: "LoanTransactions",
                type: "TIME",
                nullable: false,
                defaultValueSql: "CAST(GETDATE() AS TIME)",
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldDefaultValueSql: "CAST(GETDATE() AS TIME)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                schema: "FinanceSchema",
                table: "LoanTransactions",
                type: "DATE",
                nullable: false,
                defaultValueSql: "CAST(GETDATE() AS DATE)",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                schema: "FinanceSchema",
                table: "LoanTransactions",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

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

            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                schema: "UserSchema",
                table: "BirthsInfo",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOpened",
                schema: "FinanceSchema",
                table: "Accounts",
                type: "DATE",
                nullable: false,
                defaultValueSql: "CAST(GETDATE() AS DATE)",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateClosed",
                schema: "FinanceSchema",
                table: "Accounts",
                type: "DATE",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TransactionTime",
                schema: "FinanceSchema",
                table: "Transactions",
                type: "time",
                nullable: false,
                defaultValueSql: "CAST(GETDATE() AS TIME)",
                oldClrType: typeof(TimeSpan),
                oldType: "TIME",
                oldDefaultValueSql: "CAST(GETDATE() AS TIME)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                schema: "FinanceSchema",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldDefaultValueSql: "CAST(GETDATE() AS DATE)");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TransactionTime",
                schema: "FinanceSchema",
                table: "LoanTransactions",
                type: "time",
                nullable: false,
                defaultValueSql: "CAST(GETDATE() AS TIME)",
                oldClrType: typeof(TimeSpan),
                oldType: "TIME",
                oldDefaultValueSql: "CAST(GETDATE() AS TIME)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransactionDate",
                schema: "FinanceSchema",
                table: "LoanTransactions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldDefaultValueSql: "CAST(GETDATE() AS DATE)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                schema: "FinanceSchema",
                table: "LoanTransactions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

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

            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                schema: "UserSchema",
                table: "BirthsInfo",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOpened",
                schema: "FinanceSchema",
                table: "Accounts",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldDefaultValueSql: "CAST(GETDATE() AS DATE)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateClosed",
                schema: "FinanceSchema",
                table: "Accounts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldNullable: true);
        }
    }
}
