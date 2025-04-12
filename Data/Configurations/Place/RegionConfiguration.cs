using Data.Models.Place;

namespace Data.Configurations.Place
{
    public class RegionConfiguration : IEntityTypeConfiguration<Region>
    {
        // Configure Regions Table
        public void Configure(EntityTypeBuilder<Region> Regions)
        {
            Regions.ToTable("Regions", "PlaceSchema");
            /*  Configure Table Properties  */

            // RegionId (Primary Key)
            Regions
                .HasKey(Region => Region.RegionId);
            Regions
                .Property(Region => Region.RegionId)
                .ValueGeneratedOnAdd();
            // RegionName (Required; MaxLength=50)
            Regions
                .Property(r => r.RegionName)
                .IsRequired()
                .HasMaxLength(50);

            /*  Configure Relationships
             *  Provinces (one-to-many)
             *  Addresses (one-to-many)
             *  BirthsInfo (one-to-many)
             */
            Regions
                .HasMany(r => r.Provinces)
                .WithOne(p => p.Region)
                .HasForeignKey(p => p.RegionId);
            Regions
                .HasMany(r => r.Addresses)
                .WithOne(a => a.Region)
                .HasForeignKey(a => a.RegionId);
            Regions
                .HasMany(r => r.BirthsInfo)
                .WithOne(b => b.Region)
                .HasForeignKey(b => b.RegionId);
        }

    }
}
