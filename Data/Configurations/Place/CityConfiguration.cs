namespace E_BankingSystem.Data.Configurations.Place
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        // Configure Cities Table
        public void Configure(EntityTypeBuilder<City> Cities)
        {
            Cities.ToTable("Cities", "Place");
            // Define Primary Key
            Cities
                .HasKey(c => c.CityId);
            Cities
                .Property(c => c.CityId)
                .ValueGeneratedOnAdd();
            // Define Required Constraint for CityName
            Cities
                .Property(c => c.CityName)
                .IsRequired();
            // Define Foreign Key to Province
            Cities
                .HasOne(c => c.Province)
                .WithMany(p => p.Cities)
                .HasForeignKey(c => c.ProvinceId);
        }
    }
}
