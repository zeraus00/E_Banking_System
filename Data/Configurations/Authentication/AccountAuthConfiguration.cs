namespace E_BankingSystem.Data.Configurations.Authentication
{
    public class AccountAuthConfiguration : IEntityTypeConfiguration<AccountAuth>
    {
        // Configure AccountAuth Table
        public void Configure(EntityTypeBuilder<AccountAuth> AccountsAuth)
        {
            AccountsAuth.ToTable("AccountsAuth");

            // Define primary key
            AccountsAuth
                .HasKey(au => au.AccountAuthId);
            AccountsAuth
                .Property(au => au.AccountAuthId)
                .ValueGeneratedOnAdd();

            // Define foreign key to Accounts (many-to-one)
            AccountsAuth
                .HasOne(au => au.Account)
                .WithMany(a => a.AccountsAuth)
                .HasForeignKey(au => au.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define foreign key to UsersInfo (many-to-one)
            AccountsAuth
                .HasOne(au => au.User)
                .WithMany(u => u.AccountsAuth)
                .HasForeignKey(au => au.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Define required and unique constraint for UserName
            AccountsAuth
                .Property(au => au.UserName)
                .IsRequired();
            AccountsAuth
                .HasIndex(au => au.UserName)
                .IsUnique();

            // Define required and unique constraint for Email
            AccountsAuth
                .Property(au => au.Email)
                .IsRequired();
            AccountsAuth
                .HasIndex(au => au.Email)
                .IsUnique();
        }
    }
}
