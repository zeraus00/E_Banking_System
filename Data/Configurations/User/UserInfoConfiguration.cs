using Data.Constants;

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

            //  ProfilePicture (Required but nullable for now;
            //  MaxSize:10MB but set to max due to SQL syntax limitations)
            UsersInfo
                .Property(a => a.ProfilePicture)
                .HasColumnType("VARBINARY(MAX)");

            //  Age (Required)
            UsersInfo
                .Property(ui => ui.Age)
                .IsRequired();

            //  Sex (Required)
            UsersInfo
                .Property(ui => ui.Sex)
                .IsRequired()
                .HasMaxLength(20);

            // FatherNameId (Required)
            UsersInfo
                .Property(ui => ui.FatherNameId)
                .IsRequired();

            //  MotherNameId (Required)
            UsersInfo
                .Property(ui => ui.MotherNameId)
                .IsRequired();

            //  ContactNumber (Required; Field Length=11)
            UsersInfo
                .Property(ui => ui.ContactNumber)
                .IsRequired()
                .HasMaxLength(11)
                .IsFixedLength();

            //  Occupation (Required; Field Length=50)
            UsersInfo
                .Property(ui => ui.Occupation)
                .IsRequired()
                .HasMaxLength(50);

            //  GovernmentId (Required but nullable for now;
            //  MaxSize:10MB but set to MAX due to SQL syntax limitations.)
            UsersInfo
                .Property(ui => ui.GovernmentId)
                .HasColumnType("VARBINARY(MAX)");

            //  PayslipPicture (Optional (to be uploaded on loan application);
            //  MaxSize:10MB but set to MAX due to SQL syntax limitations.)
            UsersInfo
                .Property(ui => ui.PayslipPicture)
                .HasColumnType("VARBINARY(MAX)");

            //  TaxIdentificationNumber (Required; MaxLength=12)
            UsersInfo
                .Property(ui => ui.TaxIdentificationNumber)
                .IsRequired()
                .HasMaxLength(12);

            //  CivilStatus (Required; MaxLength=30)
            UsersInfo
                .Property(ui => ui.CivilStatus)
                .IsRequired()
                .HasMaxLength(30);

            /*  
             *  Configure Relationships
             *  UsersAuth (one-to-one) (principal)
             *  Names: UserName (one to one)
             *  Names: FatherName (many to one)
             *  Names: MotherName (many to one)
             *  BirthsInfo (many-to-one)
             *  Addresses (many-to-one)
             *  Religions (many-to-one)
             *  Accounts (many-to-many)
             */
            UsersInfo
                .HasOne(ui => ui.UserAuth)
                .WithOne(ua => ua.UserInfo)
                .HasForeignKey<UserInfo>(ui => ui.UserAuthId)
                .OnDelete(DeleteBehavior.Restrict);
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
            UsersInfo
                .HasOne(ui => ui.Religion)
                .WithMany(r => r.UsersInfo)
                .HasForeignKey(ui => ui.ReligionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
