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
                .IsRequired();

            //  TransactionDate (Required; DateTime)
            LoanTransactions
                .Property(lt => lt.TransactionDate)
                .IsRequired()
                .HasDefaultValueSql("CURDATE()");

            //  TransactionTime (Required; TimeSpan)
            LoanTransactions
                .Property(lt => lt.TransactionTime)
                .IsRequired()
                .HasDefaultValueSql("CURTIME()");

            /*
             * Configure Relationships
             * Accounts (many-to-one)
             * ActiveLoans (many-to-one)
             * LoanTypes (many-to-one)
             */

            LoanTransactions
                .HasOne(lt => lt.Account)
                .WithMany(a => a.LoanTransactions)
                .HasForeignKey(lt => lt.AccountId)
                .OnDelete(DeleteBehavior.SetNull);

            LoanTransactions
                .HasOne(lt => lt.ActiveLoans)
                .WithMany(lt => lt.Loantransactions)
                .HasForeignKey(lt => lt.LoanId)
                .OnDelete(DeleteBehavior.SetNull);

            LoanTransactions
                .HasOne(lt => lt.LoanType)
                .WithMany()
                .HasForeignKey(lt => lt.LoanTypeId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
