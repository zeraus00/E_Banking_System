namespace Data
{
    public class EBankingContext : DbContext
    {
        public EBankingContext(DbContextOptions<EBankingContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*  Configure Authentication Schema
             *  Configure UsersAuth Table
             *  Configure Roles Table
             */
            modelBuilder.ApplyConfiguration(new UserAuthConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            /*  Configure Finance Schema
             *  Configure Accounts Table
             *  Configure Loans Table
             *  Configure LoanTransactions Table
             *  Configure LoanTypes Table
             *  Configure Transactions Table
             *  Configure TransactionTypes Table 
             */
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new LoanConfiguration());
            modelBuilder.ApplyConfiguration(new LoanTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new LoanTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionTypeConfiguration());

            /*  Configure Place Schema
             *  Configure Barangays Table
             *  Configure Cities Table
             *  Configure Provinces Table
             *  Configure Regions Table
             */
            modelBuilder.ApplyConfiguration(new BarangayConfiguration());
            modelBuilder.ApplyConfiguration(new CityConfiguration());
            modelBuilder.ApplyConfiguration(new ProvinceConfiguration());
            modelBuilder.ApplyConfiguration(new RegionConfiguration());

            /*  Configure User Schema
             *  Configure Address Table
             *  Configure BirthsInfo Table
             *  Configure UsersInfo Table
             */
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new BirthInfoConfiguration());
            modelBuilder.ApplyConfiguration(new UserInfoConfiguration());


            base.OnModelCreating(modelBuilder);
        }

        /*  Add DbSet for each model    */

        //  Authentication
        public DbSet<UserAuth> UsersAuth { get; set; }
        public DbSet<Role> Roles { get; set; }

        //  Finance
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<LoanTransaction> LoanTransactions { get; set; }
        public DbSet<LoanType> LoanTypes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }

        //  Place
        public DbSet<Region> Regions { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Barangay> Barangays { get; set; }

        //  User
        public DbSet<Address> Addresses { get; set; }
        public DbSet<BirthInfo> BirthInfos { get; set; }
        public DbSet<UserInfo> UsersInfo { get; set; }

    }

}
