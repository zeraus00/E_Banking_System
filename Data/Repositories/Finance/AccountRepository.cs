using Exceptions;

namespace Data.Repositories.Finance
{
    /// <summary>
    /// CRUD operations handler for Accounts table.
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// </summary>
    /// <param name="context"></param>
    public class AccountRepository : Repository
    {
        public AccountRepository(EBankingContext context) : base(context) { }
        public Account GetAccountByIdSync(int accountId)
        {
            //
            var account = _context.Set<Account>().Find(accountId);
            if (account == null)
            {
                throw new AccountNotFoundException(accountId);
            }
            return account;
        }

        /// <summary>
        /// Gets an account object by querying with the primary key.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<Account> GetAccountByIdAsync(int accountId)
        {
            var account = await _context.Set<Account>().FindAsync(accountId);
            if (account == null)
            {
                throw new AccountNotFoundException(accountId);
            }
            return account;
        }

        /// <summary>
        /// Gets an account object by querying with the account number.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public Account GetAccountByAccountNumberSync(string accountNumber)
        {
            var trimmedAccountNumber = accountNumber.Trim();
            var account = _context
                .Set<Account>()
                .FirstOrDefault(
                    acc => acc.AccountNumber == trimmedAccountNumber
                );
            if (account == null)
            {
                throw new AccountNotFoundException(accountNumber);
            }
            return account;
        }

        /// <summary>
        /// Gets an account object by querying with the account number.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public async Task<Account> GetAccountByAccountNumberAsync(string accountNumber)
        {
            var trimmedAccountNumber = accountNumber.Trim();
            var account = await _context
                .Set<Account>()
                .FirstOrDefaultAsync(
                    acc => acc.AccountNumber == trimmedAccountNumber
                );
            if (account == null)
            {
                throw new AccountNotFoundException(accountNumber);
            }
            return account;
        }
    }

    /// <summary>
    /// Builder class for Account
    /// </summary>
    public class AccountBuilder
    {
        private int _accountTypeId;
        private string _accountNumber = string.Empty;
        private string _accountName = string.Empty;
        private string _accountStatus = string.Empty;
        private decimal _balance = 0.0m;
        private int? _linkedBeneficiaryId;
        private DateTime _dateOpened = DateTime.UtcNow;
        private DateTime? _dateClosed;

        public AccountBuilder WithAccountType(int accountTypeId)
        {
            _accountTypeId = accountTypeId;
            return this;
        }

        public AccountBuilder WithAccountNumber(string accountNumber)
        {
            _accountNumber = accountNumber.Trim();
            return this;
        }

        public AccountBuilder WithAccountName(string accountName)
        {
            _accountName = accountName.Trim();
            return this;
        }

        public AccountBuilder WithAccountStatus(string accountStatus)
        {
            _accountStatus = accountStatus.Trim();
            return this;
        }

        public AccountBuilder WithBalance(decimal balance)
        {
            _balance = balance;
            return this;
        }

        public AccountBuilder WithLinkedBeneficiaryId(int linkedBeneficiaryId)
        {
            _linkedBeneficiaryId = linkedBeneficiaryId;
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
        public Account Build()
        {
            return new Account
            {
                AccountTypeId = _accountTypeId,
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
