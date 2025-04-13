namespace Data.Configurations.Authentication
{
    public class UserAuthConfiguration : IEntityTypeConfiguration<UserAuth>
    {
        // Configure CustomersAuth Table
        public void Configure(EntityTypeBuilder<UserAuth> UsersAuth)
        {
            UsersAuth.ToTable("UsersAuth", "AuthSchema");

            /*  Configure Table Properties  */

            // UsersAuthId (Primary Key)
            UsersAuth
                .HasKey(ua => ua.UserAuthId);
            UsersAuth
                .Property(ua => ua.UserAuthId)
                .ValueGeneratedOnAdd();

            // RoleId (Required; Foreign Key to Roles Table)
            UsersAuth
                .Property(ua => ua.RoleId)
                .IsRequired()
                .HasDefaultValue(1);

            // UserName (Required; MaxLength=20 ; Unique)
            UsersAuth
                .Property(ua => ua.UserName)
                .IsRequired()
                .HasMaxLength(20);
            UsersAuth
                .HasIndex(ua => ua.UserName)
                .IsUnique();

            // Email (Required; Maxlength=254; Unique
            UsersAuth
                .Property(ua => ua.Email)
                .IsRequired()
                .HasMaxLength(254);
            UsersAuth
                .HasIndex(ua => ua.Email)
                .IsUnique();

            // Password (Required; MaxLength=60; FixedLength)
            UsersAuth
                .Property(ua => ua.Password)
                .IsRequired()
                .HasMaxLength(60)
                .IsFixedLength();

            /*  
             *  Configure Relationships
             *  Roles (many-to-one)
             *  Accounts (many-to-one)
             *  UsersInfo (many-to-one)
             */
            UsersAuth
                .HasOne(ua => ua.Role)
                .WithMany(r => r.UsersAuth)
                .HasForeignKey(ua => ua.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            UsersAuth
                .HasOne(ua => ua.Account)
                .WithMany(acc => acc.UsersAuth)
                .HasForeignKey(ua => ua.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            UsersAuth
                .HasOne(ua => ua.UserInfo)
                .WithMany(ui => ui.UsersAuth)
                .HasForeignKey(ua => ua.UserInfoId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
