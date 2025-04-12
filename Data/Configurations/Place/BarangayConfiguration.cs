using Data.Models.Place;

namespace Data.Configurations.Place
{
    public class BarangayConfiguration : IEntityTypeConfiguration<Barangay>
    {
        // Configure Barangays Table
        public void Configure(EntityTypeBuilder<Barangay> Barangays)
        {
            Barangays.ToTable("Barangays", "PlaceSchema");
            /*  Configure Table Properties  */

            // BarangayId (Primary Key)
            Barangays
                .HasKey(b => b.BarangayId);
            Barangays
                .Property(b => b.BarangayId)
                .ValueGeneratedOnAdd();
            // BarangayName (Required, MaxLength=50) 
            Barangays
                .Property(b => b.BarangayName)
                .IsRequired()
                .HasMaxLength(50);
            /*
             *  Configure Relationships
             *  Cities (many-to-one)
             *  Addresses (one-to-many)
             */
            Barangays
                .HasMany(b => b.Addresses)
                .WithOne(a => a.Barangay)
                .HasForeignKey(a => a.BarangayId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
