using Data.Models.Place;

namespace Data.Configurations.Place
{
    public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
    {
        // Configure Provinces Table
        public void Configure(EntityTypeBuilder<Province> Provinces)
        {
            Provinces.ToTable("Provinces", "PlaceSchema");
            /*  Configure Table Properties  */

            //  ProvinceId (Primary Key)
            Provinces
                .HasKey(p => p.ProvinceId);
            Provinces
                .Property(p => p.ProvinceId)
                .ValueGeneratedOnAdd();

            //  ProvinceCode (Required)
            Provinces
                .Property(p => p.ProvinceCode)
                .IsRequired();

            //  ProvinceName (Required; Max Length: 50)
            Provinces
                .Property(p => p.ProvinceName)
                .IsRequired()
                .HasMaxLength(50);

            /*  Configure Relationships 
             *  Regions (many-to-one)
             *  Cities (one-to-many)
             *  Addresses (one-to-many)
             *  BirthsInfo (one-to-many)
             */
            Provinces
                .HasOne(p => p.Region)
                .WithMany(r => r.Provinces)
                .HasForeignKey(p => p.RegionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
