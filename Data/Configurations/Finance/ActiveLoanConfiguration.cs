namespace Data.Configurations.Finance
{
    public class ActiveLoanConfiguration : IEntityTypeConfiguration<ActiveLoan>
    {
        public void Configure(EntityTypeBuilder<ActiveLoan> ActiveLoan) 
        {
            ActiveLoan.ToTable("ActiveLoan", "Finance");

            ActiveLoan.HasKey(al => al.LoanId);

            ActiveLoan.Property(al => al.LoanId)
                .ValueGeneratedOnAdd();

            ActiveLoan.Property(al => al.AccountId).IsRequired();

            ActiveLoan.Property(al => al.LoanAmount).IsRequired()
                .HasColumnType("DECIMAL (18, 2)");

            ActiveLoan.Property(al => al.LoanInMonths).IsRequired();

            ActiveLoan.Property(al => al.StartDate).IsRequired();

            ActiveLoan.Property(al => al.EndDate).IsRequired();

            ActiveLoan.HasOne(al => al.Account)
                .WithMany(a => a.ActiveLoans)
                .HasForeignKey(al => al.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
