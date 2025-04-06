
namespace Data.Configurations.Finance
{
    public class LoanTypeConfiguration : IEntityTypeConfiguration<LoanType>
    {
        //  Configure LoanTransactionTypesTable
        public void Configure(EntityTypeBuilder<LoanType> LoanTypes)
        {
            LoanTypes.ToTable("LoanTypes", "Finance");

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
             *  Configure Relationships
             *  LoanTransactions (one-to-many)
             */
            LoanTypes
                .HasMany(lt => lt.LoanTransactions)
                .WithOne(lt => lt.LoanType)
                .HasForeignKey(lt => lt.LoanTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
