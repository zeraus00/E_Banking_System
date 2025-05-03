namespace Data.Configurations.JoinEntity
{
    public class UsersInfoAccountConfiguration : IEntityTypeConfiguration<UserInfoAccount>
    {
        public void Configure(EntityTypeBuilder<UserInfoAccount> usersInfoAccount)
        {
            usersInfoAccount
                .ToTable("UsersInfoAccounts", "JoinEntitySchema");
            usersInfoAccount
                .HasKey(j => new { j.UserInfoId, j.AccountId });

            usersInfoAccount
                .HasOne(j => j.UserInfo)
                .WithMany(ui => ui.UserInfoAccounts)
                .HasForeignKey(j => j.UserInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            usersInfoAccount
                .HasOne(j => j.Account)
                .WithMany(a => a.UsersInfoAccount)
                .HasForeignKey(j => j.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            usersInfoAccount
                .HasOne(j => j.AccessRole)
                .WithMany(ar => ar.UsersInfoAccounts)
                .HasForeignKey(j => j.AccessRoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
