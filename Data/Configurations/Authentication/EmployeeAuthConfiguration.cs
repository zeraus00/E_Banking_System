namespace Data.Configurations.Authentication
{
    public class EmployeeAuthConfiguration : IEntityTypeConfiguration<EmployeeAuth>
    {
        // Configure EmployeesAuth Table
        public void Configure(EntityTypeBuilder<EmployeeAuth> EmployeesAuth)
        {
            EmployeesAuth.ToTable("EmployeesAuth", "Authentication");

            /*  Configure Table Properties  */

            //  EmployeeAuthId (Primary Key)
            EmployeesAuth
                .HasKey(ea => ea.EmployeeAuthId);
            EmployeesAuth
                .Property(ea => ea.EmployeeAuthId)
                .ValueGeneratedOnAdd();

            //  UserName (Required; MaxLength=20; Unique)
            EmployeesAuth
                .Property(ea => ea.UserName)
                .IsRequired()
                .HasMaxLength(20);
            EmployeesAuth
                .HasIndex(ea => ea.UserName)
                .IsUnique();

            //  Email (Required; MaxLength=254; Unique)
            EmployeesAuth
                .Property(ea => ea.Email)
                .IsRequired()
                .HasMaxLength(254);
            EmployeesAuth
                .HasIndex(ea => ea.Email)
                .IsUnique();

            //  Password (Required; MaxLength = 60; FixedLength)
            EmployeesAuth
                .Property(ea => ea.Password)
                .IsRequired()
                .HasMaxLength(60)
                .IsFixedLength();
        }
    }
}
