namespace Data.Configurations.Authentication
{
    public class CustomerAuthConfiguration : IEntityTypeConfiguration<CustomerAuth>
    {
        // Configure CustomersAuth Table
        public void Configure(EntityTypeBuilder<CustomerAuth> CustomersAuth)
        {
            CustomersAuth.ToTable("CustomersAuth", "AuthSchema");

            /*  Configure Table Properties  */

            // CustomersAuthId (Primary Key)
            CustomersAuth
                .HasKey(ca => ca.CustomerAuthId);
            CustomersAuth
                .Property(ca => ca.CustomerAuthId)
                .ValueGeneratedOnAdd();

            // RoleId (Required; Foreign Key to Roles Table)
            CustomersAuth
                .Property(ca => ca.RoleId)
                .IsRequired()
                .HasDefaultValue(1);

            // UserName (Required; MaxLength=20 ; Unique)
            CustomersAuth
                .Property(ca => ca.UserName)
                .IsRequired()
                .HasMaxLength(20);
            CustomersAuth
                .HasIndex(ca => ca.UserName)
                .IsUnique();

            // Email (Required; Maxlength=254; Unique
            CustomersAuth
                .Property(ca => ca.Email)
                .IsRequired()
                .HasMaxLength(254);
            CustomersAuth
                .HasIndex(ca => ca.Email)
                .IsUnique();

            // Password (Required; MaxLength=60; FixedLength)
            CustomersAuth
                .Property(ca => ca.Password)
                .IsRequired()
                .HasMaxLength(60)
                .IsFixedLength();

            /*  
             *  Configure Relationships
             *  Accounts (many-to-one)
             *  UsersInfo (many-to-one)
             */
            CustomersAuth
                .HasOne(ca => ca.Account)
                .WithMany(a => a.CustomersAuth)
                .HasForeignKey(ca => ca.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            CustomersAuth
                .HasOne(ca => ca.UserInfo)
                .WithMany(u => u.CustomersAuth)
                .HasForeignKey(ca => ca.UserInfoId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
