namespace Data.Configurations.User
{
    public class NameConfiguration : IEntityTypeConfiguration<Name>
    {
        //Configure Names Table
        public void Configure(EntityTypeBuilder<Name> Names)
        {
            Names.ToTable("Names", "UserSchema");

            /* Table Properties */

            // NameId (Primary Key)
            Names
                .HasKey(n => n.NameId);
            Names
                .Property(n => n.NameId)
                .ValueGeneratedOnAdd();

            // FirstName (Required; MaxLength=50)
            Names
                .Property(n => n.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            // MiddleName (Optional; MaxLength = 50)
            Names
                .Property(n => n.MiddleName)
                .HasMaxLength(50)
                .IsRequired(false);

            // LastName (Required; MaxLength = 50)
            Names
                .Property(n => n.LastName)
                .HasMaxLength(50)
                .IsRequired();

            // Suffix (Required; MaxLength = 10)
            Names
                .Property(n => n.Suffix)
                .HasMaxLength(10)
                .IsRequired(false);

            /*
             *  Relationships
             *  UsersInfo(one-to-many)
             */
        }


    }
}
