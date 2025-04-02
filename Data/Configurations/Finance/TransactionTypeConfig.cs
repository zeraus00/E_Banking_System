namespace E_BankingSystem.Data.Configurations.Finance
{
    public class TransactionTypeConfig : IEntityTypeConfiguration<TransactionType>
    {
        // Configure TransactionTypes Table
        public void Configure(EntityTypeBuilder<TransactionType> TransactionTypes)
        {
            TransactionTypes.ToTable("TransactionTypes", "Finance");

            /*  Configure Table Properties  */

            //  TransactionTypeId (Primary Key)
            TransactionTypes
                .HasKey(tt => tt.TransactionTypeId);
            TransactionTypes
                .Property(tt => tt.TransactionTypeId)
                .ValueGeneratedOnAdd();
            //  TransactionTypeName (Required; MaxLength=50)
            TransactionTypes
                .Property(tt => tt.TransactionTypeName)
                .IsRequired()
                .HasMaxLength(50);

            /*
             *  Configure Relationships
             *  Transactions (one-to-many)
             */
            TransactionTypes
                .HasMany(tt => tt.Transactions)
                .WithOne(t => t.TransactionType)
                .HasForeignKey(t => t.TransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
