namespace Database.Builder
{
    public class FinanceBuilder(DbContext context)
    {
        DbContext _context = context;

        public async Task AddAccount(Account account)
        {
            await _context.Set<Account>().AddAsync(account);
            await _context.SaveChangesAsync();
        }
    }

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
