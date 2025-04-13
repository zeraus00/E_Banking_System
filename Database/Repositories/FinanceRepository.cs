namespace Database.Repositories
{
    /// <summary>
    /// CRUD operations handler for FinanceSchema
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// </summary>
    public class FinanceRepository
    {
        DbContext _context;

        public FinanceRepository(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new Account to the database
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>

        public async Task AddAccount(Account account)
        {
            await _context.Set<Account>().AddAsync(account);
            await _context.SaveChangesAsync();
        }

    }
    /// <summary>
    /// Builder class for Account
    /// </summary>
    public class AccountBuilder
    {
        private string _accountType = string.Empty;
        private string _accountNumber = string.Empty;
        private string _accountName = string.Empty;
        private string _accountStatus = string.Empty;
        private decimal _balance = 0.0m;
        private DateTime _dateOpened = DateTime.UtcNow;
        private DateTime? _dateClosed;

        public AccountBuilder WithAccountType(string accountType)
        {
            _accountType = accountType;
            return this;
        }

        public AccountBuilder WithAccountNumber(string accountNumber)
        {
            _accountNumber = accountNumber;
            return this;
        }

        public AccountBuilder WithAccountName(string accountName)
        {
            _accountName = accountName;
            return this;
        }

        public AccountBuilder WithAccountStatus(string accountStatus)
        {
            _accountStatus = accountStatus;
            return this;
        }

        public AccountBuilder WithBalance(decimal balance)
        {
            _balance = balance;
            return this;
        }

        public AccountBuilder WithDateOpened(DateTime dateOpened)
        {
            _dateOpened = dateOpened;
            return this;
        }

        public AccountBuilder WithDateClosed(DateTime? dateClosed)
        {
            _dateClosed = dateClosed;
            return this;
        }

        /// <summary>
        /// Builds the Account object with the specified properties
        /// </summary>
        /// <returns></returns>
        public Account Builder()
        {
            return new Account
            {
                AccountType = _accountType,
                AccountNumber = _accountNumber,
                AccountName = _accountName,
                AccountStatus = _accountStatus,
                Balance = _balance,
                DateOpened = _dateOpened,
                DateClosed = _dateClosed
            };
        }
    }
}
