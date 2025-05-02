using System.Linq;
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
        public AccountQuery Query { get; private set; }
        public AccountRepository(EBankingContext context) : base(context) 
        {
            Query = new AccountQuery(context.Accounts.AsQueryable());
        }

        #region Read Methods
        public async Task<Account?> GetAccountByIdAsync(int accountId) => await GetById<Account>(accountId);
        public async Task<Account?> GetAccountByIdAsync(int accountId, IQueryable<Account> query) => await Get<Account>(a => a.AccountId == accountId, query);
        /// <summary>
        /// Retrieves an Account entry asynchronously by its account number.
        /// Optionally accepts a pre-composed IQueryable with desired includes.
        /// </summary>
        /// <param name="accountNumber">The account number of the Account entity.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// </param>
        /// <returns>The Account entity if found or null if not.</returns>
        public async Task<Account?> GetAccountByAccountNumberAsync(string accountNumber, IQueryable<Account>? query = null) => await Get<Account>(a => a.AccountNumber == accountNumber, query);

        /// <summary>
        /// Provides a strongly-typed query builder for the <see cref="Account"/> entity,
        /// enabling fluent filtering and inclusion of related navigation properties.
        /// </summary>
        /// <remarks>
        /// Use this class within the repository layer to compose complex database queries 
        /// by chaining methods that filter by account properties or include related entities.
        /// </remarks>
        public class AccountQuery : CustomQuery<Account, AccountQuery>
        {
            public AccountQuery(IQueryable<Account> account) : base(account) { }
            public AccountQuery HasAccountId(int? accountId) => WhereCondition(a => a.AccountId == accountId);
            public AccountQuery HasAccountTypeId(int? accountTypeId) => WhereCondition(a => a.AccountTypeId == accountTypeId);
            public AccountQuery HasAccountProductTypeId(int? accountProductTypeId) => WhereCondition(a => a.AccountProductTypeId == accountProductTypeId);
            public AccountQuery HasAccountNumber(string? accountNumber) => WhereCondition(a => a.AccountNumber == accountNumber);
            public AccountQuery HasAccountName(string? accountName) => WhereCondition(a => a.AccountName == accountName);
            public AccountQuery ContainsAccountName(string? accountName) => WhereCondition(a => a.AccountName.ToUpper().Contains(accountName!.ToUpper()));
            public AccountQuery HasAccountStatusTypeId(int? accountStatusTypeId) => WhereCondition(a => a.AccountStatusTypeId == accountStatusTypeId);
            public AccountQuery HasBalanceLessThanOrEqualTo(decimal? balance) => WhereCondition(a => a.Balance <= balance);
            public AccountQuery HasBalanceGreaterThanOrEqualTo(decimal? balance) => WhereCondition(a => a.Balance >= balance);
            public AccountQuery HasBeneficiaryAccountId(int? beneficiaryAccountId) => WhereCondition(a => a.LinkedBeneficiaryId == beneficiaryAccountId);
            public AccountQuery HasOpenedOn(DateTime? dateOpened) => WhereCondition(a => a.DateOpened == dateOpened);
            public AccountQuery HasOpenedOnOrBefore(DateTime? dateOpened) => WhereCondition(a => a.DateOpened <= dateOpened);
            public AccountQuery HasOpenedOnOrAfter(DateTime? dateOpened) => WhereCondition(a => a.DateOpened >= dateOpened);
            public AccountQuery HasClosedOn(DateTime? dateClosed) => WhereCondition(a => a.DateClosed == dateClosed);
            public AccountQuery HasClosedOnOrBefore(DateTime? dateClosed) => WhereCondition(a => a.DateClosed <= dateClosed);
            public AccountQuery HasClosedOnOrAfter(DateTime? dateClosed) => WhereCondition(a => a.DateClosed >= dateClosed);
            public AccountQuery IncludeAccountType(bool include = true) => include ? Include(a => a.AccountType) : this;
            public AccountQuery IncludeAccountProductType(bool include = true) => include ? Include(a => a.AccountProductType) : this;
            public AccountQuery IncludeAccountStatusType(bool include = true) => include ? Include(a => a.AccountStatusType) : this;
            public AccountQuery IncludeLinkedBeneficiaryAccount(bool include = true) => include ? Include(a => a.LinkedBeneficiaryAccount) : this;
            public AccountQuery IncludeLinkedSourceAccounts(bool include = true) => include ? Include(a => a.LinkedSourceAccounts) : this;
            public AccountQuery IncludeUsersAuth(bool include = true) => include ? Include(a => a.UsersAuth) : this;
            public AccountQuery IncludeMainTransactions(bool include = true) => include ? Include(a => a.MainTransactions) : this;
            public AccountQuery IncludeCounterTransactions(bool include = true) => include ? Include(a => a.CounterTransactions) : this;
            public AccountQuery IncludeLoans(bool include = true) => include ? Include(a => a.Loans) : this;
            public AccountQuery OrderByDateOpened() => OrderBy(a => a.DateOpened);
            public AccountQuery OrderByDateOpenedDescending() => OrderByDescending(a => a.DateOpened);
        }

        #endregion Read Methods

        #region Update Methods

        #region Account Status Update
        /// <summary>
        /// Asynchronously updates the account status of an account identified by its ID.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account to update.</param>
        /// <param name="accountStatusTypeId">The new account status to set for the account.</param>
        public async Task<bool> AccountStatusUpdateAsync(int accountId, int accountStatusTypeId)
        {
            var account = await this.GetAccountByIdAsync(accountId);
            if (account is not null)
            {
                account.AccountStatusTypeId = accountStatusTypeId;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Asynchronously updates the account status of an account identified by its account number.
        /// </summary>
        /// <param name="accountNumber">The account number of the account to update.</param>
        /// <param name="accountStatusTypeId">The new account status to set for the account.</param>
        public async Task<bool> AccountStatusUpdateAsync(string accountNumber, int accountStatusTypeId)
        {
            var account = await this.GetAccountByAccountNumberAsync(accountNumber);
            if (account is not null)
            {
                account.AccountStatusTypeId = accountStatusTypeId;
                return true;
            }
            else return false;
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
        private string _atmNumber = string.Empty;
        private string _accountName = string.Empty;
        private string _accountContactNo = string.Empty;
        private int _accountStatusTypeId;
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

        public AccountBuilder WithATMNumber(string atmNumber)
        {
            _atmNumber = atmNumber;
            return this;
        }

        public AccountBuilder WithAccountNumber(string accountNumber)
        {
            _accountNumber = accountNumber.Trim();
            return this;
        }

        public AccountBuilder WithAccountContactNo(string accountContactNo)
        {
            _accountContactNo = accountContactNo;
            return this;
        }

        public AccountBuilder WithAccountName(string accountName)
        {
            _accountName = accountName.Trim();
            return this;
        }

        public AccountBuilder WithAccountStatus(int accountStatusTypeId)
        {
            _accountStatusTypeId = accountStatusTypeId;
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
                AccountStatusTypeId = _accountStatusTypeId,
                Balance = _balance,
                DateOpened = _dateOpened,
                DateClosed = _dateClosed
            };
        }
    }
}
