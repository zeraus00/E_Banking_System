namespace Data.Configurations.Finance
{
    public class LoanTransactionConfiguration : IEntityTypeConfiguration<LoanTransaction>
    {
        //  Configure LoanTransactionTable
        public void Configure (EntityTypeBuilder<LoanTransaction> LoanTransactions) 
        {
            LoanTransactions
                .ToTable("LoanTransactions", "Finance");

            /*  Configure Table Properties  */

            //  LoanTransactionId (Primary Key)
            LoanTransactions
                .HasKey(lt => lt.LoanTransactionId);

            LoanTransactions
                .Property(lt => lt.LoanTransactionId).IsRequired();

            //  AmountPaid (Required; Decimal(18,2))

            //  RemainingLoanBalance (Required; Decimal(18,2))
            LoanTransactions
                .Property(lt => lt.RemainingLoanBalance).IsRequired()
                .HasColumnType("DECIMAL (18, 2)");

            //  InterestAmount (Required; Decimal(18,2))
            LoanTransactions
                .Property(lt => lt.InterestAmount).IsRequired()
                .HasColumnType("DECIMAL (18, 2)");

            //  PrincipalAmount (Required; Decimal(18,2))
            LoanTransactions
                .Property(lt => lt.PrincipalAmount).IsRequired()
                .HasColumnType("DECIMAL (18, 2)");



            /*
             * Configure Relationships
             * Accounts (many-to-one)
             * ActiveLoans (many-to-one)
             * LoanTypes (many-to-one)
             */

            LoanTransactions
                .HasOne(lt => lt.ActiveLoans)
                .WithMany(lt => lt.Loantransactions)
                .HasForeignKey(lt => lt.LoanId)
                .OnDelete(DeleteBehavior.Restrict);

            LoanTransactions
                .HasOne(lt => lt.LoanType)
                .WithMany()
                .HasForeignKey(lt => lt.LoanTypeId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
