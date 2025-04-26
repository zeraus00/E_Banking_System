namespace Data.Configurations.Finance
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        // Configure Transactions Table
        public void Configure(EntityTypeBuilder<Transaction> Transactions)
        {
            Transactions.ToTable("Transactions", "FinanceSchema");

            /*  Configure Table Properties  */

            //  TransactionId (Primary Key)
            Transactions
                .HasKey(t => t.TransactionId);
            Transactions
                .Property(t => t.TransactionId)
                .ValueGeneratedOnAdd();

            //  TransactionTypeId (Foreign Key to TransactionTypes Table)
            Transactions
                .Property(t => t.TransactionTypeId)
                .IsRequired();

            //  TransactionNumber (Required, VARCHAR(32), FixedLength)
            Transactions
                .Property(t => t.TransactionNumber)
                .HasMaxLength(32)
                .IsFixedLength()
                .IsRequired();

            //  Status  (Required, MaxLength(20))
            Transactions
                .Property(t => t.Status)
                .HasMaxLength(20)
                .IsRequired();

            //  ConfirmationNumber (Optional, VARCHAR(28), FixedLength)
            Transactions
                .Property(t => t.ConfirmationNumber)
                .HasMaxLength(28)
                .IsFixedLength()
                .IsRequired(false);

            //  CounterAccountId (Optional)
            Transactions
                .Property(t => t.CounterAccountId)
                .IsRequired(false);

            //  Amount (Required, DECIMAL(18,2))
            Transactions
                .Property(t => t.Amount)
                .IsRequired()
                .HasColumnType("DECIMAL(18,2)");
            //  PreviousBalance (Required, DECIMAL(18,2))
            Transactions
                .Property(t => t.PreviousBalance)
                .IsRequired()
                .HasColumnType("DECIMAL(18,2)");
            //  NewBalance (Required, DECIMAL(18,2))
            Transactions
                .Property(t => t.NewBalance)
                .IsRequired()
                .HasColumnType("DECIMAL(18,2)");
            //  TransactionDate (Required; Default Value="CURDATE()")
            Transactions
                .Property(t => t.TransactionDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            //  TransactionTime (Required, Default Value="CURTIME()")
            Transactions
                .Property(t => t.TransactionTime)
                .IsRequired()
                .HasDefaultValueSql("CAST(GETDATE() AS TIME)");
            //  TransactionFee  (Required, DECIMAL(18,2), Default Value=0,0m)
            Transactions
                .Property(t => t.TransactionFee)
                .IsRequired()
                .HasColumnType("DECIMAL(18,2)")
                .HasDefaultValue(0.0m);

            /*
             *  Configure Relationships
             *  Accounts: MainAccount (many-to-one) 
             *  Accounts: CounterAccount (many-to-one)
             *  TransactionTypes (many-to-one)
             */
            Transactions
                .HasOne(t => t.MainAccount)
                .WithMany(a => a.MainTransactions)
                .HasForeignKey(t => t.MainAccountId)
                .OnDelete(DeleteBehavior.Restrict);
            Transactions
                .HasOne(t => t.CounterAccount)
                .WithMany(a => a.CounterTransactions)
                .HasForeignKey(t => t.CounterAccountId)
                .OnDelete(DeleteBehavior.Restrict);
            Transactions
                .HasOne(t => t.TransactionType)
                .WithMany(tt => tt.Transactions)
                .HasForeignKey(t => t.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
