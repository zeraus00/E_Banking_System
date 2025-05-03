namespace Data.Configurations.Authentication
{
    public class AccessRoleConfiguration : IEntityTypeConfiguration<AccessRole>
    {
        public void Configure(EntityTypeBuilder<AccessRole> AccessRoles)
        {
            AccessRoles.ToTable("AccessRoles", "AuthSchema");
        }
    }
}
