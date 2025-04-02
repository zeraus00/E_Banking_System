namespace E_BankingSystem.Data.Configurations.Authentication
{
    public class CustomerAuthConfiguration : IEntityTypeConfiguration<CustomerAuth>
    {
        // Configure CustomersAuth Table
        public void Configure(EntityTypeBuilder<CustomerAuth> CustomersAuth)
        {
            CustomersAuth.ToTable("CustomersAuth", "Authentication");

            // Define primary key
            CustomersAuth
                .HasKey(ca => ca.CustomerAuthId);
            CustomersAuth
                .Property(ca => ca.CustomerAuthId)
                .ValueGeneratedOnAdd();

            // Define foreign key to Accounts (many-to-one)
            CustomersAuth
                .Property(ca => ca.AccountId)
                .IsRequired();
            CustomersAuth
                .HasOne(ca => ca.Account)
                .WithMany(a => a.CustomersAuth)
                .HasForeignKey(ca => ca.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define foreign key to UsersInfo (many-to-one)
            CustomersAuth
                .HasOne(ca => ca.UserInfo)
                .WithMany(u => u.CustomersAuth)
                .HasForeignKey(ca => ca.UserInfoId)
                .OnDelete(DeleteBehavior.SetNull);

            // Define required and unique constraint for UserName
            CustomersAuth
                .Property(ca => ca.UserName)
                .IsRequired();
            CustomersAuth
                .HasIndex(ca => ca.UserName)
                .IsUnique();

            // Define required and unique constraint for Email
            CustomersAuth
                .Property(ca => ca.Email)
                .IsRequired();
            CustomersAuth
                .HasIndex(ca => ca.Email)
                .IsUnique();    

            // Define required constraint for Password
            CustomersAuth
                .Property(ca => ca.Password)
                .IsRequired();
        }
    }
}
