namespace E_BankingSystem.Data.Configurations.Finance
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        // Configure Transactions Table
        public void Configure(EntityTypeBuilder<Transaction> Transactions)
        {
            Transactions.ToTable("Transactions", "Finance");
            // Define Primary Key
            Transactions
                .HasKey(t => t.TransactionId);
            Transactions
                .Property(t => t.TransactionId)
                .ValueGeneratedOnAdd();
            // Define Foreign Key to Accounts Table
            Transactions
                .Property(t => t.AccountId)
                .IsRequired();
            Transactions
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            // Define Foreign Key to TransactionTypes Table
            Transactions
                .Property(t => t.TransactionTypeId)
                .IsRequired();
            Transactions
                .HasOne(t => t.TransactionType)
                .WithMany(tt => tt.Transactions)
                .HasForeignKey(t => t.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            // Define Required Constraint for Amount
            Transactions
                .Property(t => t.Amount)
                .IsRequired();
            // Define Required Constraint for PreviousBalance
            Transactions
                .Property(t => t.PreviousBalance)
                .IsRequired();
            // Define Required Constraint for NewBalance
            Transactions
                .Property(t => t.NewBalance)
                .IsRequired();
            // Define Required Constraint for TransactionDate
            Transactions
                .Property(t => t.TransactionDate)
                .IsRequired()
                .HasDefaultValueSql("CURDATE()");
            // Define Required Constraint for TransactionTime
            Transactions
                .Property(t => t.TransactionTime)
                .IsRequired()
                .HasDefaultValueSql("CURTIME");
            // Define Required Constraint for TransactionFee
            Transactions
                .Property(t => t.TransactionFee)
                .IsRequired()
                .HasDefaultValue(0.0m);
        }
    }
}
