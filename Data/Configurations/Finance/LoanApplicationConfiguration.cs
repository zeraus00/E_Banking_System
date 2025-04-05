using Microsoft.EntityFrameworkCore;

namespace Data.Configurations.Finance
{
    public class LoanApplicationConfiguration : IEntityTypeConfiguration<LoanApplication>
    {
        public void Configure(EntityTypeBuilder<LoanApplication> LoanApplication) 
            {
                LoanApplication.ToTable("LoanApplication", "Finance");

                LoanApplication
                    .HasKey(la => la.LoanApplicationId);
                LoanApplication
                    .Property(la => la.LoanApplicationId)
                    .ValueGeneratedOnAdd();

                LoanApplication
                    .Property(la => la.LoanAmount).IsRequired()
                    .HasColumnType("DECIMAL (18,2)");

                LoanApplication
                    .Property(la => la.LoanInMonths).IsRequired();
                
                LoanApplication
                    .Property(la => la.ApplicationDate).IsRequired()
                    .HasDefaultValueSql("CURTIME()");

                LoanApplication
                    .Property(la => la.ApplicationStatus).IsRequired()
                    .HasDefaultValue("Pending");

            LoanApplication.Property(la => la.Email)
                .HasMaxLength(20);

            LoanApplication.Property(la => la.Occupation).IsRequired()
                .HasMaxLength(30);

            LoanApplication.Property(la => la.MinimumGrossIncome).IsRequired()
                .HasColumnType("DECIMAL (18, 2)");

                LoanApplication
                    .HasOne(la => la.Account)
                    .WithMany()
                    .HasForeignKey(la => la.AccountId)
                    .OnDelete(DeleteBehavior.Restrict); 
            }
    }
}
