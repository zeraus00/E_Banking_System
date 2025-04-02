namespace E_BankingSystem.Data.Configurations.Finance
{
    public class TransactionTypeConfig : IEntityTypeConfiguration<TransactionType>
    {
        // Configure TransactionTypes Table
        public void Configure(EntityTypeBuilder<TransactionType> TransactionTypes)
        {
            TransactionTypes.ToTable("TransactionTypes", "Finance");
            // Define Primary Key
            TransactionTypes
                .HasKey(tt => tt.TransactionTypeId);
            TransactionTypes
                .Property(tt => tt.TransactionTypeId)
                .ValueGeneratedOnAdd();
            // Define Required Constraint for TransactionTypeName
            TransactionTypes
                .Property(tt => tt.TransactionTypeName)
                .IsRequired()
                .HasMaxLength(50);

            // Define one-to-many relationship with Transactions
            TransactionTypes
                .HasMany(tt => tt.Transactions)
                .WithOne(t => t.TransactionType)
                .HasForeignKey(t => t.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
