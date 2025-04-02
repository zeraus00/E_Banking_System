namespace E_BankingSystem.Data.Configurations.User
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        // Addresses Table Configuration
        public void Configure(EntityTypeBuilder<Address> Addresses)
        {
            Addresses.ToTable("Addresses", "User");
            // Define Primary Key
            Addresses
                .HasKey(a => a.AddressId);
            Addresses
                .Property(a => a.AddressId)
                .ValueGeneratedOnAdd();
            // Define MaxLength for House
            Addresses
                .Property(a => a.House)
                .HasMaxLength(10);
            // Define MaxLength for Street
            Addresses
                .Property(a => a.Street)
                .HasMaxLength(50)
                .IsRequired();

            // Define Required Constraint for PostalCode
            Addresses
                .Property(a => a.PostalCode)
                .IsRequired();

            // Define Foreign Key to Barangay
            Addresses
                .HasOne(a => a.Barangay)
                .WithMany(b => b.Addresses)
                .HasForeignKey(a => a.BarangayId)
                .OnDelete(DeleteBehavior.SetNull);

            // Define Foreign Key to City
            Addresses
                .HasOne(a => a.City)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CityId)
                .OnDelete(DeleteBehavior.SetNull);

            // Define Foreign Key to Province
            Addresses
                .HasOne(a => a.Province)
                .WithMany(p => p.Addresses)
                .HasForeignKey(a => a.ProvinceId)
                .OnDelete(DeleteBehavior.SetNull);

            // Define Foreign Key to Region
            Addresses
                .HasOne(a => a.Region)
                .WithMany(r => r.Addresses)
                .HasForeignKey(a => a.RegionId)
                .OnDelete(DeleteBehavior.SetNull);

            // Define Relationship to UserInfo
            Addresses
                .HasMany(a => a.UsersInfo)
                .WithOne(ui => ui.Address)
                .HasForeignKey(a => a.AddressId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
