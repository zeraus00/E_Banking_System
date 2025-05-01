namespace Data.Configurations.Finance
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        // Configure Accounts Table
        public void Configure(EntityTypeBuilder<Account> Accounts)
        {
            Accounts.ToTable("Accounts", "FinanceSchema");

            /*  Configure Table Properties  */

            //  AccountId (Primary Key)
            Accounts
                .HasKey(a => a.AccountId);
            Accounts
                .Property(a => a.AccountId)
                .ValueGeneratedOnAdd();

            //  AccountType (Required)
            Accounts
                .Property(a => a.AccountTypeId)
                .IsRequired();

            //  AccountNumber (Required; MaxLength=12; FixedLength)
            Accounts
                .Property(a => a.AccountNumber)
                .IsRequired()
                .HasMaxLength(12)
                .IsFixedLength();

            //  ATMNumber (Required; MaxLength=16; FixedLength)
            Accounts
                .Property(a => a.ATMNumber)
                .IsRequired()
                .HasMaxLength(16)
                .IsFixedLength();

            //  AccountName (Required; MaxLength=30)
            Accounts
                .Property(a => a.AccountName)
                .IsRequired()
                .HasMaxLength(30);

            //  AccountContactNo (Required; MaxLength=11; FixedLength)
            Accounts
                .Property(a => a.AccountContactNo)
                .IsRequired()
                .HasMaxLength(11)
                .IsFixedLength();

            //  AccountStatus (Required)
            Accounts
                .Property(a => a.AccountStatusTypeId)
                .IsRequired();

            //  Balance (Required; DECIMAL(18,2); Default 0.0m)
            Accounts
                .Property(a => a.Balance)
                .IsRequired()
                .HasColumnType("DECIMAL(18,2)")
                .HasDefaultValue(0.0m);

            //  DateOpened (Required; Default DateTime.UtcNow)
            Accounts
                .Property(a => a.DateOpened)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            //  DateClosed (Optional)
            Accounts
                .Property(a => a.DateClosed)
                .IsRequired(false);

            /*
             *  Relationships
             *  AccountTypes (many-to-one)
             *  AccountProductTypes (many-to-one)
             *  AccountStatusTypes (many-to-one)
             *  LinkedBeneficiary Accounts (self-referencing many-to-one)
             *  UsersAuth (many-to-many)
             *  Transactions (one-to-many)
             *  Loans (one-to-many)
             *  LoanTransactions (one-to-many)
             */

            Accounts
                .HasOne(a => a.AccountType)
                .WithMany(at => at.Accounts)
                .HasForeignKey(a => a.AccountTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            Accounts
                .HasOne(a => a.AccountProductType)
                .WithMany(apt => apt.Accounts)
                .HasForeignKey(a => a.AccountProductTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            Accounts
                .HasOne(a => a.AccountStatusType)
                .WithMany(ast => ast.Accounts)
                .HasForeignKey(a => a.AccountStatusTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            Accounts
                .HasOne(a => a.LinkedBeneficiaryAccount)
                .WithMany(b => b.LinkedSourceAccounts)
                .HasForeignKey(a => a.LinkedBeneficiaryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
