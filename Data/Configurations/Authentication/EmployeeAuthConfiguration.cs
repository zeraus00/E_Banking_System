namespace E_BankingSystem.Data.Configurations.Authentication
{
    public class EmployeeAuthConfiguration : IEntityTypeConfiguration<EmployeeAuth>
    {
        // Configure EmployeesAuth Table
        public void Configure(EntityTypeBuilder<EmployeeAuth> EmployeesAuth)
        {
            EmployeesAuth.ToTable("EmployeesAuth", "Authentication");
            // Define primary key
            EmployeesAuth
                .HasKey(ea => ea.EmployeeAuthId);
            EmployeesAuth
                .Property(ea => ea.EmployeeAuthId)
                .ValueGeneratedOnAdd();
            // Define required and unique constraint for Username
            EmployeesAuth
                .Property(ea => ea.Username)
                .IsRequired();
            EmployeesAuth
                .HasIndex(ea => ea.Username)
                .IsUnique();
            // Define required and unique constraint for Email
            EmployeesAuth
                .Property(ea => ea.Email)
                .IsRequired();
            EmployeesAuth
                .HasIndex(ea => ea.Email)
                .IsUnique();
            // Define required constraint for Password
            EmployeesAuth
                .Property(ea => ea.Password)
                .IsRequired();
        }
    }
}
