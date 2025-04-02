using Microsoft.EntityFrameworkCore;

namespace E_BankingSystem.Data.Configurations.Finance
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        // Configure Accounts Table
        public void Configure(EntityTypeBuilder<Account> Accounts)
        {
            Accounts.ToTable("Accounts", "Finance");

            /*  Configure Table Properties  */

            //  AccountId (Primary Key)
            Accounts
                .HasKey(a => a.AccountId);
            Accounts
                .Property(a => a.AccountId)
                .ValueGeneratedOnAdd();

            //  AccountType (Required; MaxLength=20)
            Accounts
                .Property(a => a.AccountType)
                .IsRequired()
                .HasMaxLength(20);

            //  AccountNumber (Required; MaxLength=12; FixedLength)
            Accounts
                .Property(a => a.AccountNumber)
                .IsRequired()
                .HasMaxLength(12)
                .IsFixedLength();

            //  AccountStatus (Required; MaxLength=10)
            Accounts
                .Property(a => a.AccountStatus)
                .IsRequired()
                .HasMaxLength(10);

            //  Balance (Required; Default 0)
            Accounts
                .Property(a => a.Balance)
                .IsRequired()
                .HasDefaultValue(0);

            //  DateOpened (Required; Default DateTime.UtcNow)
            Accounts
                .Property(a => a.DateOpened)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            //  DateClosed (Optional)
            Accounts
                .Property(a => a.DateClosed)
                .IsRequired(false);

            /*
             *  Configure Relationships
             *  CustomersAuth (one-to-many)
             *  Transactions (one-to-many)
             */
            Accounts
                .HasMany(a => a.CustomersAuth)
                .WithOne(ca => ca.Account)
                .HasForeignKey(ca => ca.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            Accounts
                .HasMany(a => a.Transactions)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
