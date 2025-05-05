namespace Data.Configurations.Finance
{
    public class LoanTransactionConfiguration : IEntityTypeConfiguration<LoanTransaction>
    {
        //  Configure LoanTransactionTable
        public void Configure (EntityTypeBuilder<LoanTransaction> LoanTransactions) 
        {
            LoanTransactions
                .ToTable("LoanTransactions", "FinanceSchema");

            /*  Configure Table Properties  */

            //  LoanTransactionId (Primary Key)
            LoanTransactions
                .HasKey(lt => lt.LoanTransactionId);
            LoanTransactions
                .Property(lt => lt.LoanTransactionId)
                .ValueGeneratedOnAdd();

            //  AmountPaid (Required; Decimal(18,2))
            LoanTransactions
                .Property(lt => lt.AmountPaid)
                .IsRequired()
                .HasColumnType("DECIMAL (18, 2)");

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

            //  DueDate (Required; DateTime)
            LoanTransactions
                .Property(lt => lt.DueDate)
                .HasColumnType("DATE")
                .IsRequired();

            //  TransactionDate (Required; DateTime)
            LoanTransactions
                .Property(lt => lt.TransactionDate)
                .HasColumnType("DATE")
                .IsRequired()
                .HasDefaultValueSql("CAST(GETDATE() AS DATE)");

            //  TransactionTime (Required; TimeSpan)
            LoanTransactions
                .Property(lt => lt.TransactionTime)
                .HasColumnType("TIME")
                .IsRequired()
                .HasDefaultValueSql("CAST(GETDATE() AS TIME)");

            //  Notes (Optional; Max Length: 100)
            LoanTransactions
                .Property(lt => lt.Notes)
                .IsRequired(false)
                .HasMaxLength(100);

            /*
             * Configure Relationships
             * Accounts (many-to-one)
             * Loans (many-to-one)
             */

            LoanTransactions
                .HasOne(lt => lt.Account)
                .WithMany(a => a.LoanTransactions)
                .HasForeignKey(lt => lt.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            LoanTransactions
                .HasOne(lt => lt.Loan)
                .WithMany(l => l.LoanTransactions)
                .HasForeignKey(lt => lt.LoanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
