namespace Data.Configurations.User
{
    public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
    {
        // Configure UserInfo Table
        public void Configure(EntityTypeBuilder<UserInfo> UsersInfo)
        {
            UsersInfo.ToTable("UsersInfo", "User");
            // Define Primary Key
            UsersInfo
                .HasKey(ui => ui.UserInfoId);
            UsersInfo
                .Property(ui => ui.UserInfoId)
                .ValueGeneratedOnAdd();
            // Define Required and MaxLength Constraints for FirstName
            UsersInfo
                .Property(ui => ui.FirstName)
                .HasMaxLength(50)
                .IsRequired();
            // Define MaxLength for MiddleName
            UsersInfo
                .Property(ui => ui.MiddleName)
                .HasMaxLength(50)
                .IsRequired(false);
            // Define Required and MaxLength Constraints for LastName
            UsersInfo
                .Property(ui => ui.LastName)
                .HasMaxLength(50)
                .IsRequired();
            // Define MaxLength for Suffix
            UsersInfo
                .Property(ui => ui.Suffix)
                .HasMaxLength(10)
                .IsRequired(false);
            // Define Required Constraint for Age
            UsersInfo
                .Property(ui => ui.Age)
                .IsRequired();
            // Define Required Constraint for Sex
            UsersInfo
                .Property(ui => ui.Age)
                .IsRequired();
            // Define Required Constraint for ContactNumber
            UsersInfo
                .Property(ui => ui.ContactNumber)
                .IsRequired();
            // Define Required Constraint for TaxIdentificationNumber
            UsersInfo
                .Property(ui => ui.TaxIdentificationNumber)
                .IsRequired();
            // Define Required Constraint for CivilStatus
            UsersInfo
                .Property(ui => ui.CivilStatus)
                .IsRequired();
            // Define Required Constraint for Religion
            UsersInfo
                .Property(ui => ui.Religion)
                .IsRequired();

            // Define Foreign Key to BirthInfo
            UsersInfo
                .HasOne(ui => ui.BirthInfo)
                .WithMany(bi => bi.UsersInfo)
                .HasForeignKey(ui => ui.BirthInfoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Define Foreign Key to Address
            UsersInfo
                .HasOne(ui => ui.Address)
                .WithMany(a => a.UsersInfo)
                .HasForeignKey(ui => ui.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

            // Define Relationship to CustomerAuth
            UsersInfo
                .HasMany(ui => ui.CustomersAuth)
                .WithOne(ca => ca.UserInfo)
                .HasForeignKey(ca => ca.UserInfoId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
