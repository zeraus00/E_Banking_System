namespace Data.Configurations.Finance
{
    public class AccountProductTypeConfiguration : IEntityTypeConfiguration<AccountProductType>
    {
        public void Configure(EntityTypeBuilder<AccountProductType> AccountProductTypes)
        {
            AccountProductTypes.ToTable("AccountProductTypes", "FinanceSchema");

            /*  Configure Table Properties  */

            //  AccountProductTypeId (Primary Key)
            AccountProductTypes
                .HasKey(apt => apt.AccountProductTypeId);
            AccountProductTypes
                .Property(apt => apt.AccountProductTypeId)
                .ValueGeneratedOnAdd();

            //  AccountProductTypes (Required; MaxLength=20;)
            AccountProductTypes
                .Property(apt => apt.AccountProductTypeName)
                .HasMaxLength(20)
                .IsRequired();

            /*
             *  Relationships
             *  Accounts (one-to-many)
             */
        }

    }
}
