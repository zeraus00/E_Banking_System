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
            /*  Configure Authentication Schema
             *  Configure CustomerAuth Table
             *  Configure EmployeeAuthTable
             *  Configure Roles Table
             */
            modelBuilder.ApplyConfiguration(new CustomerAuthConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeAuthConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            /*  Configure Finance Schema
             *  Configure Accounts Table
             *  Configure Transactions Table
             *  Configure TransactionTypes Table 
             */
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionTypeConfig());

            /*  Configure Place Schema
             *  Configure Barangays Table
             *  Configure Cities Table
             *  Configure Provinces Table
             *  Configure Regions Table
             */
            modelBuilder.ApplyConfiguration(new BarangayConfiguration());
            modelBuilder.ApplyConfiguration(new CityConfiguration());
            modelBuilder.ApplyConfiguration(new ProvinceConfiguration());


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
