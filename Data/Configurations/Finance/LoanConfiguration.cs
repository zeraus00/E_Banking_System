using Microsoft.Extensions.Options;

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

            //  LoanNumber (Required; Varchar(20))
            Loans
                .Property(l => l.LoanNumber)
                .HasColumnType("VARCHAR (20)")
                .IsFixedLength();

            //  AccountId (Foreign Key; Required)
            Loans
                .Property(l => l.AccountId)
                .IsRequired();

            //  UserInfoId (Foreign Key; Required)
            Loans
                .Property(l => l.UserInfoId)
                .IsRequired();

            //  ContactNo (Required; VARCHAR(11); FixedLnegth)
            Loans
                .Property(l => l.ContactNo)
                .HasColumnType("VARCHAR(11)")
                .IsFixedLength()
                .IsRequired();

            //  Email (Required; VARCHAR(50); MinLength=16)
            Loans
                .Property(l => l.Email)
                .HasColumnType("VARCHAR(50)")
                .IsRequired();


            //  LoanTypeId (Foreign Key; Required)
            Loans
                .Property(l => l.LoanTypeId)
                .IsRequired();

            //  LoanPurpose (Required; Varchar(30)
            Loans
                .Property(l => l.LoanPurpose)
                .HasColumnType("VARCHAR (30)")
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

            //  LoanTermFrequency (Required)
            Loans
                .Property(l => l.PaymentFrequency)
                .IsRequired();

            //  PaymentAmount (Required; Decimal(18,2))
            Loans
                .Property(l => l.PaymentAmount)
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
                .HasColumnType("DATE")
                .IsRequired()
                .HasDefaultValueSql("CAST(GETDATE() AS DATE)");

            //  LoanStatus (Required; Default: 'Pending')
            Loans
                .Property(l => l.LoanStatus)
                .IsRequired()
                .HasDefaultValue("Pending");

            //  StartDate (Optional; DateTime)
            Loans
                .Property(l => l.StartDate)
                .IsRequired(false);

            //  DueDate (Optional; DateTime)
            Loans
                .Property(l => l.DueDate)
                .IsRequired(false);

            //  UpdateDate (Optional; DateTime)
            Loans
            .Property(l => l.UpdateDate)
                .IsRequired(false);

            //  EndDate (Optional; DateTime)
            Loans
                .Property(l => l.EndDate)
                .IsRequired(false);

            //  Remarks (Optional; MaxLength: 50
            Loans
                .Property(l => l.Remarks)
                .HasMaxLength(50)
                .IsRequired(false);

            /*  Configure Relationships  
             *  Accounts (many-to-one)
             *  UserInfo (many-to-one)
             *  LoanTypes (many-to-one)
             *  LoanTransactions (one-to-many)
             */

            Loans
                .HasOne(l => l.Account)
                .WithMany(a => a.Loans)
                .HasForeignKey(l => l.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            Loans
                .HasOne(l => l.UserInfo)
                .WithMany(ui => ui.Loans)
                .HasForeignKey(l => l.UserInfoId)
                .OnDelete(DeleteBehavior.Restrict);
            Loans
                .HasOne(l => l.LoanType)
                .WithMany(lt => lt.Loans)
                .HasForeignKey(l => l.LoanTypeId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
