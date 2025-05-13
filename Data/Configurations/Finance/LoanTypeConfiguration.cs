
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
            LoanTypes
                .Property(lt => lt.LoanTypeName)
                .IsRequired()
                .HasMaxLength(20);

            //  MinimumLoanAmount (Required; Decimal(9, 2))
            LoanTypes
                .Property(lt => lt.MinimumLoanAmount)
                .HasColumnType("Decimal(18, 2)")
                .IsRequired();

            //  InterestRatePerAnnum (Required; Decimal(5, 2))
            LoanTypes
                .Property(lt => lt.InterestRate)
                .HasColumnType("DECIMAL (5, 2)")
                .IsRequired();

            //  LoanTermInMonths (Required)
            LoanTypes
                .Property(lt => lt.LoanTermInMonths)
                .IsRequired();

            /*
             *  Relationships
             *  Loans (one-to-many)
             *  LoanTransactions (one-to-many)
             */
        }
    }
}
