namespace E_BankingSystem.Data.Configurations.Place
{
    public class BarangayConfiguration : IEntityTypeConfiguration<Barangay>
    {
        // Configure Barangays Table
        public void Configure(EntityTypeBuilder<Barangay> Barangays)
        {
            Barangays.ToTable("Barangays", "Place");
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
             */
            Barangays
                .HasOne(b => b.City)
                .WithMany(c => c.Barangays)
                .HasForeignKey(b => b.CityId);
        }
    }
}
