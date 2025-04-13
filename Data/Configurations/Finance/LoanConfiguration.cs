namespace Data.Configurations.Finance
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        //  Configure Loans Table
        public void Configure(EntityTypeBuilder<Loan> Loans) 
        {
            Loans.ToTable("Loans", "FinanceSchema");

            /*  Configure Table Properties  */

            //  LoanId (Primary Key)
            Loans
                .HasKey(l => l.LoanId);
            Loans
                .Property(l => l.LoanId)
                .ValueGeneratedOnAdd();

            //  AccountId (Foreign Key; Required)
            Loans
                .Property(l => l.AccountId)
                .IsRequired();

            //  LoanAmount (Required; Decimal(18,2))
            Loans
                .Property(l => l.LoanAmount)
                .IsRequired()
                .HasColumnType("DECIMAL (18, 2)");

            //  InterestRate (Required; Decimal(5,2))
            Loans
                .Property(l => l.InterestRate)
                .IsRequired()
                .HasColumnType("DECIMAL (5, 2)");

            //  LoanTermMonths (Required; Integer)
            Loans
                .Property(l => l.LoanTermMonths)
                .IsRequired();

            //  MonthlyPayment (Required; Decimal(18,2))
            Loans
                .Property(l => l.MonthlyPayment)
                .IsRequired()
                .HasColumnType("DECIMAL (18, 2)");

            //  RemainingLoanBalance (Required; Decimal(18,2))
            Loans
                .Property(l => l.RemainingLoanBalance)
                .IsRequired()
                .HasColumnType("DECIMAL (18, 2)");

            //  ApplicationDate (Required; Default: CURDATE())
            Loans
                .Property(l => l.ApplicationDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            //  LoanStatus (Required; Default: 'Pending')
            Loans
                .Property(l => l.LoanStatus)
                .IsRequired()
                .HasDefaultValue("Pending");

            //  StartDate (Required; DateTime)
            Loans
                .Property(l => l.StartDate)
                .IsRequired();

            //  DueDate (Required; DateTime)
            Loans
                .Property(l => l.DueDate)
                .IsRequired();

            //  UpdateDate (Required; DateTime)
            Loans
                .Property(l => l.UpdateDate)
                .IsRequired();

            //  EndDate (Required; DateTime)
            Loans
                .Property(l => l.EndDate)
                .IsRequired();

            /*  Configure Relationships  
             *  Accounts (many-to-one)
             *  LoanTypes (many-to-one)
             *  LoanTransactions (one-to-many)
             */

            Loans
                .HasOne(l => l.Account)
                .WithMany(a => a.Loans)
                .HasForeignKey(l => l.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            Loans
                .HasOne(l => l.LoanType)
                .WithMany(lt => lt.Loans)
                .HasForeignKey(l => l.LoanTypeId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
