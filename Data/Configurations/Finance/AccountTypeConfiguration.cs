namespace Data.Configurations.Finance
{
    public class AccountTypeConfiguration : IEntityTypeConfiguration<AccountType>
    {
        //  Configure Accounts Table
        public void Configure(EntityTypeBuilder<AccountType> AccountTypes)
        {
            AccountTypes.ToTable("AccountTypes", "FinanceSchema");

            /*  Configure Table Properties  */

            //  AccountTypeId (PrimaryKey)
            AccountTypes
                .HasKey(at => at.AccountTypeId);
            AccountTypes
                .Property(at => at.AccountTypeId)
                .ValueGeneratedOnAdd();

            //  AccountTypeName (Required; MaxLength=20
            AccountTypes
                .Property(at => at.AccountTypeName)
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}
