namespace E_BankingSystem.Data.Configurations.Authentication
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        // Configure Roles Table
        public void Configure(EntityTypeBuilder<Role> Roles)
        {
            Roles.ToTable("Roles", "Authentication");
            // Define primary key
            Roles
                .HasKey(r => r.RoleId);
            Roles
                .Property(r => r.RoleId)
                .ValueGeneratedOnAdd();
            // Define required constraint for RoleName
            Roles
                .Property(r => r.RoleName)
                .IsRequired()
                .HasMaxLength(50);
        }

    }
}
