namespace E_BankingSystem.Data.Configurations.User
{
    public class BirthInfoConfiguration : IEntityTypeConfiguration<BirthInfo>
    {
        //  Configure BirthsInfo Table
        public void Configure(EntityTypeBuilder<BirthInfo> BirthInfos)
        {
            BirthInfos.ToTable("BirthsInfo", "User");
            // Define Primary Key
            BirthInfos
                .HasKey(b => b.BirthInfoId);
            BirthInfos
                .Property(b => b.BirthInfoId)
                .ValueGeneratedOnAdd();

            // Define Required Constraint for BirthDate
            BirthInfos
                .Property(b => b.BirthDate)
                .IsRequired();

            // Define Foreign Key to Cities
            BirthInfos
                .HasOne(b => b.City)
                .WithMany(c => c.BirthInfos)
                .HasForeignKey(b => b.CityId)
                .OnDelete(DeleteBehavior.SetNull);

            // Define Foreign Key to Provinces
            BirthInfos
                .HasOne(b => b.Province)
                .WithMany(p => p.BirthsInfo)
                .HasForeignKey(b => b.ProvinceId)
                .OnDelete(DeleteBehavior.SetNull);

            // Define Foreign Key to Regions
            BirthInfos
                .HasOne(b => b.Region)
                .WithMany(r => r.BirthsInfo)
                .HasForeignKey(b => b.RegionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
