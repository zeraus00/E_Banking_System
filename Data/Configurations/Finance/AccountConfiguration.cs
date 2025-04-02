using Microsoft.EntityFrameworkCore;

namespace E_BankingSystem.Data.Configurations.Finance
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        // Configure Accounts Table
        public void Configure(EntityTypeBuilder<Account> Accounts)
        {
            Accounts.ToTable("Accounts", "Finance");
            // Define Primary Key
            Accounts
                .HasKey(a => a.AccountId);
            Accounts
                .Property(a => a.AccountId)
                .ValueGeneratedOnAdd();
            // Define Required Constraint for AccountType
            Accounts
                .Property(a => a.AccountType)
                .IsRequired()
                .HasMaxLength(20);
            // Define Required Constraint for AccountNumber
            Accounts
                .Property(a => a.AccountNumber)
                .IsRequired()
                .HasMaxLength(12)
                .IsFixedLength();
            // Define Required Constraint for AccountStatus
            Accounts
                .Property(a => a.AccountStatus)
                .IsRequired()
                .HasMaxLength(10);
            // Define Required Constraint for Balance
            Accounts
                .Property(a => a.Balance)
                .IsRequired()
                .HasDefaultValue(0);
            // Define Required Constraint for DateOpened
            Accounts
                .Property(a => a.DateOpened)
                .IsRequired()
                .HasDefaultValueSql("NOW()");
            // Define Optional Constraint for DateClosed
            Accounts
                .Property(a => a.DateClosed)
                .IsRequired(false);

            // Configure Relationship
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
