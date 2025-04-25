using Data.Models.Finance;
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

        #region Read Methods
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

        /// <summary>
        /// Builds an IQueryable for querying the Accounts table that includes all related entities.
        /// </summary>
        /// <returns>An IQueryable of Account with all includes.</returns>
        public IQueryable<Account> QueryIncludeAll()
        {
            return this.ComposeQuery(
                includeAccountType: true,
                includeLinkedBeneficiaryAccount: true,
                includeLinkedSourceAccounts: true,
                includeUsersAuth: true,
                includeTransactions: true,
                includeLoans: true,
                includeLoanTransactions: true
                );
        }
        
        /// <summary>
        /// Composes an <see cref="IQueryable{Account}"/> for retrieving account entities with optional related data.
        /// </summary>
        /// <param name="includeAccountType">Whether to include the associated <c>AccountType</c> entity.</param>
        /// <param name="includeLinkedBeneficiaryAccount">Whether to include the linked beneficiary account.</param>
        /// <param name="includeLinkedSourceAccounts">Whether to include linked source accounts.</param>
        /// <param name="includeUsersAuth">Whether to include user authentication information associated with the account.</param>
        /// <param name="includeTransactions">Whether to include related transactions.</param>
        /// <param name="includeLoans">Whether to include loans associated with the account.</param>
        /// <param name="includeLoanTransactions">Whether to include loan transactions associated with the account.</param>
        /// <returns>
        /// A queryable sequence of <see cref="Account"/> entities, with specified navigation properties included as needed.
        /// </returns>
        public IQueryable<Account> ComposeQuery(
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

        #endregion Read Methods

        #region Update Methods

        #region Account Status Update
        /// <summary>
        /// Asynchronously updates the account status of an account identified by its ID.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account to update.</param>
        /// <param name="accountStatus">The new account status to set for the account.</param>
        public async Task AccountStatusUpdateAsync(int accountId, string accountStatus)
        {
            var account = await this.GetAccountByIdAsync(accountId);
            if (account is not null) account.AccountStatus = accountStatus;
        }

        /// <summary>
        /// Asynchronously updates the account status of an account identified by its account number.
        /// </summary>
        /// <param name="accountNumber">The account number of the account to update.</param>
        /// <param name="accountStatus">The new account status to set for the account.</param>
        public async Task AccountStatusUpdateAsync(string accountNumber, string accountStatus)
        {
            var account = await this.GetAccountByAccountNumberAsync(accountNumber);
            if (account is not null) account.AccountStatus = accountStatus;
        }
        #endregion Account Status Update

        #region Balance Update

        /// <summary>
        /// Asynchronously updates the balance of an account identified by its ID.
        /// </summary>
        /// <param name="accountId">The primary key of the account to update.</param>
        /// <param name="newBalance">The new balance to set for the account.</param>
        public async Task UpdateBalanceAsync(int accountId, decimal newBalance)
        {
            var account = await this.GetAccountByIdAsync(accountId);
            if (account is not null) account.Balance = newBalance;
        }

        /// <summary>
        /// Asynchronously updates the balance of an account identified by its account number.
        /// </summary>
        /// <param name="accountNumber">The account number of the account to update.</param>
        /// <param name="newBalance">The new balance to set for the account.</param>
        public async Task UpdateBalanceAsync(string accountNumber, decimal newBalance)
        {
            var account = await this.GetAccountByAccountNumberAsync(accountNumber);
            if (account is not null) account.Balance = newBalance;
        }
        #endregion Balance Update

        #region Date Closed update
        /// <summary>
        /// Asynchronously updates the date closed for an account identified by its ID.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account to update.</param>
        /// <param name="dateClosed">The new date closed to set for the account.</param>
        public async Task DateClosedUpdateAsync(int accountId, DateTime dateClosed)
        {
            var account = await this.GetAccountByIdAsync(accountId);
            if (account is not null) account.DateClosed = dateClosed;
        }

        /// <summary>
        /// Asynchronously updates the date closed for an account identified by its account number.
        /// </summary>
        /// <param name="accountNumber">The account number of the account to update.</param>
        /// <param name="dateClosed">The new date closed to set for the account.</param>
        public async Task DateClosedUpdateAsync(string accountNumber, DateTime dateClosed)
        {
            var account = await this.GetAccountByAccountNumberAsync(accountNumber);
            if (account is not null) account.DateClosed = dateClosed;
        }
        #endregion DateClosed Update

        #endregion Update Methods
    }

    /// <summary>
    /// Builder class for Account
    /// </summary>
    public class AccountBuilder
    {
        #region Table Fields
        private int _accountTypeId;
        private int _accountProductTypeId;
        private string _accountNumber = string.Empty;
        private string _accountName = string.Empty;
        private string _accountStatus = string.Empty;
        private decimal _balance = 0.0m;
        private int? _linkedBeneficiaryId;
        private DateTime _dateOpened = DateTime.UtcNow;
        private DateTime? _dateClosed;
        #endregion Table Fields

        #region Builder Methods
        public AccountBuilder WithAccountType(int accountTypeId)
        {
            _accountTypeId = accountTypeId;
            return this;
        }

        public AccountBuilder WithAccountProductTypeId(int accountProductTypeId)
        {
            _accountProductTypeId = accountProductTypeId;
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
        #endregion Builder Methods

        /// <summary>
        /// Builds the Account object with the specified properties
        /// </summary>
        /// <returns></returns>
        public Account Build()
        {
            return new Account
            {
                AccountTypeId = _accountTypeId,
                AccountProductTypeId = _accountProductTypeId,
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
