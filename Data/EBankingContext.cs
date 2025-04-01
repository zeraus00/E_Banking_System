namespace E_Banking.Data
{
    using Microsoft.EntityFrameworkCore;
    using E_Banking.Models.User;
    using E_Banking.Models.Place;
    using E_Banking.Models.Authentication;
    using E_Banking.Models.Finance;

    public class EBankingContext : DbContext
    {
        public EBankingContext(DbContextOptions<EBankingContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySql();
            optionsBuilder.UseInMemoryDatabase("EBankingDb");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*  AccountsAuth Table  */
            var AccountsAuth = modelBuilder.Entity<AccountAuth>();
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
            /*  Accounts Table  */
            var Accounts = modelBuilder.Entity<Account>();
            Accounts.ToTable("Accounts");

            Accounts.HasKey(a => a.AccountId);
            Accounts.Property(a => a.AccountId).ValueGeneratedOnAdd();

            Accounts.Property(a => a.AccountType).IsRequired()
                .HasMaxLength(20);

            Accounts.Property(a => a.AccountNumber).IsRequired()
                .HasMaxLength(12)
                .IsFixedLength();

            Accounts.Property(a => a.AccountStatus).IsRequired()
                .HasMaxLength(10);

            Accounts.Property(a => a.Balance).IsRequired()
                .HasDefaultValue(0);


            base.OnModelCreating(modelBuilder);
        }

        // Add DbSet for each model
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountAuth> Users { get; set; }
        public DbSet<UserInfo> UsersDetail { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<BirthInfo> BirthRecords { get; set; }

        public DbSet<Region> Regions { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Barangay> Barangays { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
    }

}
