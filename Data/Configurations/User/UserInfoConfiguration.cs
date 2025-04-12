namespace Data.Configurations.User
{
    public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
    {
        // Configure UserInfo Table
        public void Configure(EntityTypeBuilder<UserInfo> UsersInfo)
        {
            UsersInfo.ToTable("UsersInfo", "UserSchema");

            /*  Configure Table Properties  */

            //  UserInfoId (Primary Key)
            UsersInfo
                .HasKey(ui => ui.UserInfoId);
            UsersInfo
                .Property(ui => ui.UserInfoId)
                .ValueGeneratedOnAdd();

            //  FirstName (Required; MaxLength=50)
            UsersInfo
                .Property(ui => ui.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            //  MiddleName (Optional; MaxLength=50)
            UsersInfo
                .Property(ui => ui.MiddleName)
                .HasMaxLength(50)
                .IsRequired(false);

            //  LastName (Required; MaxLength=50)
            UsersInfo
                .Property(ui => ui.LastName)
                .HasMaxLength(50)
                .IsRequired();

            //  Suffix (Optional; MaxLength=10)
            UsersInfo
                .Property(ui => ui.Suffix)
                .HasMaxLength(10)
                .IsRequired(false);

            //  Age (Required)
            UsersInfo
                .Property(ui => ui.Age)
                .IsRequired();

            //  Sex (Required)
            UsersInfo
                .Property(ui => ui.Sex)
                .IsRequired()
                .HasMaxLength(10);

            //  ContactNumber (Required; Field Length=11)
            UsersInfo
                .Property(ui => ui.ContactNumber)
                .IsRequired()
                .HasMaxLength(11)
                .IsFixedLength();

            //  TaxIdentificationNumber (Required; MaxLength=12)
            UsersInfo
                .Property(ui => ui.TaxIdentificationNumber)
                .IsRequired()
                .HasMaxLength(12);

            //  CivilStatus (Required; MaxLength=20)
            UsersInfo
                .Property(ui => ui.CivilStatus)
                .IsRequired()
                .HasMaxLength(20);

            //  Religion (Required; MaxLength=50)
            UsersInfo
                .Property(ui => ui.Religion)
                .IsRequired()
                .HasMaxLength(50);



            /*  
             *  Configure Relationships  
             *  BirthInfo (many-to-one)
             *  Address (many-to-one)
             *  CustomersAuth (one-to-many)
             */
            UsersInfo
                .HasOne(ui => ui.BirthInfo)
                .WithMany(bi => bi.UsersInfo)
                .HasForeignKey(ui => ui.BirthInfoId)
                .OnDelete(DeleteBehavior.SetNull);
            UsersInfo
                .HasOne(ui => ui.Address)
                .WithMany(a => a.UsersInfo)
                .HasForeignKey(ui => ui.AddressId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
