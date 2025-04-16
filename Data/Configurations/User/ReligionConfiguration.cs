namespace Data.Configurations.User
{
    public class ReligionConfiguration : IEntityTypeConfiguration<Religion>
    {
        public void Configure(EntityTypeBuilder<Religion> Religions)
        {
            Religions.ToTable("Religions", "UserSchema");

            /*  Table Properties    */

            //  ReligionId (Primary Key)
            Religions
                .HasKey(r => r.ReligionId);
            Religions
                .Property(r => r.ReligionId)
                .ValueGeneratedOnAdd();

            //  ReligionName (Required; MaxLength=50)
            Religions
                .Property(r => r.ReligionName)
                .HasMaxLength(50)
                .IsRequired();

            /*
             *  Relationships
             *  UserInfo (One-to-many)
             */
        }
    }
}
