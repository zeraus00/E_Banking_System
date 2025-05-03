namespace Data.Configurations.Authentication
{
    public class AccessRoleConfiguration : IEntityTypeConfiguration<AccessRole>
    {
        public void Configure(EntityTypeBuilder<AccessRole> AccessRoles)
        {
            AccessRoles.ToTable("AccessRoles", "AuthSchema");

            AccessRoles
                .HasKey(ar => ar.AccessRoleId);

            AccessRoles
                .Property(ar => ar.AccessRoleId)
                .ValueGeneratedOnAdd();

            AccessRoles
                .Property(ar => ar.AccessRoleName)
                .IsRequired();
        }
    }
}
