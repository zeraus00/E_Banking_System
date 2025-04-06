namespace Data.Configurations.Finance
{
    public class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> Loans) 
        {
            Loans.ToTable("Loans", "Finance");

            Loans.HasKey(al => al.LoanId);

            Loans.Property(al => al.LoanId)
                .ValueGeneratedOnAdd();

            Loans.Property(al => al.AccountId).IsRequired();

            Loans.Property(al => al.LoanAmount).IsRequired()
                .HasColumnType("DECIMAL (18, 2)");

            Loans.Property(al => al.LoanTermMonths).IsRequired();

            Loans.Property(al => al.StartDate).IsRequired();

            Loans.Property(al => al.EndDate).IsRequired();

            Loans.HasOne(al => al.Account)
                .WithMany(a => a.ActiveLoans)
                .HasForeignKey(al => al.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
