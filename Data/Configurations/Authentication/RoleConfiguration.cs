namespace Data.Configurations.Authentication
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        // Configure Roles Table
        public void Configure(EntityTypeBuilder<Role> Roles)
        {
            Roles.ToTable("Roles", "Authentication");

            /*  Configure Table Properties  */

            //  RoleId (Primary Key)
            Roles
                .HasKey(r => r.RoleId);
            Roles
                .Property(r => r.RoleId)
                .ValueGeneratedOnAdd();

            //  RoleName (Required; MaxLength=50)
            Roles
                .Property(r => r.RoleName)
                .IsRequired()
                .HasMaxLength(50);
        }

    }
}
