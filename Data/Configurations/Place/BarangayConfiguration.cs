namespace E_BankingSystem.Data.Configurations.Place
{
    public class BarangayConfiguration : IEntityTypeConfiguration<Barangay>
    {
        // Configure Barangays Table
        public void Configure(EntityTypeBuilder<Barangay> Barangays)
        {
            Barangays.ToTable("Barangays", "Place");
            // Define Primary Key
            Barangays
                .HasKey(b => b.BarangayId);
            Barangays
                .Property(b => b.BarangayId)
                .ValueGeneratedOnAdd();
            // Define Required Constraint for BarangayName
            Barangays
                .Property(b => b.BarangayName)
                .IsRequired();
            // Define Foreign Key to City
            Barangays
                .HasOne(b => b.City)
                .WithMany(c => c.Barangays)
                .HasForeignKey(b => b.CityId);
        }
    }
}
