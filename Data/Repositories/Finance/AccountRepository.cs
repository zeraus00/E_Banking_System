using Exceptions;
using Microsoft.Identity.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        /// <summary>
        /// Retrieves an Account entry by its primary key.
        /// Optionally accepts a pre-composed IQueryable with desired includes.
        /// </summary>
        /// <param name="accountId">The primary key of the Account entity.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// If null, a basic lookup using DbContext.Find is performed.
        /// </param>
        /// <returns>The Account entity if found or null if not.</returns>
        public Account? GetAccountByIdSync(int accountId, IQueryable<Account>? query = null)
        {
            Account? account;
            if (query != null)
            {
                account = query.FirstOrDefault(a => a.AccountId == accountId);
            } else
            {
                account = _context
                    .Accounts
                    .Find(accountId);
            }
            return account;
        }

        /// <summary>
        /// Retrieves an Account entry by its primary key asynchronously.
        /// Optionally accepts a pre-composed IQueryable with desired includes.
        /// </summary>
        /// <param name="accountId">The primary key of the Account entity.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// If null, a basic lookup using DbContext.Find is performed.
        /// </param>
        /// <returns>The Account entity if found or null if not.</returns>
        public async Task<Account?> GetAccountByIdAsync(int accountId, IQueryable<Account>? query = null)
        {
            Account? account;
            if (query != null)
            {
                account = await query.FirstOrDefaultAsync(a => a.AccountId == accountId);
            }
            else
            {
                account = await _context
                    .Accounts
                    .FindAsync(accountId);
            }
            return account;
        }

        /// <summary>
        /// Retrieves an Account entry by its account number.
        /// Optionally accepts a pre-composed IQueryable with desired includes.
        /// </summary>
        /// <param name="accountNumber">The account number of the Account entity.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// </param>
        /// <returns>The Account entity if found or null if not.</returns>
        public Account? GetAccountByAccountNumberSync(string accountNumber, IQueryable<Account>? query = null)
        {
            var trimmedAccountNumber = accountNumber.Trim();
            Account? account;
            if (query != null)
            {
                account = query.FirstOrDefault(a => a.AccountNumber == accountNumber);
            }
            else
            {
                account = _context
                    .Accounts
                    .FirstOrDefault(a => a.AccountNumber == accountNumber);
            }
            return account;
        }

        /// <summary>
        /// Retrieves an Account entry asynchronously by its account number.
        /// Optionally accepts a pre-composed IQueryable with desired includes.
        /// </summary>
        /// <param name="accountNumber">The account number of the Account entity.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// </param>
        /// <returns>The Account entity if found or null if not.</returns>
        public async Task<Account?> GetAccountByAccountNumberAsync(string accountNumber, IQueryable<Account>? query = null)
        {
            var trimmedAccountNumber = accountNumber.Trim();
            Account? account;
            if (query != null)
            {
                account = await query.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            }
            else
            {
                account = await _context
                    .Accounts
                    .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            }
            return account;
        }

        public IQueryable<Account> ComposeAccountQuery(
            bool includeAccountType = false,
            bool includeLinkedBeneficiaryAccount = false,
            bool includeLinkedSourceAccounts = false,
            bool includeUsersAuth = false,
            bool includeTransactions = false,
            bool includeLoans = false,
            bool includeLoanTransactions = false
            )
        {
            var query = _context
                .Accounts
                .AsQueryable();

            if (includeAccountType) { query = query.Include(a => a.AccountType); }
            if (includeLinkedBeneficiaryAccount) { query = query.Include(a => a.LinkedBeneficiaryAccount); }
            if (includeLinkedSourceAccounts) { query = query.Include(a => a.LinkedSourceAccounts); }
            if (includeUsersAuth) { query = query.Include(a => a.UsersAuth); }
            if (includeTransactions) { query = query.Include(a => a.Transactions); }
            if (includeLoans) { query = query.Include(a => a.Loans); }
            if (includeLoanTransactions) { query = query.Include(a => a.LoanTransactions); }

            return query;
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
