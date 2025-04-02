namespace E_BankingSystem.Data
{
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
            // Configure AccountsAuth Table
            modelBuilder.ApplyConfiguration(new CustomerAuthConfiguration());

            // Configure EmployeesAuth Table
            modelBuilder.ApplyConfiguration(new EmployeeAuthConfiguration());

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
        public DbSet<CustomerAuth> Users { get; set; }
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
