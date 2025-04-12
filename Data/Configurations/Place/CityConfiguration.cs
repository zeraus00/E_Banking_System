using Data.Models.Place;

namespace Data.Configurations.Place
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        // Configure Cities Table
        public void Configure(EntityTypeBuilder<City> Cities)
        {
            Cities.ToTable("Cities", "PlaceSchema");
            /*  Configure Table Properties  */

            // CityId (Primary Key)
            Cities
                .HasKey(c => c.CityId);
            Cities
                .Property(c => c.CityId)
                .ValueGeneratedOnAdd();
            // CityName (Required; MaxLength=50)
            Cities
                .Property(c => c.CityName)
                .IsRequired()
                .HasMaxLength(50);
            /*  Configure Relationships 
             *  Provinces (many-to-one)
             *  Barangays (one-to-many)
             *  Addresses (one-to-many)
             *  BirthsInfo (one-to-many)
             */
            Cities
                .HasOne(c => c.Province)
                .WithMany(p => p.Cities)
                .HasForeignKey(c => c.ProvinceId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
