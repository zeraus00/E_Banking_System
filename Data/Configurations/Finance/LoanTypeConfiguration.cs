
namespace Data.Configurations.Finance
{
    public class LoanTypeConfiguration : IEntityTypeConfiguration<LoanType>
    {
        //  Configure LoanTransactionTypesTable
        public void Configure(EntityTypeBuilder<LoanType> LoanTypes)
        {
            LoanTypes.ToTable("LoanTypes", "FinanceSchema");

            /*  Configure Table Properties  */

            //  LoanTypeId (Primary Key)
            LoanTypes
                .HasKey(lt => lt.LoanTypeId);
            LoanTypes
                .Property(lt => lt.LoanTypeId)
                .ValueGeneratedOnAdd();

            //  LoanTypeName (Required; MaxLength=20)
            LoanTypes.Property(lt => lt.LoanTypeName)
                .IsRequired()
                .HasMaxLength(20);

            /*
             *  Relationships
             *  Loans (one-to-many)
             *  LoanTransactions (one-to-many)
             */
        }
    }
}
