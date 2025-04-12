

namespace Data.Configurations.User
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        // Addresses Table Configuration
        public void Configure(EntityTypeBuilder<Address> Addresses)
        {
            Addresses.ToTable("Addresses", "UserSchema");

            /*  Configure Table Properties  */

            // AddressId (Primary Key)
            Addresses
                .HasKey(a => a.AddressId);
            Addresses
                .Property(a => a.AddressId)
                .ValueGeneratedOnAdd();
            // House (MaxLength=10)
            Addresses
                .Property(a => a.House)
                .HasMaxLength(10);
            // Street (MaxLength=50)
            Addresses
                .Property(a => a.Street)
                .HasMaxLength(50)
                .IsRequired();
            // PostalCode (Required)
            Addresses
                .Property(a => a.PostalCode)
                .IsRequired();

            /*  Configure Relationships  
             *  Barangays (many-to-one)
             *  Cities (many-to-one)
             *  Provinces (many-to-one)
             *  Regions (many-to-one)
             *  UsersInfo (one-to-many)
             */
            Addresses
                .HasOne(a => a.Barangay)
                .WithMany(b => b.Addresses)
                .HasForeignKey(a => a.BarangayId)
                .OnDelete(DeleteBehavior.SetNull);
            Addresses
                .HasOne(a => a.City)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => a.CityId)
                .OnDelete(DeleteBehavior.SetNull);
            Addresses
                .HasOne(a => a.Province)
                .WithMany(p => p.Addresses)
                .HasForeignKey(a => a.ProvinceId)
                .OnDelete(DeleteBehavior.SetNull);
            Addresses
                .HasOne(a => a.Region)
                .WithMany(r => r.Addresses)
                .HasForeignKey(a => a.RegionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
