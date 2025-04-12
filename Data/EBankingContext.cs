using Database.Builder;

namespace Data
{
    public class EBankingContext : DbContext
    {
        /*
        public EBankingContext(DbContextOptions<EBankingContext> options) : base(options)
        {
        }
        */

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySql("Server=localhost;Port=3306;Database=ebankingDB;User Id=root;Password=admin1234;", new MySqlServerVersion(new Version(8, 0, 41)))
                .UseSeeding((context, _) =>
                {
                    var testAuth  = context.Set<CustomerAuth>().FirstOrDefault(ca => ca.Email == "testemail@gmail.com");
                    if (testAuth == null)
                    {
                        var customerAuthBuilder = new CustomerAuthBuilder();
                        customerAuthBuilder
                            .WithEmail("testemail@gmail.com")
                            .WithUserName("testuser")
                            .WithPassword("testpassword");

                        CustomerAuth customerAuth = customerAuthBuilder.Build();
                        var authenticationBuilder = new AuthenticationBuilder(context);

                        authenticationBuilder.AddCustomerAuth(customerAuth);
                    }
                })
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    var testAuth = context.Set<CustomerAuth>().FirstOrDefault(ca => ca.Email == "testemail@gmail.com");
                    if (testAuth == null)
                    {
                        var customerAuthBuilder = new CustomerAuthBuilder();
                        customerAuthBuilder
                            .WithEmail("testemail@gmail.com")
                            .WithUserName("testuser")
                            .WithPassword("testpassword");

                        CustomerAuth customerAuth = customerAuthBuilder.Build();
                        var authenticationBuilder = new AuthenticationBuilder(context);

                        await authenticationBuilder.AddCustomerAuthAsync(customerAuth);
                    }
                }
                );
            //optionsBuilder.UseInMemoryDatabase("EBankingDb");
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
        public DbSet<CustomerAuth> CustomersAuth { get; set; }
        public DbSet<EmployeeAuth> EmployeesAuth { get; set; }
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
        public DbSet<UserInfo> UserInfos { get; set; }

    }

}
