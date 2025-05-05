namespace Data.Configurations.User
{
    public class BirthInfoConfiguration : IEntityTypeConfiguration<BirthInfo>
    {
        //  Configure BirthsInfo Table
        public void Configure(EntityTypeBuilder<BirthInfo> BirthInfos)
        {
            BirthInfos.ToTable("BirthsInfo", "UserSchema");

            /*  Configure Table Properties  */

            //  BirthInfoId (Primary Key)
            BirthInfos
                .HasKey(b => b.BirthInfoId);
            BirthInfos
                .Property(b => b.BirthInfoId)
                .ValueGeneratedOnAdd();

            //  BirthDate (Required)
            BirthInfos
                .Property(b => b.BirthDate)
                .HasColumnType("DATE")
                .IsRequired();


            /*  
             *  Configure Relationships
             *  Cities (many-to-one)
             *  Provinces (many-to-one)
             *  Regions (many-to-one)
             *  UsersInfo (one-to-many)
             */
            BirthInfos
                .HasOne(b => b.City)
                .WithMany(c => c.BirthInfos)
                .HasForeignKey(b => b.CityId)
                .OnDelete(DeleteBehavior.SetNull);
            BirthInfos
                .HasOne(b => b.Province)
                .WithMany(p => p.BirthsInfo)
                .HasForeignKey(b => b.ProvinceId)
                .OnDelete(DeleteBehavior.SetNull);
            BirthInfos
                .HasOne(b => b.Region)
                .WithMany(r => r.BirthsInfo)
                .HasForeignKey(b => b.RegionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
