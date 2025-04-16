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

            //  UserNameId (Required)
            UsersInfo
                .Property(ui => ui.UserNameId)
                .IsRequired();

            //  Age (Required)
            UsersInfo
                .Property(ui => ui.Age)
                .IsRequired();

            //  Sex (Required)
            UsersInfo
                .Property(ui => ui.Sex)
                .IsRequired()
                .HasMaxLength(10);

            // FatherNameId (Required)
            UsersInfo
                .Property(ui => ui.FatherNameId)
                .IsRequired();

            //  MotherNameId (Required)
            UsersInfo
                .Property(ui => ui.MotherName)
                .IsRequired();

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
             *  Names: UserName (one to one)
             *  Names: FatherName (many to one)
             *  Names: MotherName (many to one)
             *  BirthsInfo (many-to-one)
             *  Addresses (many-to-one)
             *  CustomersAuth (one-to-many)
             */

            UsersInfo
                .HasOne(ui => ui.UserName)
                .WithOne(n => n.UserInUsersInfo)
                .HasForeignKey<UserInfo>(ui => ui.UserNameId)
                .OnDelete(DeleteBehavior.Restrict);
            UsersInfo
                .HasOne(ui => ui.FatherName)
                .WithMany(n => n.FatherInUsersInfo)
                .HasForeignKey(ui => ui.FatherNameId)
                .OnDelete(DeleteBehavior.Restrict);
            UsersInfo
                .HasOne(ui => ui.MotherName)
                .WithMany(n => n.MotherInUsersInfo)
                .HasForeignKey(ui => ui.MotherNameId)
                .OnDelete(DeleteBehavior.Restrict);
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
