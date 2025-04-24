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

            //  AccountName (Required; MaxLength=30)
            Accounts
                .Property(a => a.AccountName)
                .IsRequired()
                .HasMaxLength(30);

            //  AccountStatus (Required; MaxLength=10)
            Accounts
                .Property(a => a.AccountStatus)
                .IsRequired()
                .HasMaxLength(10);

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
                .HasOne(a => a.LinkedBeneficiaryAccount)
                .WithMany(b => b.LinkedSourceAccounts)
                .HasForeignKey(a => a.LinkedBeneficiaryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
