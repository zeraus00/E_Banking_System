namespace Data.Configurations.Finance
{
    public class TransactionTypeConfig : IEntityTypeConfiguration<TransactionType>
    {
        // Configure TransactionTypes Table
        public void Configure(EntityTypeBuilder<TransactionType> TransactionTypes)
        {
            TransactionTypes.ToTable("TransactionTypes", "FinanceSchema");

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
        }
    }
}
