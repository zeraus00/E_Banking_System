namespace E_BankingSystem.Data.Configurations.Place
{
    public class RegionConfiguration : IEntityTypeConfiguration<Region>
    {
        // Configure Regions Table
        public void Configure(EntityTypeBuilder<Region> Regions)
        {
            Regions.ToTable("Regions");
            // Define Primary Key
            Regions
                .HasKey(Region => Region.RegionId);
            // Define Required Constraint for RegionName
            Regions
                .Property(r => r.RegionName)
                .IsRequired();

            // Define Relationship to Provinces
            Regions
                .HasMany(r => r.Provinces)
                .WithOne(p => p.Region)
                .HasForeignKey(p => p.RegionId);
        }

    }
}
