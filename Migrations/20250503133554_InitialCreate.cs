using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AuthSchema");

            migrationBuilder.EnsureSchema(
                name: "FinanceSchema");

            migrationBuilder.EnsureSchema(
                name: "UserSchema");

            migrationBuilder.EnsureSchema(
                name: "PlaceSchema");

            migrationBuilder.EnsureSchema(
                name: "JoinEntitySchema");

            migrationBuilder.CreateTable(
                name: "AccessRoles",
                schema: "AuthSchema",
                columns: table => new
                {
                    AccessRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessRoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRoles", x => x.AccessRoleId);
                });

            migrationBuilder.CreateTable(
                name: "AccountProductTypes",
                schema: "FinanceSchema",
                columns: table => new
                {
                    AccountProductTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountProductTypeName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountProductTypes", x => x.AccountProductTypeId);
                });

            migrationBuilder.CreateTable(
                name: "AccountStatusTypes",
                schema: "FinanceSchema",
                columns: table => new
                {
                    AccountStatusTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountStatusTypeName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatusTypes", x => x.AccountStatusTypeId);
                });

            migrationBuilder.CreateTable(
                name: "AccountTypes",
                schema: "FinanceSchema",
                columns: table => new
                {
                    AccountTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountTypeName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.AccountTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ExternalVendors",
                schema: "FinanceSchema",
                columns: table => new
                {
                    VendorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalVendors", x => x.VendorId);
                });

            migrationBuilder.CreateTable(
                name: "LoanTypes",
                schema: "FinanceSchema",
                columns: table => new
                {
                    LoanTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanTypeName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    InterestRatePerAnnum = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false),
                    LoanTermInMonths = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanTypes", x => x.LoanTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Names",
                schema: "UserSchema",
                columns: table => new
                {
                    NameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Suffix = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Names", x => x.NameId);
                });

            migrationBuilder.CreateTable(
                name: "Regions",
                schema: "PlaceSchema",
                columns: table => new
                {
                    RegionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegionName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.RegionId);
                });

            migrationBuilder.CreateTable(
                name: "Religions",
                schema: "UserSchema",
                columns: table => new
                {
                    ReligionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReligionName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Religions", x => x.ReligionId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "AuthSchema",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTypes",
                schema: "FinanceSchema",
                columns: table => new
                {
                    TransactionTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTypes", x => x.TransactionTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                schema: "FinanceSchema",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountTypeId = table.Column<int>(type: "int", nullable: false),
                    AccountProductTypeId = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "nchar(12)", fixedLength: true, maxLength: 12, nullable: false),
                    ATMNumber = table.Column<string>(type: "nchar(16)", fixedLength: true, maxLength: 16, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AccountContactNo = table.Column<string>(type: "nchar(11)", fixedLength: true, maxLength: 11, nullable: false),
                    AccountStatusTypeId = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false, defaultValue: 0.0m),
                    LinkedBeneficiaryId = table.Column<int>(type: "int", nullable: true),
                    DateOpened = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DateClosed = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountProductTypes_AccountProductTypeId",
                        column: x => x.AccountProductTypeId,
                        principalSchema: "FinanceSchema",
                        principalTable: "AccountProductTypes",
                        principalColumn: "AccountProductTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountStatusTypes_AccountStatusTypeId",
                        column: x => x.AccountStatusTypeId,
                        principalSchema: "FinanceSchema",
                        principalTable: "AccountStatusTypes",
                        principalColumn: "AccountStatusTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalSchema: "FinanceSchema",
                        principalTable: "AccountTypes",
                        principalColumn: "AccountTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Accounts_LinkedBeneficiaryId",
                        column: x => x.LinkedBeneficiaryId,
                        principalSchema: "FinanceSchema",
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                schema: "PlaceSchema",
                columns: table => new
                {
                    ProvinceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProvinceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProvinceName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.ProvinceId);
                    table.ForeignKey(
                        name: "FK_Provinces_Regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "PlaceSchema",
                        principalTable: "Regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UsersAuth",
                schema: "AuthSchema",
                columns: table => new
                {
                    UserAuthId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    UserName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersAuth", x => x.UserAuthId);
                    table.ForeignKey(
                        name: "FK_UsersAuth_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "AuthSchema",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                schema: "FinanceSchema",
                columns: table => new
                {
                    LoanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanNumber = table.Column<string>(type: "VARCHAR(20)", fixedLength: true, nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    LoanTypeId = table.Column<int>(type: "int", nullable: false),
                    LoanPurpose = table.Column<string>(type: "VARCHAR(30)", nullable: false),
                    LoanAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false),
                    LoanTermMonths = table.Column<int>(type: "int", nullable: false),
                    PaymentFrequency = table.Column<int>(type: "int", nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    RemainingLoanBalance = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    LoanStatus = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "Pending"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.LoanId);
                    table.ForeignKey(
                        name: "FK_Loans_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "FinanceSchema",
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Loans_LoanTypes_LoanTypeId",
                        column: x => x.LoanTypeId,
                        principalSchema: "FinanceSchema",
                        principalTable: "LoanTypes",
                        principalColumn: "LoanTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "FinanceSchema",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionTypeId = table.Column<int>(type: "int", nullable: false),
                    TransactionNumber = table.Column<string>(type: "nchar(32)", fixedLength: true, maxLength: 32, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ConfirmationNumber = table.Column<string>(type: "nchar(28)", fixedLength: true, maxLength: 28, nullable: true),
                    MainAccountId = table.Column<int>(type: "int", fixedLength: true, maxLength: 28, nullable: false),
                    CounterAccountId = table.Column<int>(type: "int", nullable: true),
                    ExternalVendorId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    PreviousBalance = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    NewBalance = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    TransactionTime = table.Column<TimeSpan>(type: "time", nullable: false, defaultValueSql: "CAST(GETDATE() AS TIME)"),
                    TransactionFee = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false, defaultValue: 0.0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_CounterAccountId",
                        column: x => x.CounterAccountId,
                        principalSchema: "FinanceSchema",
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_MainAccountId",
                        column: x => x.MainAccountId,
                        principalSchema: "FinanceSchema",
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_ExternalVendors_ExternalVendorId",
                        column: x => x.ExternalVendorId,
                        principalSchema: "FinanceSchema",
                        principalTable: "ExternalVendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_TransactionTypes_TransactionTypeId",
                        column: x => x.TransactionTypeId,
                        principalSchema: "FinanceSchema",
                        principalTable: "TransactionTypes",
                        principalColumn: "TransactionTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                schema: "PlaceSchema",
                columns: table => new
                {
                    CityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProvinceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.CityId);
                    table.ForeignKey(
                        name: "FK_Cities_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalSchema: "PlaceSchema",
                        principalTable: "Provinces",
                        principalColumn: "ProvinceId",
                        onDelete: ReferentialAction.SetNull);
                });

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

            migrationBuilder.CreateTable(
                name: "LoanTransactions",
                schema: "FinanceSchema",
                columns: table => new
                {
                    LoanTransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    LoanId = table.Column<int>(type: "int", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    RemainingLoanBalance = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    InterestAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    PrincipalAmount = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    TransactionTime = table.Column<TimeSpan>(type: "time", nullable: false, defaultValueSql: "CAST(GETDATE() AS TIME)"),
                    Notes = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanTransactions", x => x.LoanTransactionId);
                    table.ForeignKey(
                        name: "FK_LoanTransactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "FinanceSchema",
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanTransactions_Loans_LoanId",
                        column: x => x.LoanId,
                        principalSchema: "FinanceSchema",
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Barangays",
                schema: "PlaceSchema",
                columns: table => new
                {
                    BarangayId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarangayCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BarangayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barangays", x => x.BarangayId);
                    table.ForeignKey(
                        name: "FK_Barangays_Cities_CityId",
                        column: x => x.CityId,
                        principalSchema: "PlaceSchema",
                        principalTable: "Cities",
                        principalColumn: "CityId");
                });

            migrationBuilder.CreateTable(
                name: "BirthsInfo",
                schema: "UserSchema",
                columns: table => new
                {
                    BirthInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: true),
                    RegionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirthsInfo", x => x.BirthInfoId);
                    table.ForeignKey(
                        name: "FK_BirthsInfo_Cities_CityId",
                        column: x => x.CityId,
                        principalSchema: "PlaceSchema",
                        principalTable: "Cities",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BirthsInfo_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalSchema: "PlaceSchema",
                        principalTable: "Provinces",
                        principalColumn: "ProvinceId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BirthsInfo_Regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "PlaceSchema",
                        principalTable: "Regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "UserSchema",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    House = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Street = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BarangayId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    ProvinceId = table.Column<int>(type: "int", nullable: true),
                    RegionId = table.Column<int>(type: "int", nullable: true),
                    PostalCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Addresses_Barangays_BarangayId",
                        column: x => x.BarangayId,
                        principalSchema: "PlaceSchema",
                        principalTable: "Barangays",
                        principalColumn: "BarangayId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Addresses_Cities_CityId",
                        column: x => x.CityId,
                        principalSchema: "PlaceSchema",
                        principalTable: "Cities",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Addresses_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalSchema: "PlaceSchema",
                        principalTable: "Provinces",
                        principalColumn: "ProvinceId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Addresses_Regions_RegionId",
                        column: x => x.RegionId,
                        principalSchema: "PlaceSchema",
                        principalTable: "Regions",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UsersInfo",
                schema: "UserSchema",
                columns: table => new
                {
                    UserInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAuthId = table.Column<int>(type: "int", nullable: false),
                    UserNameId = table.Column<int>(type: "int", nullable: false),
                    ProfilePicture = table.Column<byte[]>(type: "VARBINARY(MAX)", maxLength: 1048576, nullable: true),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Sex = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BirthInfoId = table.Column<int>(type: "int", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    FatherNameId = table.Column<int>(type: "int", nullable: false),
                    MotherNameId = table.Column<int>(type: "int", nullable: false),
                    ContactNumber = table.Column<string>(type: "nchar(11)", fixedLength: true, maxLength: 11, nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GovernmentId = table.Column<byte[]>(type: "VARBINARY(MAX)", maxLength: 1048576, nullable: true),
                    TaxIdentificationNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CivilStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReligionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersInfo", x => x.UserInfoId);
                    table.ForeignKey(
                        name: "FK_UsersInfo_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "UserSchema",
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UsersInfo_BirthsInfo_BirthInfoId",
                        column: x => x.BirthInfoId,
                        principalSchema: "UserSchema",
                        principalTable: "BirthsInfo",
                        principalColumn: "BirthInfoId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UsersInfo_Names_FatherNameId",
                        column: x => x.FatherNameId,
                        principalSchema: "UserSchema",
                        principalTable: "Names",
                        principalColumn: "NameId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersInfo_Names_MotherNameId",
                        column: x => x.MotherNameId,
                        principalSchema: "UserSchema",
                        principalTable: "Names",
                        principalColumn: "NameId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersInfo_Names_UserNameId",
                        column: x => x.UserNameId,
                        principalSchema: "UserSchema",
                        principalTable: "Names",
                        principalColumn: "NameId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersInfo_Religions_ReligionId",
                        column: x => x.ReligionId,
                        principalSchema: "UserSchema",
                        principalTable: "Religions",
                        principalColumn: "ReligionId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UsersInfo_UsersAuth_UserAuthId",
                        column: x => x.UserAuthId,
                        principalSchema: "AuthSchema",
                        principalTable: "UsersAuth",
                        principalColumn: "UserAuthId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersInfoAccounts",
                schema: "JoinEntitySchema",
                columns: table => new
                {
                    UserInfoId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    AccessRoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersInfoAccounts", x => new { x.UserInfoId, x.AccountId });
                    table.ForeignKey(
                        name: "FK_UsersInfoAccounts_AccessRoles_AccessRoleId",
                        column: x => x.AccessRoleId,
                        principalSchema: "AuthSchema",
                        principalTable: "AccessRoles",
                        principalColumn: "AccessRoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersInfoAccounts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "FinanceSchema",
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersInfoAccounts_UsersInfo_UserInfoId",
                        column: x => x.UserInfoId,
                        principalSchema: "UserSchema",
                        principalTable: "UsersInfo",
                        principalColumn: "UserInfoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "FinanceSchema",
                table: "ExternalVendors",
                columns: new[] { "VendorId", "VendorName" },
                values: new object[,]
                {
                    { 1, "GCash" },
                    { 2, "Paymaya" },
                    { 3, "GoTyme" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountProductTypeId",
                schema: "FinanceSchema",
                table: "Accounts",
                column: "AccountProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountStatusTypeId",
                schema: "FinanceSchema",
                table: "Accounts",
                column: "AccountStatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountTypeId",
                schema: "FinanceSchema",
                table: "Accounts",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_LinkedBeneficiaryId",
                schema: "FinanceSchema",
                table: "Accounts",
                column: "LinkedBeneficiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_BarangayId",
                schema: "UserSchema",
                table: "Addresses",
                column: "BarangayId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CityId",
                schema: "UserSchema",
                table: "Addresses",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_ProvinceId",
                schema: "UserSchema",
                table: "Addresses",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_RegionId",
                schema: "UserSchema",
                table: "Addresses",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Barangays_CityId",
                schema: "PlaceSchema",
                table: "Barangays",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_BirthsInfo_CityId",
                schema: "UserSchema",
                table: "BirthsInfo",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_BirthsInfo_ProvinceId",
                schema: "UserSchema",
                table: "BirthsInfo",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_BirthsInfo_RegionId",
                schema: "UserSchema",
                table: "BirthsInfo",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProvinceId",
                schema: "PlaceSchema",
                table: "Cities",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_AccountId",
                schema: "FinanceSchema",
                table: "Loans",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_LoanTypeId",
                schema: "FinanceSchema",
                table: "Loans",
                column: "LoanTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanTransactions_AccountId",
                schema: "FinanceSchema",
                table: "LoanTransactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanTransactions_LoanId",
                schema: "FinanceSchema",
                table: "LoanTransactions",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_RegionId",
                schema: "PlaceSchema",
                table: "Provinces",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CounterAccountId",
                schema: "FinanceSchema",
                table: "Transactions",
                column: "CounterAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ExternalVendorId",
                schema: "FinanceSchema",
                table: "Transactions",
                column: "ExternalVendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_MainAccountId",
                schema: "FinanceSchema",
                table: "Transactions",
                column: "MainAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionTypeId",
                schema: "FinanceSchema",
                table: "Transactions",
                column: "TransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAuthAccount_UserAuthId",
                table: "UserAuthAccount",
                column: "UserAuthId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersAuth_Email",
                schema: "AuthSchema",
                table: "UsersAuth",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersAuth_RoleId",
                schema: "AuthSchema",
                table: "UsersAuth",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersAuth_UserName",
                schema: "AuthSchema",
                table: "UsersAuth",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfo_AddressId",
                schema: "UserSchema",
                table: "UsersInfo",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfo_BirthInfoId",
                schema: "UserSchema",
                table: "UsersInfo",
                column: "BirthInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfo_FatherNameId",
                schema: "UserSchema",
                table: "UsersInfo",
                column: "FatherNameId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfo_MotherNameId",
                schema: "UserSchema",
                table: "UsersInfo",
                column: "MotherNameId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfo_ReligionId",
                schema: "UserSchema",
                table: "UsersInfo",
                column: "ReligionId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfo_UserAuthId",
                schema: "UserSchema",
                table: "UsersInfo",
                column: "UserAuthId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfo_UserNameId",
                schema: "UserSchema",
                table: "UsersInfo",
                column: "UserNameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfoAccounts_AccessRoleId",
                schema: "JoinEntitySchema",
                table: "UsersInfoAccounts",
                column: "AccessRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInfoAccounts_AccountId",
                schema: "JoinEntitySchema",
                table: "UsersInfoAccounts",
                column: "AccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanTransactions",
                schema: "FinanceSchema");

            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "FinanceSchema");

            migrationBuilder.DropTable(
                name: "UserAuthAccount");

            migrationBuilder.DropTable(
                name: "UsersInfoAccounts",
                schema: "JoinEntitySchema");

            migrationBuilder.DropTable(
                name: "Loans",
                schema: "FinanceSchema");

            migrationBuilder.DropTable(
                name: "ExternalVendors",
                schema: "FinanceSchema");

            migrationBuilder.DropTable(
                name: "TransactionTypes",
                schema: "FinanceSchema");

            migrationBuilder.DropTable(
                name: "AccessRoles",
                schema: "AuthSchema");

            migrationBuilder.DropTable(
                name: "UsersInfo",
                schema: "UserSchema");

            migrationBuilder.DropTable(
                name: "Accounts",
                schema: "FinanceSchema");

            migrationBuilder.DropTable(
                name: "LoanTypes",
                schema: "FinanceSchema");

            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "UserSchema");

            migrationBuilder.DropTable(
                name: "BirthsInfo",
                schema: "UserSchema");

            migrationBuilder.DropTable(
                name: "Names",
                schema: "UserSchema");

            migrationBuilder.DropTable(
                name: "Religions",
                schema: "UserSchema");

            migrationBuilder.DropTable(
                name: "UsersAuth",
                schema: "AuthSchema");

            migrationBuilder.DropTable(
                name: "AccountProductTypes",
                schema: "FinanceSchema");

            migrationBuilder.DropTable(
                name: "AccountStatusTypes",
                schema: "FinanceSchema");

            migrationBuilder.DropTable(
                name: "AccountTypes",
                schema: "FinanceSchema");

            migrationBuilder.DropTable(
                name: "Barangays",
                schema: "PlaceSchema");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "AuthSchema");

            migrationBuilder.DropTable(
                name: "Cities",
                schema: "PlaceSchema");

            migrationBuilder.DropTable(
                name: "Provinces",
                schema: "PlaceSchema");

            migrationBuilder.DropTable(
                name: "Regions",
                schema: "PlaceSchema");
        }
    }
}
