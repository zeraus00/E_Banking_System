namespace E_BankingSystem.Data.Configurations.Authentication
{
    public class CustomersAuthConfiguration : IEntityTypeConfiguration<CustomerAuth>
    {
        // Configure CustomersAuth Table
        public void Configure(EntityTypeBuilder<CustomerAuth> CustomersAuth)
        {
            CustomersAuth.ToTable("CustomersAuth");

            // Define primary key
            CustomersAuth
                .HasKey(au => au.CustomerAuthId);
            CustomersAuth
                .Property(au => au.CustomerAuthId)
                .ValueGeneratedOnAdd();

            // Define foreign key to Accounts (many-to-one)
            CustomersAuth
                .HasOne(au => au.Account)
                .WithMany(a => a.CustomersAuth)
                .HasForeignKey(au => au.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define foreign key to UsersInfo (many-to-one)
            CustomersAuth
                .HasOne(au => au.User)
                .WithMany(u => u.CustomersAuth)
                .HasForeignKey(au => au.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Define required and unique constraint for UserName
            CustomersAuth
                .Property(au => au.UserName)
                .IsRequired();
            CustomersAuth
                .HasIndex(au => au.UserName)
                .IsUnique();

            // Define required and unique constraint for Email
            CustomersAuth
                .Property(au => au.Email)
                .IsRequired();
            CustomersAuth
                .HasIndex(au => au.Email)
                .IsUnique();

            // Define required constraint for Password
            CustomersAuth
                .Property(au => au.Password)
                .IsRequired();
        }
    }
}
