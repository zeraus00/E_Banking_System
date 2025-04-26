namespace Data.Configurations.Finance
{
    public class AccountStatusTypeConfiguration : IEntityTypeConfiguration<AccountStatusType>
    {
        public void Configure(EntityTypeBuilder<AccountStatusType> AccountStatusTypes)
        {
            AccountStatusTypes.ToTable("AccountStatusTypes", "FinanceSchema");

            /*  Configure Table Properties  */

            //  AccountStatusTypeId (Primary Key)
            AccountStatusTypes
                .HasKey(ast => ast.AccountStatusTypeId);
            AccountStatusTypes
                .Property(ast => ast.AccountStatusTypeId)
                .ValueGeneratedOnAdd();

            //  AccountStatusTypeName (Required; MaxLength: 20)
            AccountStatusTypes
                .Property(ast => ast.AccountStatusTypeName)
                .HasMaxLength(20)
                .IsRequired();

            /*
             *  Relationships
             *  Accounts (one-to-many)
             */

        }
    }
}
