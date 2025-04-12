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

            //  AccountId (Foreign Key to Accounts Table)
            Transactions
                .Property(t => t.AccountId)
                .IsRequired();

            //  TransactionTypeId (Foreign Key to TransactionTypes Table)
            Transactions
                .Property(t => t.TransactionTypeId)
                .IsRequired();

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
                .HasDefaultValueSql("CURDATE()");
            //  TransactionTime (Required, Default Value="CURTIME()")
            Transactions
                .Property(t => t.TransactionTime)
                .IsRequired()
                .HasDefaultValueSql("CURTIME()");
            //  TransactionFee  (Required, DECIMAL(18,2), Default Value=0,0m)
            Transactions
                .Property(t => t.TransactionFee)
                .IsRequired()
                .HasColumnType("DECIMAL(18,2)")
                .HasDefaultValue(0.0m);

            /*
             *  Configure Relationships
             *  Accounts (many-to-one)
             *  TransactionTypes (many-to-one)
             */
            Transactions
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.SetNull);
            Transactions
                .HasOne(t => t.TransactionType)
                .WithMany(tt => tt.Transactions)
                .HasForeignKey(t => t.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
