namespace E_BankingSystem.Data.Configurations.Place
{
    public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
    {
        // Configure Provinces Table
        public void Configure(EntityTypeBuilder<Province> Provinces)
        {
            Provinces.ToTable("Provinces");
            // Define Primary Key
            Provinces
                .HasKey(p => p.ProvinceId);
            Provinces
                .Property(p => p.ProvinceId)
                .ValueGeneratedOnAdd();
            // Define Required Constraint for ProvinceName
            Provinces
                .Property(p => p.ProvinceName)
                .IsRequired();
            // Define Foreign Key to Region
            Provinces
                .HasOne(p => p.Region)
                .WithMany(r => r.Provinces)
                .HasForeignKey(p => p.RegionId);
        }
    }
}
