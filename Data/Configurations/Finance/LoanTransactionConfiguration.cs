namespace Data.Configurations.Finance
{
    public class LoanTransactionConfiguration : IEntityTypeConfiguration<LoanTransactions>
    {
        public void Configure (EntityTypeBuilder<LoanTransactions> LoanTransactions) 
        {
            LoanTransactions
                .ToTable("LoanTransactions", "Finance");

            LoanTransactions
                .HasKey(lt => lt.LoanTransactionId);

            LoanTransactions
                .Property(lt => lt.LoanTransactionId).IsRequired();

            LoanTransactions
                .Property(lt => lt.RemainingLoanBalance).IsRequired()
                .HasColumnType("DECIMAL (18, 2)");

            LoanTransactions
                .Property(lt => lt.InterestAmount).IsRequired()
                .HasColumnType("DECIMAL (18, 2)");

            LoanTransactions
                .Property(lt => lt.PrincipalAmount).IsRequired()
                .HasColumnType("DECIMAL (18, 2)");

            LoanTransactions
                .HasOne(lt => lt.ActiveLoans)
                .WithMany(lt => lt.Loantransactions)
                .HasForeignKey(lt => lt.LoanId)
                .OnDelete(DeleteBehavior.Restrict);

            LoanTransactions
                .HasOne(lt => lt.LoanTransactionType)
                .WithMany()
                .HasForeignKey(lt => lt.LoanTransactionTypeId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
