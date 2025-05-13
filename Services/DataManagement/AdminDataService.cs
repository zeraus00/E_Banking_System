using System.Globalization;
using System.Linq;
using Data;
using Data.Constants;
using Data.Enums;
using Data.Models.Finance;
using Data.Repositories.Auth;
using Data.Repositories.Finance;
using Data.Repositories.JointEntity;
using Data.Repositories.User;
using Exceptions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Services.SessionsManagement;
using ViewModels.AdminDashboard;
using static Data.Repositories.Finance.TransactionRepository;

namespace Services.DataManagement
{
    public class AdminDataService : Service
    {
        private readonly DataMaskingService _dataMaskingService;
        private readonly UserDataService _userDataService;
        private readonly UserSessionService _userSessionService;
        public AdminDataService(
            IDbContextFactory<EBankingContext> contextFactory,
            DataMaskingService dataMaskingService,
            UserDataService userDataService,
            UserSessionService userSessionService
            ) : base(contextFactory)
        {
            _dataMaskingService = dataMaskingService;
            _userDataService = userDataService;
            _userSessionService = userSessionService;
        }
        #region Manage Account Helper Methods
        /// <summary>
        /// Asynchronously filters and retrieves a list of pending <see cref="Account"/> based on optional criteria.
        /// </summary>
        /// <param name="accountNumber">Optional account number to filter by. If empty or null, this filter is ignored.</param>
        /// <param name="startDate">Optional start date to filter accounts opened on or after this date. If null, this filter is ignored.</param>
        /// <param name="endDate">Optional end date to filter accounts opened on or before this date. If null, this filter is ignored.</param>
        /// <param name="accountTypeId">Optional account type ID to filter by. Only applies if greater than zero.</param>
        /// <returns>A list of <see cref="Account"> objects matching the specified filters.</returns>
        public async Task<List<Account>> LoadAccountsListAsync(
            string accountNumber = "",
            DateTime? startDate = null,
            DateTime? endDate = null,
            int accountTypeId = 0,
            int accountStatusTypeId = 0,
            int pageNumber = 1,
            int pageSize = 5
            )
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                //  Declare repository dependencies.
                var accountRepo = new AccountRepository(dbContext);

                //  Filter the accounts.
                var queryBuilder = accountRepo.Query
                    .IncludeAccountType()
                    .IncludeAccountStatusType()
                    .OrderByDateOpenedDescending();

                if (!string.IsNullOrWhiteSpace(accountNumber))
                {
                    if (accountNumber.Length == 12)
                        queryBuilder.HasAccountNumber(accountNumber);
                    else if (accountNumber.Length == 3)
                        queryBuilder.AccountNumberEndsWith(accountNumber);
                }
                if (startDate is not null)
                    queryBuilder.HasOpenedOnOrAfter(startDate);
                if (endDate is not null)
                    queryBuilder.HasOpenedOnOrBefore(endDate);
                if (accountTypeId > 0)
                    queryBuilder.HasAccountTypeId(accountTypeId);
                if (accountStatusTypeId > 0)
                    queryBuilder.HasAccountStatusTypeId(accountStatusTypeId);

                //  Pagination
                int skipCount = (pageNumber - 1) * pageSize;
                int takeCount = pageSize;
                List<Account> accountList = await queryBuilder
                    .SkipBy(skipCount)
                    .TakeWithCount(takeCount)
                    .GetQuery()
                    .ToListAsync();

                //  Mask the user's account number.
                foreach (var account in accountList)
                {
                    account.AccountNumber = _dataMaskingService.MaskAccountNumber(account.AccountNumber);
                }

                //  Get the query as list
                return accountList;
            }
        }
        public async Task<int> CountRemainingAccountsAsync(
            string accountNumber = "",
            DateTime? startDate = null,
            DateTime? endDate = null,
            int accountTypeId = 0,
            int accountStatusTypeId = 0,
            int pageNumber = 1,
            int pageSize = 5)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var accountRepo = new AccountRepository(dbContext);

                var queryBuilder = accountRepo
                    .Query
                    .IncludeAccountType()
                    .IncludeAccountStatusType()
                    .OrderByDateOpenedDescending();

                if (!string.IsNullOrWhiteSpace(accountNumber))
                {
                    if (accountNumber.Length == 12)
                        queryBuilder.HasAccountNumber(accountNumber);
                    else if (accountNumber.Length == 3)
                        queryBuilder.AccountNumberEndsWith(accountNumber);
                }
                if (startDate is not null)
                    queryBuilder.HasOpenedOnOrAfter(startDate);
                if (endDate is not null)
                    queryBuilder.HasOpenedOnOrBefore(endDate);
                if (accountTypeId > 0)
                    queryBuilder.HasAccountTypeId(accountTypeId);
                if (accountStatusTypeId > 0)
                    queryBuilder.HasAccountStatusTypeId(accountStatusTypeId);

                //  Pagination
                int skipCount = pageNumber * pageSize;
                return await queryBuilder
                    .SkipBy(skipCount)
                    .GetQuery()
                    .CountAsync();
            }
        }
        /// <summary>
        /// Retrieves the <see cref="UserInfo"> of the first primary owner of the <see cref="Account"> 
        /// with the specified <see cref="UserInfo.UserInfoId">. Mainly used for tracking the owner 
        /// that registered an <see cref="Account">.
        /// </summary>
        /// <param name="accountId">The <see cref="Account.AccountId"/> of the primary owner.</returns>
        public async Task<UserInfo> GetAccountPrimaryOwner(int accountId)
        {
            try
            {
                await using (var dbContext = await _contextFactory.CreateDbContextAsync())
                {
                    var userInfoAccountRepo = new UserInfoAccountRepository(dbContext);
                    var userInfoRepo = new UserInfoRepository(dbContext);

                    //  Get the UserInfoId of the first primary owner linked to the account.
                    int userInfoId = await userInfoAccountRepo
                        .Query
                        .HasAccountId(accountId)
                        .HasAccessRoleId((int)AccessRoleIDs.PRIMARY_OWNER)
                        .SelectUserInfoId();

                    //  Prepare a query for the user info table using the UserInfoId.
                    var userInfoQuery = userInfoRepo
                        .Query
                        .HasUserInfoId(userInfoId)
                        .IncludeUserAuth()
                        .IncludeUserName()
                        .IncludeFatherName()
                        .IncludeMotherName()
                        .IncludeReligion()
                        .GetQuery();

                    //  Execute the query. Throws UserNotFoundException if UserInfo is not found.
                    //  FirstOrDefaultAsync is used to retrieve the first user associated with the account, 
                    //  the same user that registers it.
                    return await userInfoQuery.FirstOrDefaultAsync() ?? throw new UserNotFoundException();
                }
            }
            catch (AccountNotFoundException ex)
            {
                Console.WriteLine("COULD NOT FETCH PRIMARY OWNER. " + ex.Message);
                throw;
            }
            catch (UserNotFoundException ex)
            {
                Console.WriteLine("COULD NOT FETCH PRIMARY OWNER. " + ex.Message);
                throw;
            }

        }
        #endregion
        #region Pending Accounts Helper Methods
        /// <summary>
        /// Updates the <see cref="Account.AccountStatusType"> of an <see cref="Account"/> with a
        /// specified <see cref="Account.AccountId"/> and a <see cref="Account.AccountStatusTypeId"/>
        /// of <see cref="AccountStatusTypeIDs.Pending"/>.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public async Task UpdateAccountStatus(int accountId, int newStatus)
        {
            try
            {
                await using (var dbContext = await _contextFactory.CreateDbContextAsync())
                {
                    var accountRepo = new AccountRepository(dbContext);
                    bool isAccountUpdated = await accountRepo.AccountStatusUpdateAsync(accountId, newStatus);
                    if (!isAccountUpdated) throw new AccountNotFoundException(accountId);

                    await accountRepo.SaveChangesAsync();
                }
            }
            catch (AccountNotFoundException ex)
            {
                Console.WriteLine($"ACCOUNT_STATUS_UPDATE_FAILED: {ex.Message}");
                throw;
            }
        }
        #endregion
        #region UserAuth Data Helper Methods
        public async Task<string> GetUserEmailAsync(int userInfoId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                UserAuth userAuth = await new UserInfoRepository(dbContext)
                    .Query
                    .IncludeUserAuth()
                    .HasUserInfoId(userInfoId)
                    .SelectUserAuth() ?? throw new UserNotFoundException();

                return userAuth.Email;    
            }
        }
        #endregion
        #region Account Data Helper Methods
        public async Task<int> GetAccountIdAsync(string accountNumber)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var accountRepo = new AccountRepository(dbContext);

                var queryBuilder = accountRepo.Query;

                if (accountNumber.Length == 12)
                    queryBuilder.HasAccountNumber(accountNumber);
                else if (accountNumber.Length == 3)
                    queryBuilder.AccountNumberEndsWith(accountNumber);
                return await queryBuilder.SelectId();
            }
        }
        public async Task<string> GetAccountNumberAsync(int accountId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                string accountNumber = await new AccountRepository(dbContext)
                    .Query
                    .HasAccountId(accountId)
                    .SelectAccountNumber();

                return _dataMaskingService.MaskAccountNumber(accountNumber);
            }
        }
        public async Task<string> GetAccountNameAsync(int accountId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                return await new AccountRepository(dbContext)
                    .Query
                    .HasAccountId(accountId)
                    .SelectAccountName();
            }
        }
        public async Task<decimal> GetAccountBalanceAsync(int accountId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                return await new AccountRepository(dbContext)
                    .Query
                    .HasAccountId(accountId)
                    .SelectBalance();
            }
        }
        #endregion
        #region Loan Page Helper Methods
        public async Task<Loan?> GetLoanByIdAsync(int loanId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var loanRepo = new LoanRepository(dbContext);
                return await loanRepo
                    .Query
                    .IncludeLoanType()
                    .HasLoanId(loanId)
                    .GetQuery()
                    .FirstOrDefaultAsync();
            }
        }
        public async Task<List<Loan>> LoadLoansListAsync(
            string accountNumber = "",
            DateTime? startDate = null,
            DateTime? endDate = null,
            string loanStatus = "",
            int pageSize = 5,
            int pageNumber = 1)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var loanRepo = new LoanRepository(dbContext);

                var queryBuilder = loanRepo
                    .Query
                    .OrderByDateDescending();

                //  Apply filters as needed.
                if (!string.IsNullOrWhiteSpace(accountNumber))
                {
                    int accountId = await GetAccountIdAsync(accountNumber);
                    queryBuilder.HasAccountId(accountId);
                }
                if (startDate is DateTime sDate)
                    queryBuilder.LoanApplicationOrOrAfter(sDate);
                if (endDate is DateTime eDate)
                    queryBuilder.LoanApplicationOnOrBefore(eDate);
                if (!string.IsNullOrWhiteSpace(loanStatus))
                    queryBuilder.HasStatus(loanStatus);

                //  Count the number of loans from the first page until the previous page.
                int skipCount = (pageNumber - 1) * pageSize;
                //  Specify the number of loans to be displayed in a page.
                int takeCount = pageSize;

                //  Get the list of loans.
                List<Loan> loanList = await queryBuilder
                    .SkipBy(skipCount)
                    .TakeWithCount(takeCount)
                    .GetQuery()
                    .ToListAsync();
                return loanList;
            }
        }
        public async Task<int> CountRemainingLoansAsync(
            string accountNumber = "",
            DateTime? startDate = null,
            DateTime? endDate = null,
            string loanStatus = "",
            int pageSize = 5,
            int pageNumber = 1)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var loanRepo = new LoanRepository(dbContext);

                var queryBuilder = loanRepo.Query;

                //  Apply filters as needed.
                if (!string.IsNullOrWhiteSpace(accountNumber))
                {
                    int accountId = await GetAccountIdAsync(accountNumber);
                    queryBuilder.HasAccountId(accountId);
                }
                if (startDate is DateTime sDate)
                    queryBuilder.LoanApplicationOrOrAfter(sDate);
                if (endDate is DateTime eDate)
                    queryBuilder.LoanApplicationOnOrBefore(eDate);
                if (!string.IsNullOrWhiteSpace(loanStatus))
                    queryBuilder.HasStatus(loanStatus);

                //  Count the number of loans from the first page until the current page.
                int skipCount = pageNumber * pageSize;
                //  Get count of the remaining loans in the list.
                return await queryBuilder
                    .SkipBy(skipCount)
                    .GetQuery()
                    .CountAsync();
            }
        }
        public async Task UpdateLoanStatus(int loanId, string newStatus)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var loanRepo = new LoanRepository(dbContext);

                Loan loan = await loanRepo.GetLoanById(loanId) ?? throw new NullReferenceException();

                loan.LoanStatus = newStatus;

                await loanRepo.SaveChangesAsync();
            }
        }
        #endregion
        #region Dashboard Helper Methods
        #region Account Filtering
        /// <summary>
        /// Retrieves the count of newly opened <see cref="Account"> in a specified time frame or
        /// a default time frame starting from the start of the month until the most recent day.
        /// </summary>
        /// <param name="dateOpenedStart">Specifies the earliest <see cref="Account.DateOpened"> value.</param>
        /// <param name="dateOpenedEnd">Specifies the latest <see cref="Account.DateOpened"> value.</param>
        /// <returns>The <see cref="int"/> count of newly opened <see cref="Account"/>.</returns>
        public async Task<int> GetNewAccountsCount(DateTime? dateOpenedStart = null, DateTime? dateOpenedEnd = null) =>
            await GetAccountCountByFilterAsync((int)AccountStatusTypeIDs.New, dateOpenedStart, dateOpenedEnd);
        /// <summary>
        /// Retrieves the count of closed <see cref="Account"/> in a specified time frame or
        /// a default time frame starting from the start of the month until the most recent day.
        /// </summary>
        /// <param name="dateClosedStart">Specifies the earliest <see cref="Account.DateClosed"> value.</param>
        /// <param name="dateClosedEnd">Specifies the latest <see cref="Account.DateClosed"> value.</param>
        /// <returns>The <see cref="int"/> count of closed <see cref="Account"/>.</returns>
        public async Task<int> GetClosedAccountsCount(DateTime? dateClosedStart = null, DateTime? dateClosedEnd = null) =>
            await GetAccountCountByFilterAsync((int)AccountStatusTypeIDs.Closed, dateClosedStart, dateClosedEnd);
        /// <summary>
        /// Retrieves the count of <see cref="Account"/> with the specified <see cref="Account.AccountStatusTypeId"> 
        /// and time frame or a default time frame starting from the start of the month until the most recent day.
        /// </summary>
        /// <param name="statusTypeId">The <see cref="Account.AccountStatusTypeId"/> of the accounts specified through
        /// <see cref="AccountStatusTypeIDs"/>Specifies the <<see cref="Account.AccountStatusTypeId"/>>of the accounts 
        /// being queried.</param>
        /// <param name="startDate">Specifies the earliest date for the query.</param>
        /// <param name="endDate">Specifies the latest date for the query.</param>
        /// <returns>An <see cref="int"/> representing the count of Accounts in the query after the filter.</returns>
        public async Task<int> GetAccountCountByFilterAsync(int statusTypeId, DateTime? startDate = null, DateTime? endDate = null)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                //  Set timestamps.
                var now = DateTime.Now;
                startDate = startDate ?? now.AddDays(-now.Day + 1);
                endDate = endDate ?? now;

                var accountRepo = new AccountRepository(dbContext);
                var queryBuilder = accountRepo.Query;

                bool IsStatusNew = statusTypeId == (int)AccountStatusTypeIDs.New;
                bool IsStatusClosed = statusTypeId == (int)AccountStatusTypeIDs.Closed;
                //  Set time stamps based on status.
                if (IsStatusNew)
                    queryBuilder
                        .HasOpenedOnOrAfter(startDate)
                        .HasOpenedOnOrBefore(endDate);
                else if (IsStatusClosed)
                    queryBuilder
                        .HasClosedOnOrAfter(startDate)
                        .HasClosedOnOrBefore(endDate);

                //  Get count.
                return await queryBuilder.GetCountAsync();
            }
        }
        #endregion
        #region Transaction Metrics
        /// <summary>
        /// Calculates the total transactions done using a <see cref="Dictionary{string, TransactionBreakdown}">
        /// containing the details of trasaction breakdowns.
        /// </summary>
        /// <param name="transactionBreakdowns"></param>
        /// <returns>An <see cref="int"/> representing the total volume of transactions.</returns>
        public decimal GetTransactionVolume(Dictionary<string, TransactionBreakdown> transactionBreakdowns, decimal totalLoanAmount)
        {
            decimal sum = 0;
            foreach (var kvp in transactionBreakdowns.Where(t => t.Key != TransactionTypes.OUTGOING_TRANSFER).ToDictionary())
            {
                sum += kvp.Value.Total;
            }
            return sum + totalLoanAmount;
        }
        /// <summary>
        /// Calculates the net movement of currency in the bank through a 
        /// <see cref="Dictionary{string, TransactionBreakdown}"> containing the details of transaction
        /// breakdowns.
        /// </summary>
        /// <param name="transactionBreakdowns">The main <see cref="Dictionary{string, TransactionBreakdown}">.</param>
        /// <returns>The calculated <see cref="decimal"/> net movement based on the provided <see cref="List{Transaction}">.
        /// </returns>
        public decimal GetNetMovement(Dictionary<string, TransactionBreakdown> transactionBreakdowns, decimal totalLoanAmount)
        {
            decimal inflow = transactionBreakdowns[TransactionTypes.DEPOSIT].Total + transactionBreakdowns[TransactionTypes.LOAN_PAYMENT].Total;
            decimal outflow = transactionBreakdowns[TransactionTypes.WITHDRAWAL].Total + totalLoanAmount;

            return inflow - outflow;
        }
        /// <summary>
        /// Creates a <see cref="Dictionary{String, TransactionBreakdown}"> of <see cref="TransactionBreakdown"/> with a 
        /// <see cref="string"> specifying the <see cref="TransactionType.TransactionTypeName"> as key and a 
        /// <see cref="TransactionBreakdown"/> object as the value. Each <see cref="TransactionBreakdown"/> object 
        /// contains details including the count of the transaction types, the total amount involved in all transactions 
        /// of the said type, and the average amount involved in transactions of the said type.
        /// </summary>
        /// <param name="startDate">Specifies the earliest date for the query</param>
        /// <param name="endDate">Specifies the latest date for the query.</param>
        /// <returns>A <see cref="Dictionary{String, TransactionBreakdown}"> containing the details of
        /// the transaction breakdowns.</returns>
        public async Task<Dictionary<string, TransactionBreakdown>> GetTransactionBreakdowns(DateTime? startDate = null, DateTime? endDate = null)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var transactionRepo = new TransactionRepository(dbContext);

                //  Exclude incoming transfers.
                var queryBuilder = transactionRepo
                    .Query
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .HasStatusConfirmed();

                //  Filter by start and end dates as needed.
                if (startDate is DateTime sDate)
                    queryBuilder.HasStartDate(sDate);
                if (endDate is DateTime eDate)
                    queryBuilder.HasEndDate(eDate);

                //  Group the transactions by type and cast the result into a dictionary.
                Dictionary<string, TransactionBreakdown> transactionBreakdowns = await queryBuilder
                    .GetQuery()
                    .GroupBy(t => t.TransactionTypeId)
                    .ToDictionaryAsync(
                        g => TransactionTypes.AS_STRING_LIST[g.Key-1],  //  TransactionType string as key.
                        g => new TransactionBreakdown                   //  TransactionBreakdown object as value.
                        {
                            TransactionType = TransactionTypes.AS_STRING_LIST[g.Key-1],
                            Count = g.Count(),
                            Total = g.Sum(t => t.Amount),
                            Average = g.Average(t => t.Amount)
                    });

                //  Initialize the other transaction types in the dictionary except incoming transfer if
                //  no transactions of such type exist in the database.
                foreach(var transactionType in TransactionTypes.AS_STRING_LIST)
                {
                    if (!transactionBreakdowns.ContainsKey(transactionType)
                        && !transactionType.Equals(TransactionTypes.INCOMING_TRANSFER))
                        transactionBreakdowns[transactionType] = new()
                        {
                            TransactionType = transactionType
                        };
                }
                //  TO DO: INCLUDE LOANS BREAKDOWN

                return transactionBreakdowns;
            }    
        }
        /// <summary>
        /// Retrieves a <see cref="List{Transaction}"/> with specified <see cref="int"/> count of 
        /// "/><see cref="Transaction"/> objects that have the largest amount of money involved.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="startDate">Specifies the earliest date for the query</param>
        /// <param name="endDate">Specifies the latest date for the query.</param>
        /// <returns>A <see cref="List{Transaction}"/> containing the <see cref="Transaction"> with the largest 
        /// <see cref="Transaction.Amount"/> of <see cref="int"/> count.</returns>
        public async Task<List<Transaction>> GetLargestTransactions(int count, DateTime? startDate, DateTime? endDate)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var transactionRepo = new TransactionRepository(dbContext);

                var queryBuilder = transactionRepo
                    .Query
                    .IncludeTransactionType()
                    .IncludeMainAccount()
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .HasStatusConfirmed()
                    .OrderByAmountDescending();
                if (startDate is DateTime sDate)
                    queryBuilder.HasStartDate(sDate);
                if (endDate is DateTime eDate)
                    queryBuilder.HasEndDate(eDate);

                List<Transaction> largestTransactions = await queryBuilder
                    .TakeWithCount(count)
                    .GetQuery()
                    .ToListAsync();

                foreach (var transaction in largestTransactions)
                {
                    var accountNumber = transaction.MainAccount.AccountNumber;
                    transaction.MainAccount.AccountNumber = new DataMaskingService().MaskAccountNumber(accountNumber);
                }

                return largestTransactions;
            }
        }
        /// <summary>
        /// Retrieves a <see cref="List{Transaction}"/> filtered by an optional start and end date, and with
        /// optional pagination.
        /// </summary>
        /// <param name="transactionStartDate">The start date for filtering transactions. If null, 
        /// no lower bound is applied.</param>
        /// <param name="transactionEndDate">The end date for filtering transactions. If null, no 
        /// upper bound is applied.</param>
        /// <param name="pageSize">The size of a page.</param>
        /// <param name="pageNumber">The page number of the list in the pagination.</param>
        /// <returns>A reversed <see cref="List{Transaction}"/> with <see cref="Transaction"/> entities 
        /// matching the specified date range.</returns>
        /// <remarks>
        /// Transactions are ordered such that the most recent ones appear first.
        /// </remarks>
        public async Task<List<Transaction>> AdminLoadTransactionsListAsync(
            DateTime? transactionStartDate = null, 
            DateTime? transactionEndDate = null,
            int transactionTypeId = 0,
            int pageSize = 5,
            int pageNumber = 1)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                //  Declare repository dependency.
                var transactionRepo = new TransactionRepository(dbContext);

                //  Compose Query
                var queryBuilder = transactionRepo
                    .Query
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .IncludeMainAccount()
                    .IncludeTransactionType()
                    .OrderByDateAndTimeDescending();

                //  Apply the filters as needed.
                if (transactionStartDate is DateTime startDate)
                    queryBuilder.HasStartDate(startDate);
                if (transactionEndDate is DateTime endDate)
                    queryBuilder.HasEndDate(endDate);
                if (transactionTypeId > 0)
                    queryBuilder.HasTransactionTypeId(transactionTypeId);


                //  Count the number of transactions until the previous page.
                int skipCount = (pageNumber - 1) * pageSize;
                //  Specify the number of transactions in a page.
                int takeCount = pageSize;
                queryBuilder
                    .SkipBy(skipCount)
                    .TakeWithCount(takeCount);

                //  Get the transaction list.
                List<Transaction> transactions = await queryBuilder
                    .GetQuery()
                    .ToListAsync();

                //  Mask the account numbers.
                foreach (var transaction in transactions)
                {
                    string accountNumber = transaction.MainAccount.AccountNumber;
                    transaction.MainAccount.AccountNumber = _dataMaskingService.MaskAccountNumber(accountNumber);
                }

                return transactions;
            }
        }
        public async Task<int> AdminCountRemainingTransactionsAsync(
            DateTime? transactionStartDate = null,
            DateTime? transactionEndDate = null,
            int transactionTypeId = 0,
            int pageSize = 5,
            int pageNumber = 1)

        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                //  Declare repository dependency.
                var transactionRepo = new TransactionRepository(dbContext);

                //  Compose Query
                var queryBuilder = transactionRepo
                    .Query
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .IncludeMainAccount()
                    .IncludeTransactionType()
                    .OrderByDateAndTimeDescending();

                //  Apply the filters as needed.
                if (transactionStartDate is DateTime startDate)
                    queryBuilder.HasStartDate(startDate);
                if (transactionEndDate is DateTime endDate)
                    queryBuilder.HasEndDate(endDate);
                if (transactionTypeId > 0)
                    queryBuilder.HasTransactionTypeId(transactionTypeId);


                //  Count the number of transactions from the first page until the current page.
                int skipCount = pageNumber * pageSize;
                //  Return the count of the transactions from the current until the last page.
                return await queryBuilder
                    .GetQuery()
                    .Skip(skipCount)
                    .CountAsync();
            }
        }
        /// <summary>
        /// Gets the total count of <see cref="Loan"/> entities in the databse. between a given timeframe.
        /// </summary>
        /// <param name="startDate">The earliest <see cref="Loan.ApplicationDate"/> value.</param>
        /// <param name="endDate">The latest. <see cref="Loan.ApplicationDate"/> value.</param>
        /// <returns></returns>
        public async Task<int> GetLoanCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var loanRepo = new LoanRepository(dbContext);

                var queryBuilder = loanRepo
                    .Query
                    .OrderByDateDescending()
                    .HasPostDisbursementStatus();

                if (startDate is DateTime sDate)
                    queryBuilder.LoanStartsOnOrAfter(sDate);
                if (endDate is DateTime eDate)
                    queryBuilder.LoanStartsOnOrBefore(eDate);

                return await queryBuilder.GetCountAsync();
            }
        }
        /// <summary>
        /// Gets the total sum of <see cref="Loan.RemainingLoanBalance"/> in the databse. between a given timeframe.
        /// </summary>
        /// <param name="startDate">The earliest <see cref="Loan.ApplicationDate"/> value.</param>
        /// <param name="endDate">The latest. <see cref="Loan.ApplicationDate"/> value.</param>
        /// <returns></returns>
        public async Task<decimal> GetLoanAmountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var loanRepo = new LoanRepository(dbContext);

                var queryBuilder = loanRepo
                    .Query
                    .OrderByDateDescending()
                    .HasPostDisbursementStatus();

                if (startDate is DateTime sDate)
                    queryBuilder.LoanStartsOnOrAfter(sDate);
                if (endDate is DateTime eDate)
                    queryBuilder.LoanStartsOnOrBefore(eDate);

                return await queryBuilder
                    .GetQuery()
                    .Select(l => l.RemainingLoanBalance)
                    .SumAsync();
            }
        }
        #endregion
        #region Chart Data
        #region Net Movement Data By Time Filter
        /// <summary>
        /// Asynchronously retrieves transaction chart data for all predefined time filter modes
        /// (e.g., hourly, daily, weekly, monthly, yearly), excluding the current day,
        /// and stores the result in a dictionary keyed by the time filter.
        /// This method is used for grabbing non-real-time data, prioritizing querying performance by
        /// bulk loading all data in one dictionary.
        /// </summary>
        /// <param name="currentDate">The current date used as a reference point for time-based filters. 
        /// Today's data is excluded.</param>
        /// <returns>
        /// A dictionary where the key is a string representing the time filter mode,
        /// and the value is a list of <see cref="ChartData"/> representing the transaction totals for 
        /// that time frame.
        /// </returns>
        public async Task<Dictionary<string, List<ChartData>>> GetNetMovementDictionary(DateTime currentDate, int accountId = 0)
        {
            Dictionary<string, List<ChartData>> chartCache = new();
            foreach(var filter in AdminDashboardTimeFilters.AS_STRING_LIST)
                chartCache[filter] = await GetNetMovementByTimeFilter(filter, currentDate.AddDays(-1), accountId);
            return chartCache;
        }
        /// <summary>
        /// Asynchronously retrieves transaction chart data for a specific time filter mode
        /// (e.g., hourly, daily, weekly, monthly, yearly) using the given reference date.
        /// </summary>
        /// <param name="filterMode">The selected time filter mode for the chart (e.g., "Hourly", "Daily").</param>
        /// <param name="currentDate">The current date used to calculate the relevant date range for the selected time 
        /// filter.</param>
        /// <returns>
        /// A list of <see cref="ChartData"/> representing the total transaction amounts grouped by the specified time interval.
        /// If the filter mode is unrecognized, an empty list is returned.
        /// </returns>
        public async Task<List<ChartData>> GetNetMovementByTimeFilter(string filterMode, DateTime currentDate, int accountId = 0)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var transactionRepo = new TransactionRepository(dbContext);
                var loanRepo = new LoanRepository(dbContext);

                var transactionQuery = BuildTransactionQuery(transactionRepo, filterMode, currentDate, accountId);
                var loanQuery = BuildLoanQuery(loanRepo, filterMode, currentDate, accountId);

                bool isAccountNetMovement = accountId > 0 ? true : false;

                return filterMode switch
                {
                    AdminDashboardTimeFilters.HOURLY => await GetHourlyNetMovement(transactionQuery, loanQuery, currentDate, isAccountNetMovement),
                    AdminDashboardTimeFilters.DAILY => await GetDailyNetMovement(transactionQuery, loanQuery, currentDate, isAccountNetMovement),
                    AdminDashboardTimeFilters.WEEKLY => await GetWeeklyNetMovement(transactionQuery, loanQuery, currentDate, isAccountNetMovement),
                    AdminDashboardTimeFilters.MONTHLY => await GetMonthlyNetMovement(transactionQuery, loanQuery, currentDate, isAccountNetMovement),
                    AdminDashboardTimeFilters.YEARLY => await GetYearlyNetMovement(transactionQuery, loanQuery, currentDate, isAccountNetMovement),
                    _ => new List<ChartData>()
                };
            }
        }
        /// <summary>
        /// Asynchronously retrieves transaction data grouped by hour for the specified date.
        /// Excludes transactions with the type <c>Incoming_Transfer</c>.
        /// Fills in missing hours with zero values up to the current hour.
        /// </summary>
        /// <param name="currentDate">The date for which hourly transaction data will be retrieved.</param>
        /// <returns>
        /// A list of <see cref="ChartData"/> where each item represents a specific hour and the total transaction 
        /// amount for that hour.
        /// </returns>
        private async Task<List<ChartData>> GetHourlyNetMovement(IQueryable<Transaction> transactionQuery, IQueryable<Loan> loanQuery, DateTime currentDate, bool isAccountNetMovement = false)
        {
            List<ChartData> transactionChartData = await transactionQuery
                    .GroupBy(t => t.TransactionTime.Hours)
                    .OrderBy(transactionGroup => transactionGroup.Key)
                    .Select(transactionGroup => new ChartData
                    {
                        Label = new DateTime(1, 1, 1, transactionGroup.Key, 0, 0).ToString("hh tt"),
                        Value = isAccountNetMovement ?
                            transactionGroup.Sum(t =>
                                t.TransactionTypeId == (int)TransactionTypeIDs.Deposit ||
                                t.TransactionTypeId == (int)TransactionTypeIDs.Incoming_Transfer ? t.Amount :
                                t.TransactionTypeId == (int)TransactionTypeIDs.Withdrawal ||
                                t.TransactionTypeId == (int)TransactionTypeIDs.Outgoing_Transfer ? -t.Amount :
                                0
                            ) : 
                            transactionGroup.Sum(t =>
                                t.TransactionTypeId == (int)TransactionTypeIDs.Deposit ? t.Amount :
                                t.TransactionTypeId == (int)TransactionTypeIDs.Withdrawal ? -t.Amount :
                                0
                            )
                    }).ToListAsync();
            //  filler
            transactionChartData = FillHourlyData(transactionChartData, currentDate);

            //  get loan chart data
            var loanChartData = await GetHourlyLoaned(loanQuery, currentDate);

            //  Left Join transaction chart with loan chart
            transactionChartData = transactionChartData
                .GroupJoin(
                    loanChartData,
                    mainChart => mainChart.Label,
                    loanChart => loanChart.Label,
                    (mainChart, matchingLoans) => new ChartData
                    {
                        Label = mainChart.Label,
                        Value = isAccountNetMovement ?
                            mainChart.Value + matchingLoans.Sum(l => l.Value) :
                            mainChart.Value - matchingLoans.Sum(l => l.Value)
                    }
                ).ToList();

            return transactionChartData;
        }
        /// <summary>
        /// Asynchronously retrieves transaction data grouped by day for the past 7 days up to the specified date.
        /// Excludes transactions with the type <c>Incoming_Transfer</c>.
        /// Fills in missing days with zero values.
        /// </summary>
        /// <param name="currentDate">The current date used to determine the 7-day range (excluding today).</param>
        /// <returns>
        /// A list of <see cref="ChartData"/> where each item represents a date label and the total transaction amount for 
        /// that day.
        /// </returns>
        private async Task<List<ChartData>> GetDailyNetMovement(IQueryable<Transaction> transactionQuery, IQueryable<Loan> loanQuery, DateTime currentDate, bool isAccountNetMovement = false)
        {
            List<ChartData> transactionChartData = await transactionQuery
                .GroupBy(t => t.TransactionDate.Date)
                .OrderBy(transactionGroup => transactionGroup.Key)
                .Select(transactionGroup => new ChartData
                {
                    Label = transactionGroup.Key.ToString("MMM dd"),
                    Value = isAccountNetMovement ?
                            transactionGroup.Sum(t =>
                                t.TransactionTypeId == (int)TransactionTypeIDs.Deposit ||
                                t.TransactionTypeId == (int)TransactionTypeIDs.Incoming_Transfer ? t.Amount :
                                t.TransactionTypeId == (int)TransactionTypeIDs.Withdrawal ||
                                t.TransactionTypeId == (int)TransactionTypeIDs.Outgoing_Transfer ? -t.Amount :
                                0
                            ) :
                            transactionGroup.Sum(t =>
                                t.TransactionTypeId == (int)TransactionTypeIDs.Deposit ? t.Amount :
                                t.TransactionTypeId == (int)TransactionTypeIDs.Withdrawal ? -t.Amount :
                                0
                            )
                }).ToListAsync();

            //  fillers
            transactionChartData = FillDailyData(transactionChartData, currentDate);

            //  Get loan chart
            var loanChartData = await GetDailyLoaned(loanQuery, currentDate);

            //  Left join transaction chart with loan chart.
            transactionChartData = transactionChartData
                .GroupJoin(
                    loanChartData,
                    mainChart => mainChart.Label,
                    loanChart => loanChart.Label,
                    (mainChart, matchingLoans) => new ChartData
                    {
                        Label = mainChart.Label,
                        Value = isAccountNetMovement ?
                            mainChart.Value + matchingLoans.Sum(l => l.Value) :
                            mainChart.Value - matchingLoans.Sum(l => l.Value)
                    }
                ).ToList();

            return transactionChartData;
        }
        /// <summary>
        /// Asynchronously retrieves transaction data grouped into 4 weekly segments from the past 28 days up to 
        /// the specified date.
        /// Excludes transactions with the type <c>Incoming_Transfer</c>.
        /// Fills in missing days and aggregates totals by week.
        /// </summary>
        /// <param name="currentDate">The current date used to determine the 4-week range (excluding today).</param>
        /// <returns>
        /// A list of <see cref="ChartData"/> where each item represents a week label (e.g., "Week 1") and the total 
        /// transaction amount for that week.
        /// </returns>
        private async Task<List<ChartData>> GetWeeklyNetMovement(IQueryable<Transaction> transactionQuery, IQueryable<Loan> loanQuery, DateTime currentDate, bool isAccountNetMovement = false)
        {
            //  Group the transactions by day and cast the result into a temporary object.
            var result = await transactionQuery
                .GroupBy(t => t.TransactionDate.Date)
                .OrderBy(transactionGroup => transactionGroup.Key)
                .Select(transactionGroup => new TempChartData
                {
                    Label = transactionGroup.Key,
                    Value = isAccountNetMovement ?
                            transactionGroup.Sum(t =>
                                t.TransactionTypeId == (int)TransactionTypeIDs.Deposit ||
                                t.TransactionTypeId == (int)TransactionTypeIDs.Incoming_Transfer ? t.Amount :
                                t.TransactionTypeId == (int)TransactionTypeIDs.Withdrawal ||
                                t.TransactionTypeId == (int)TransactionTypeIDs.Outgoing_Transfer ? -t.Amount :
                                0
                            ) :
                            transactionGroup.Sum(t =>
                                t.TransactionTypeId == (int)TransactionTypeIDs.Deposit ? t.Amount :
                                t.TransactionTypeId == (int)TransactionTypeIDs.Withdrawal ? -t.Amount :
                                0
                            )
                }).ToListAsync();

            //  Filler
            result = FillWeeklyData(result, currentDate);

            //  Cast the result into the LineChartData object indexed by week.
            List<ChartData> transactionChartData = new();
            int weekindex = 1;
            for (int i = 0; i < result.Count; i += 7)
            {
                string newLabel = $"Week {weekindex++}";
                int take = Math.Min(7, result.Count - i);
                decimal value = result.Skip(i).Take(take).Sum(cd => cd.Value);
                transactionChartData.Add(new ChartData
                {
                    Label = newLabel,
                    Value = value
                });
            }

            //  Get Loan Chart Data
            var loanChartData = await GetWeeklyLoaned(loanQuery, currentDate);

            //  Left join transaction chart with loan chart.
            transactionChartData = transactionChartData
                .GroupJoin(
                    loanChartData,
                    mainChart => mainChart.Label,
                    loanChart => loanChart.Label,
                    (mainChart, matchingLoans) => new ChartData
                    {
                        Label = mainChart.Label,
                        Value = isAccountNetMovement ?
                            mainChart.Value + matchingLoans.Sum(l => l.Value) :
                            mainChart.Value - matchingLoans.Sum(l => l.Value)
                    }
                ).ToList();

            return transactionChartData;
        }
        /// <summary>
        /// Asynchronously retrieves transaction data grouped by month from the start of the current year up to the 
        /// specified date.
        /// Excludes transactions with the type <c>Incoming_Transfer</c>.
        /// Fills in missing months with zero values.
        /// </summary>
        /// <param name="currentDate">The current date used to determine the year-to-date range.</param>
        /// <returns>
        /// A list of <see cref="ChartData"/> where each item represents a month label (e.g., "Jan") and the total 
        /// transaction amount for that month.
        /// </returns>
        private async Task<List<ChartData>> GetMonthlyNetMovement(IQueryable<Transaction> transactionQuery, IQueryable<Loan> loanQuery, DateTime currentDate, bool isAccountNetMovement = false)
        {
            List<ChartData> transactionChartData = await transactionQuery
                .GroupBy(t => t.TransactionDate.Month)
                .OrderBy(transactionGroup => transactionGroup.Key)
                .Select(transactionGroup => new ChartData
                {
                    Label = new DateTime(1, transactionGroup.Key, 1).ToString("MMM"),
                    Value = isAccountNetMovement ?
                            transactionGroup.Sum(t =>
                                t.TransactionTypeId == (int)TransactionTypeIDs.Deposit ||
                                t.TransactionTypeId == (int)TransactionTypeIDs.Incoming_Transfer ? t.Amount :
                                t.TransactionTypeId == (int)TransactionTypeIDs.Withdrawal ||
                                t.TransactionTypeId == (int)TransactionTypeIDs.Outgoing_Transfer ? -t.Amount :
                                0
                            ) :
                            transactionGroup.Sum(t =>
                                t.TransactionTypeId == (int)TransactionTypeIDs.Deposit ? t.Amount :
                                t.TransactionTypeId == (int)TransactionTypeIDs.Withdrawal ? -t.Amount :
                                0
                            )
                }).ToListAsync();
            //  Filler
            transactionChartData = FillMonthlyData(transactionChartData, currentDate);

            //  Get loan chart data.
            var loanChartData = await GetMonthlyLoaned(loanQuery, currentDate);

            //  Left join transaction chart data with loan chart data.
            transactionChartData = transactionChartData
                .GroupJoin(
                    loanChartData,
                    mainChart => mainChart.Label,
                    loanChart => loanChart.Label,
                    (mainChart, matchingLoans) => new ChartData
                    {
                        Label = mainChart.Label,
                        Value = mainChart.Value - matchingLoans.Sum(l => l.Value)
                    }
                ).ToList();

            return transactionChartData;
        }
        /// <summary>
        /// Asynchronously retrieves transaction data grouped by year over the past 10 years up to the specified date.
        /// Excludes transactions with the type <c>Incoming_Transfer</c>.
        /// Fills in missing years with zero values.
        /// </summary>
        /// <param name="currentDate">The current date used to determine the 10-year range.</param>
        /// <returns>
        /// A list of <see cref="ChartData"/> where each item represents a year label and the total transaction amount 
        /// for that year.
        /// </returns>
        private async Task<List<ChartData>> GetYearlyNetMovement(IQueryable<Transaction> transactionQuery, IQueryable<Loan> loanQuery, DateTime currentDate, bool isAccountNetMovement = false)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                TransactionRepository transactionRepo = new TransactionRepository(dbContext);
                var queryBuilder = transactionRepo.Query;
                DateTime yearlyModeStartDate = currentDate.AddYears(-10);

                List<ChartData> transactionChartData = await transactionQuery
                    .GroupBy(t => t.TransactionDate.Year)
                    .OrderBy(transactionGroup => transactionGroup.Key)
                    .Select(transactionGroup => new ChartData
                    {
                        Label = transactionGroup.Key.ToString(),
                        Value = isAccountNetMovement ?
                            transactionGroup.Sum(t =>
                                t.TransactionTypeId == (int)TransactionTypeIDs.Deposit ||
                                t.TransactionTypeId == (int)TransactionTypeIDs.Incoming_Transfer ? t.Amount :
                                t.TransactionTypeId == (int)TransactionTypeIDs.Withdrawal ||
                                t.TransactionTypeId == (int)TransactionTypeIDs.Outgoing_Transfer ? -t.Amount :
                                0
                            ) :
                            transactionGroup.Sum(t =>
                                t.TransactionTypeId == (int)TransactionTypeIDs.Deposit ? t.Amount :
                                t.TransactionTypeId == (int)TransactionTypeIDs.Withdrawal ? -t.Amount :
                                0
                            )
                    }).ToListAsync();

                //  Filler
                transactionChartData = FillYearlyData(transactionChartData, currentDate);

                //  Get loan chart data.
                var loanChartData = await GetYearlyLoaned(loanQuery, currentDate);

                //  Left join transaction chart data with loan chart data.
                transactionChartData = transactionChartData
                    .GroupJoin(
                        loanChartData,
                        mainChart => mainChart.Label,
                        loanChart => loanChart.Label,
                        (mainChart, matchingLoans) => new ChartData
                        {
                            Label = mainChart.Label,
                            Value = mainChart.Value - matchingLoans.Sum(l => l.Value)
                        }
                    ).ToList();

                return transactionChartData;
            }
        }
        #endregion
        #region Loan Data By Time Filter
        private async Task<List<ChartData>> GetHourlyLoaned(IQueryable<Loan> query, DateTime currentDate)
        {
            var loanChartData = await query
                .GroupBy(l => l.StartDate!.Value.Hour)
                .OrderBy(loanGroup => loanGroup.Key)
                .Select(loanGroup => new ChartData
                {
                    Label = new DateTime(1, 1, 1, loanGroup.Key, 0, 0).ToString("hh tt"),
                    Value = loanGroup.Sum(loan => loan.LoanAmount)
                }).ToListAsync();

            //  Filler
            loanChartData = FillHourlyData(loanChartData, currentDate);

            return loanChartData;
        }
        private async Task<List<ChartData>> GetDailyLoaned(IQueryable<Loan> query, DateTime currentDate)
        {
            var loanChartData = await query
                .GroupBy(l => l.StartDate!.Value.Date)
                .OrderBy(loanGroup => loanGroup.Key)
                .Select(loanGroup => new ChartData
                {
                    Label = loanGroup.Key.ToString("MMM dd"),
                    Value = loanGroup.Sum(loan => loan.LoanAmount)
                }).ToListAsync();

            //  Filler
            loanChartData = FillDailyData(loanChartData, currentDate);

            return loanChartData;
        }
        private async Task<List<ChartData>> GetWeeklyLoaned(IQueryable<Loan> query, DateTime currentDate)
        {
            var result = await query
                .GroupBy(l => l.StartDate!.Value.Date)
                .OrderBy(loanGroup => loanGroup.Key)
                .Select(loanGroup => new TempChartData
                {
                    Label = loanGroup.Key,
                    Value = loanGroup.Sum(loan => loan.LoanAmount)
                }).ToListAsync();
            //  Initialize fillers for days where no loans took place.
            result = FillWeeklyData(result, currentDate);

            //  Cast the result into the LineChartData object indexed by week.
            List<ChartData> chartData = new();
            int weekindex = 1;
            for (int i = 0; i < result.Count; i += 7)
            {
                string newLabel = $"Week {weekindex++}";
                int take = Math.Min(7, result.Count - i);
                decimal value = result.Skip(i).Take(take).Sum(cd => cd.Value);
                chartData.Add(new ChartData
                {
                    Label = newLabel,
                    Value = value
                });
            }

            return chartData;
        }
        private async Task<List<ChartData>> GetMonthlyLoaned(IQueryable<Loan> query, DateTime currentDate)
        {
            var loanChartData = await query
                .GroupBy(l => l.StartDate!.Value.Month)
                .OrderBy(loanGroup => loanGroup.Key)
                .Select(loanGroup => new ChartData
                {
                    Label = new DateTime(1, loanGroup.Key, 1).ToString("MMM"),
                    Value = loanGroup.Sum(loan => loan.LoanAmount)
                }).ToListAsync();

            //  Filler
            loanChartData = FillMonthlyData(loanChartData, currentDate);

            return loanChartData;
        }
        private async Task<List<ChartData>> GetYearlyLoaned(IQueryable<Loan> query, DateTime currentDate)
        {
            var loanChartData = await query
                .GroupBy(l => l.StartDate!.Value.Year)
                .OrderBy(loanGroup => loanGroup.Key)
                .Select(loanGroup => new ChartData
                {
                    Label = loanGroup.Key.ToString(),
                    Value = loanGroup.Sum(loan => loan.LoanAmount)
                }).ToListAsync();

            //  Filler
            loanChartData = FillYearlyData(loanChartData, currentDate);

            return loanChartData;
        }
        private IQueryable<Loan> BuildLoanQuery(
            LoanRepository loanRepo,
            string filterMode,
            DateTime currentDate,
            int accountId = 0)
        {
            var queryBuilder = loanRepo
                .Query
                .LoanStartsOnOrAfter(filterMode switch
                {
                    AdminDashboardTimeFilters.HOURLY => currentDate.Date,
                    AdminDashboardTimeFilters.DAILY => currentDate.AddDays(-7),
                    AdminDashboardTimeFilters.WEEKLY => currentDate.AddDays(-28),
                    AdminDashboardTimeFilters.MONTHLY => currentDate.AddMonths(-currentDate.Month + 1),
                    AdminDashboardTimeFilters.YEARLY => currentDate.AddYears(-10),
                    _ => currentDate.Date
                })
                .HasPostDisbursementStatus();
            if (accountId > 0)
                queryBuilder.HasAccountId(accountId);

            return queryBuilder.GetQuery();
        }
        #endregion
        #region Recent Transaction Type Volumes (Pie Chart)
        /// <summary>
        /// Retrieves pie chart data for all time filter modes (hourly, daily, weekly, monthly, yearly),
        /// relative to the specified current date.
        /// </summary>
        /// <param name="currentDate">The base date used to calculate each time filter's start date.</param>
        /// <returns>A dictionary where each key is a time filter label and the value is the pie chart data for that filter.</returns>
        public async Task<Dictionary<string, List<ChartData>>> GetTransactionTypeCountsDictionary(DateTime currentDate, params string[] exceptStatus)
        {
            Dictionary<string, List<ChartData>> chartCache = new();
            foreach (var filter in AdminDashboardTimeFilters.AS_STRING_LIST)
                chartCache[filter] = await GetRecentTransactionTypeCounts(filter, currentDate.AddDays(-1), exceptStatus);
            return chartCache;
        }
        /// <summary>
        /// Retrieves pie chart data based on the specified time filter mode.
        /// </summary>
        /// <param name="filterMode">The time filter mode (e.g., hourly, daily, weekly).</param>
        /// <param name="currentDate">The current date used to calculate the range of the filter.</param>
        /// <returns>A list of ChartData objects grouped by transaction type.</returns>
        public async Task<List<ChartData>> GetRecentTransactionTypeCounts(string filterMode, DateTime currentDate, params string[] exceptStatus)
        {
            return filterMode switch
            {
                AdminDashboardTimeFilters.HOURLY => await GetLastHourTransactionTypeCounts(currentDate, exceptStatus),
                AdminDashboardTimeFilters.DAILY => await GetLastDayTransactionTypeCounts(currentDate, exceptStatus),
                AdminDashboardTimeFilters.WEEKLY => await GetLastWeekTransactionTypeCounts(currentDate, exceptStatus),
                AdminDashboardTimeFilters.MONTHLY => await GetLastMonthTransactionTypeCounts(currentDate, exceptStatus),
                AdminDashboardTimeFilters.YEARLY => await GetLastYearTransactionTypeCounts(currentDate, exceptStatus),
                _ => new List<ChartData>()
            };
        }
        /// <summary>
        /// Retrieves pie chart data for the last hour relative to the specified date.
        /// </summary>
        /// <param name="currentDate">The current date and time.</param>
        /// <returns>A list of ChartData for the past hour.</returns>
        private async Task<List<ChartData>> GetLastHourTransactionTypeCounts(DateTime currentDate, params string[] exceptStatus) =>
            await GetPieChartDataByDateAndTime(currentDate, currentDate.AddHours(-1).TimeOfDay, exceptStatus: exceptStatus);
        /// <summary>
        /// Retrieves pie chart data for the previous day.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>A list of ChartData for the current day.</returns>
        private async Task<List<ChartData>> GetLastDayTransactionTypeCounts(DateTime currentDate, params string[] exceptStatus) =>
            await GetPieChartDataByDateAndTime(currentDate, exceptStatus: exceptStatus);
        /// <summary>
        /// Retrieves pie chart data for the past 7 days.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>A list of ChartData for the past week.</returns>
        private async Task<List<ChartData>> GetLastWeekTransactionTypeCounts(DateTime currentDate, params string[] exceptStatus) =>
            await GetPieChartDataByDateAndTime(currentDate.AddDays(-7), exceptStatus: exceptStatus);
        /// <summary>
        /// Retrieves pie chart data for the past month (approximated using DateTime.AddMonths).
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>A list of ChartData for the past month.</returns>
        private async Task<List<ChartData>> GetLastMonthTransactionTypeCounts(DateTime currentDate, params string[] exceptStatus) =>
            await GetPieChartDataByDateAndTime(currentDate.AddMonths(-1), exceptStatus: exceptStatus);
        /// <summary>
        /// Retrieves pie chart data for the past year (approximated using DateTime.AddYears).
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>A list of ChartData for the past year.</returns>
        private async Task<List<ChartData>> GetLastYearTransactionTypeCounts(DateTime currentDate, params string[] exceptStatus) =>
            await GetPieChartDataByDateAndTime(currentDate.AddYears(-1), exceptStatus: exceptStatus);
        /// <summary>
        /// Retrieves pie chart data starting from a given date and optionally a specific time of day.
        /// Filters out transactions of a specific type (Incoming Transfer).
        /// </summary>
        /// <param name="startDate">The starting date for the data range.</param>
        /// <param name="startTime">Optional: the starting time on the given date to begin filtering.</param>
        /// <returns>A list of ChartData grouped by transaction type.</returns>
        private async Task<List<ChartData>> GetPieChartDataByDateAndTime(DateTime startDate, TimeSpan? startTime = null, params string[] exceptStatus)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var transactionRepo = new TransactionRepository(dbContext);

                var queryBuilder = transactionRepo.Query;

                if (startTime is not null)
                    queryBuilder.HasStartTime(startTime.Value);

                foreach (var status in exceptStatus)
                    queryBuilder.ExceptStatus(status);

                return await queryBuilder
                    .HasStartDate(startDate.Date)
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .GetQuery()
                    .GroupBy(t => t.TransactionTypeId)
                    .Select(g => new ChartData
                    {
                        Label = TransactionTypes.AS_STRING_LIST[g.Key - 1],
                        Value = g.Count()
                    }).ToListAsync();
            }
        }
        #endregion
        #region Transaction Volume By Time Filter
        /// <summary>
        /// Retrieves bar chart data for all predefined time filters (hourly, daily, weekly, monthly, yearly).
        /// </summary>
        /// <param name="currentDate">The reference date used to calculate the time ranges.</param>
        /// <returns>A dictionary mapping each time filter to its corresponding list of chart data.</returns>
        public async Task<Dictionary<string, List<ChartData>>> GetTransactionsVolumeDictionary(DateTime currentDate, int acccountId = 0)
        {
            Dictionary<string, List<ChartData>> chartCache = new();
            foreach (var filter in AdminDashboardTimeFilters.AS_STRING_LIST)
                chartCache[filter] = await GetTransactionsVolumeByTimeFilter(filter, currentDate.AddDays(-1));
            return chartCache;
        }
        /// <summary>
        /// Retrieves bar chart data based on the specified time filter.
        /// </summary>
        /// <param name="filterMode">The time filter mode (e.g., HOURLY, DAILY, etc.).</param>
        /// <param name="currentDate">The reference date used to calculate the time range.</param>
        /// <returns>A list of chart data for the specified filter.</returns>
        public async Task<List<ChartData>> GetTransactionsVolumeByTimeFilter(string filterMode, DateTime currentDate, int accountId = 0)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                TransactionRepository transactionRepo = new TransactionRepository(dbContext);

                var query = BuildTransactionQuery(transactionRepo, filterMode, currentDate, accountId);

                return filterMode switch
                {
                    AdminDashboardTimeFilters.HOURLY => await GetHourlyTransactionsVolume(query, currentDate),
                    AdminDashboardTimeFilters.DAILY => await GetDailyTransactionsVolume(query, currentDate),
                    AdminDashboardTimeFilters.WEEKLY => await GetWeeklyTransactionsVolume(query, currentDate),
                    AdminDashboardTimeFilters.MONTHLY => await GetMonthlyTransactionsVolume(query, currentDate),
                    AdminDashboardTimeFilters.YEARLY => await GetYearlyTransactionsVolume(query, currentDate),
                    _ => new()
                };

            }
        }
        /// <summary>
        /// Retrieves hourly bar chart data representing the transaction volume per hour from the start of the day to the current hour.
        /// </summary>
        /// <param name="currentDate">The reference date and time.</param>
        /// <returns>A list of chart data with hourly labels and counts.</returns>
        private async Task<List<ChartData>> GetHourlyTransactionsVolume(IQueryable<Transaction> query, DateTime currentDate)
        {
            List<ChartData> chartData = await query
                    .GroupBy(t => t.TransactionTime.Hours)
                    .OrderBy(g => g.Key)
                    .Select(subgroup => new ChartData
                    {
                        Label = new DateTime(1, 1, 1, subgroup.Key, 0, 0).ToString("hh tt"),
                        DataUnits = subgroup
                            .GroupBy(t => t.TransactionTypeId)
                            .Select(subgroup => new DataUnit
                            {
                                Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                                Value = subgroup.Sum(t => t.Amount)
                            }).ToList()
                    }).ToListAsync();
            chartData = chartData
                .Select(cd => new ChartData
                {
                    Label = cd.Label,
                    DataUnits = ChartDataConstants
                        .ALL_TRANSACTION_TYPES
                        .GroupJoin(
                            cd.DataUnits,
                            type => type.Label,
                            dataUnit => dataUnit.Label,
                            (type, matchingDataUnits) => new DataUnit
                            {
                                Label = type.Label,
                                Value = matchingDataUnits.FirstOrDefault()?.Value ?? 0
                            }
                        ).ToList()
                }).ToList();

            int currentHour = currentDate.Hour;
            for (int i = 0; i <= currentHour; i++)
            {
                string newLabel = new DateTime(1, 1, 1, i, 0, 0).ToString("hh tt");
                if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                    chartData.Insert(i, new()
                    {
                        Label = newLabel,
                        DataUnits = ChartDataConstants.ALL_TRANSACTION_TYPES
                    });
            }

            return chartData;
        }
        /// <summary>
        /// Retrieves daily bar chart data representing transaction counts for each of the past 7 days.
        /// </summary>
        /// <param name="currentDate">The reference date.</param>
        /// <returns>A list of chart data with daily labels (e.g., "Apr 09") and counts.</returns>
        private async Task<List<ChartData>> GetDailyTransactionsVolume(IQueryable<Transaction> query, DateTime currentDate)
        {
            DateTime dailyModeStartDate = currentDate.AddDays(-7);

            List<ChartData> chartData = await query
                .GroupBy(t => t.TransactionDate.Date)
                .OrderBy(g => g.Key)
                .Select(subgroup => new ChartData
                {
                    Label = subgroup.Key.ToString("MMM dd"),
                    DataUnits = subgroup
                        .GroupBy(t => t.TransactionTypeId)
                        .Select(subgroup => new DataUnit
                        {
                            Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                            Value = subgroup.Sum(t => t.Amount)
                        }).ToList()
                }).ToListAsync();
            chartData = chartData
                .Select(cd => new ChartData
                {
                    Label = cd.Label,
                    DataUnits = ChartDataConstants
                        .ALL_TRANSACTION_TYPES
                        .GroupJoin(
                            cd.DataUnits,
                            type => type.Label,
                            dataUnit => dataUnit.Label,
                            (type, matchingDataUnits) => new DataUnit
                            {
                                Label = type.Label,
                                Value = matchingDataUnits.FirstOrDefault()?.Value ?? 0
                            }
                        ).ToList()
                }).ToList();
            for (int dayIterator = 0; dailyModeStartDate.AddDays(dayIterator) <= currentDate; dayIterator++)
            {
                string newLabel = dailyModeStartDate.AddDays(dayIterator).ToString("MMM dd");
                if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                    chartData.Insert(dayIterator, new()
                    {
                        Label = newLabel,
                        DataUnits = ChartDataConstants.ALL_TRANSACTION_TYPES
                    });
            }

            return chartData;
        }
        /// <summary>
        /// Retrieves weekly bar chart data, aggregating transaction counts into four weekly groups over the past 28 days.
        /// </summary>
        /// <param name="currentDate">The reference date.</param>
        /// <returns>A list of chart data with "Week 1", "Week 2", etc., as labels and total weekly counts.</returns>
        private async Task<List<ChartData>> GetWeeklyTransactionsVolume(IQueryable<Transaction> query, DateTime currentDate)
        {
            //  Get the start date of the weekly filter mode.
            DateTime weeklyModeStartDate = currentDate.AddDays(-28);
            //  Group the transactions by day and cast the result into a temporary object.
            var result = await query
                .GroupBy(t => t.TransactionDate.Date)
                .OrderBy(g => g.Key)
                .Select(subgroup => new
                {
                    Label = subgroup.Key,
                    DataUnits = subgroup
                        .GroupBy(t => t.TransactionTypeId)
                        .Select(subgroup => new DataUnit
                        {
                            Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                            Value = subgroup.Sum(t => t.Amount)
                        }).ToList()
                }).ToListAsync();
            result = result
                .Select(cd => new
                {
                    Label = cd.Label,
                    DataUnits = ChartDataConstants
                        .ALL_TRANSACTION_TYPES
                        .GroupJoin(
                            cd.DataUnits,
                            type => type.Label,
                            dataUnit => dataUnit.Label,
                            (type, matchingDataUnits) => new DataUnit
                            {
                                Label = type.Label,
                                Value = matchingDataUnits.FirstOrDefault()?.Value ?? 0
                            }
                        ).ToList()
                }).ToList();

            //  Initialize fillers for days where no transactions took place.
            for (int dayIterator = 0; weeklyModeStartDate.AddDays(dayIterator) <= currentDate; dayIterator++)
            {
                DateTime newLabel = weeklyModeStartDate.AddDays(dayIterator);
                if (!result.Any(t => t.Label == newLabel))
                    result.Insert(dayIterator, new
                    {
                        Label = newLabel,
                        DataUnits = ChartDataConstants.ALL_TRANSACTION_TYPES
                    });
            }

            //  Cast the result into the LineChartData object indexed by week.
            List<ChartData> chartData = new();
            int weekindex = 1;
            for (int i = 0; i < result.Count; i += 7)
            {
                string newLabel = $"Week {weekindex++}";
                int take = Math.Min(7, result.Count - i);
                var dailyDataInWeek = result
                    .Skip(i)
                    .Take(take)
                    .ToList();
                var unitSummations = Enumerable.Range(0, dailyDataInWeek[0].DataUnits.Count)
                    .Select(col => new DataUnit
                    {
                        Label = dailyDataInWeek[0].DataUnits[col].Label,
                        Value = dailyDataInWeek.Sum(day => day.DataUnits[col].Value)
                    }).ToList();
                chartData.Add(new ChartData
                {
                    Label = newLabel,
                    DataUnits = unitSummations
                });
            }

            return chartData;
        }
        /// <summary>
        /// Retrieves monthly bar chart data for the current year, showing transaction counts per month.
        /// </summary>
        /// <param name="currentDate">The reference date used to determine the current year.</param>
        /// <returns>A list of chart data with monthly labels (e.g., "Jan", "Feb") and counts.</returns>
        private async Task<List<ChartData>> GetMonthlyTransactionsVolume(IQueryable<Transaction> query, DateTime currentDate)
        {
            DateTime monthlyModeStartDate = currentDate.AddMonths(-currentDate.Month + 1);

            List<ChartData> chartData = await query
                .GroupBy(t => t.TransactionDate.Month)
                .OrderBy(g => g.Key)
                .Select(subgroup => new ChartData
                {
                    Label = new DateTime(1, subgroup.Key, 1).ToString("MMM"),
                    DataUnits = subgroup
                        .GroupBy(t => t.TransactionTypeId)
                        .Select(subgroup => new DataUnit
                        {
                            Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                            Value = subgroup.Sum(t => t.Amount)
                        }).ToList()
                }).ToListAsync();
            chartData = chartData
                .Select(cd => new ChartData
                {
                    Label = cd.Label,
                    DataUnits = ChartDataConstants
                        .ALL_TRANSACTION_TYPES
                        .GroupJoin(
                            cd.DataUnits,
                            type => type.Label,
                            dataUnit => dataUnit.Label,
                            (type, matchingDataUnits) => new DataUnit
                            {
                                Label = type.Label,
                                Value = matchingDataUnits.FirstOrDefault()?.Value ?? 0
                            }
                        ).ToList()
                }).ToList();


            for (int monthIterator = 0; monthlyModeStartDate.AddMonths(monthIterator) <= currentDate; monthIterator++)
            {
                string newLabel = monthlyModeStartDate.AddMonths(monthIterator).ToString("MMM");
                if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                    chartData.Insert(monthIterator, new ChartData
                    {
                        Label = newLabel,
                        DataUnits = ChartDataConstants.ALL_TRANSACTION_TYPES
                    });
            }

            return chartData;
        }
        /// <summary>
        /// Retrieves yearly bar chart data for the past 10 years, showing transaction counts per year.
        /// </summary>
        /// <param name="currentDate">The reference date used to determine the year range.</param>
        /// <returns>A list of chart data with year labels and counts.</returns>
        private async Task<List<ChartData>> GetYearlyTransactionsVolume(IQueryable<Transaction> query, DateTime currentDate)
        {
            DateTime yearlyModeStartDate = currentDate.AddYears(-10);

            List<ChartData> chartData = await query
                .GroupBy(t => t.TransactionDate.Year)
                .OrderBy(g => g.Key)
                .Select(subgroup => new ChartData
                {
                    Label = subgroup.Key.ToString(),
                    DataUnits = subgroup
                        .GroupBy(t => t.TransactionTypeId)
                        .Select(subgroup => new DataUnit
                        {
                            Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                            Value = subgroup.Sum(t => t.Amount)
                        }).ToList()
                }).ToListAsync();
            chartData = chartData
                .Select(cd => new ChartData
                {
                    Label = cd.Label,
                    DataUnits = ChartDataConstants
                        .ALL_TRANSACTION_TYPES
                        .GroupJoin(
                            cd.DataUnits,
                            type => type.Label,
                            dataUnit => dataUnit.Label,
                            (type, matchingDataUnits) => new DataUnit
                            {
                                Label = type.Label,
                                Value = matchingDataUnits.FirstOrDefault()?.Value ?? 0
                            }
                        ).ToList()
                }).ToList();

            for (int yearIterator = 0; yearlyModeStartDate.AddYears(yearIterator) <= currentDate; yearIterator++)
            {
                string newLabel = yearlyModeStartDate.AddYears(yearIterator).Year.ToString();
                if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                    chartData.Insert(yearIterator, new ChartData
                    {
                        Label = newLabel,
                        DataUnits = ChartDataConstants.ALL_TRANSACTION_TYPES
                    });
            }

            return chartData;
        }
        #endregion
        #region Transaction Count By Time Filter
        /// <summary>
        /// Retrieves bar chart data for all predefined time filters (hourly, daily, weekly, monthly, yearly).
        /// </summary>
        /// <param name="currentDate">The reference date used to calculate the time ranges.</param>
        /// <returns>A dictionary mapping each time filter to its corresponding list of chart data.</returns>
        public async Task<Dictionary<string, List<ChartData>>> GetTransactionsCountDictionary(DateTime currentDate, int acccountId = 0)
        {
            Dictionary<string, List<ChartData>> chartCache = new();
            foreach (var filter in AdminDashboardTimeFilters.AS_STRING_LIST)
                chartCache[filter] = await GetTransactionsCountByTimeFilter(filter, currentDate.AddDays(-1));
            return chartCache;
        }
        /// <summary>
        /// Retrieves bar chart data based on the specified time filter.
        /// </summary>
        /// <param name="filterMode">The time filter mode (e.g., HOURLY, DAILY, etc.).</param>
        /// <param name="currentDate">The reference date used to calculate the time range.</param>
        /// <returns>A list of chart data for the specified filter.</returns>
        public async Task<List<ChartData>> GetTransactionsCountByTimeFilter(string filterMode, DateTime currentDate, int accountId = 0)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                TransactionRepository transactionRepo = new TransactionRepository(dbContext);

                var query = BuildTransactionQuery(transactionRepo, filterMode, currentDate, accountId);

                return filterMode switch
                {
                    AdminDashboardTimeFilters.HOURLY => await GetHourlyTransactionsCount(query, currentDate),
                    AdminDashboardTimeFilters.DAILY => await GetDailyTransactionsCount(query, currentDate),
                    AdminDashboardTimeFilters.WEEKLY => await GetWeeklyTransactionsCount(query, currentDate),
                    AdminDashboardTimeFilters.MONTHLY => await GetMonthlyTransactionsCount(query, currentDate),
                    AdminDashboardTimeFilters.YEARLY => await GetYearlyTransactionsCount(query, currentDate),
                    _ => new()
                };

            }
        }
        /// <summary>
        /// Retrieves hourly bar chart data representing the transaction volume per hour from the start of the day to the current hour.
        /// </summary>
        /// <param name="currentDate">The reference date and time.</param>
        /// <returns>A list of chart data with hourly labels and counts.</returns>
        private async Task<List<ChartData>> GetHourlyTransactionsCount(IQueryable<Transaction> query, DateTime currentDate)
        {
            List<ChartData> chartData = await query
                    .GroupBy(t => t.TransactionTime.Hours)
                    .OrderBy(g => g.Key)
                    .Select(subgroup => new ChartData
                    {
                        Label = new DateTime(1, 1, 1, subgroup.Key, 0, 0).ToString("hh tt"),
                        DataUnits = subgroup
                            .GroupBy(t => t.TransactionTypeId)
                            .Select(subgroup => new DataUnit
                            {
                                Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                                Value = subgroup.Count()
                            }).ToList()
                    }).ToListAsync();
            chartData = chartData
                .Select(cd => new ChartData
                {
                    Label = cd.Label,
                    DataUnits = ChartDataConstants
                        .ALL_TRANSACTION_TYPES
                        .GroupJoin(
                            cd.DataUnits,
                            type => type.Label,
                            dataUnit => dataUnit.Label,
                            (type, matchingDataUnits) => new DataUnit
                            {
                                Label = type.Label,
                                Value = matchingDataUnits.FirstOrDefault()?.Value ?? 0
                            }
                        ).ToList()
                }).ToList();

            int currentHour = currentDate.Hour;
            for (int i = 0; i <= currentHour; i++)
            {
                string newLabel = new DateTime(1, 1, 1, i, 0, 0).ToString("hh tt");
                if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                    chartData.Insert(i, new()
                    {
                        Label = newLabel,
                        DataUnits = ChartDataConstants.ALL_TRANSACTION_TYPES
                    });
            }

            return chartData;
        }
        /// <summary>
        /// Retrieves daily bar chart data representing transaction counts for each of the past 7 days.
        /// </summary>
        /// <param name="currentDate">The reference date.</param>
        /// <returns>A list of chart data with daily labels (e.g., "Apr 09") and counts.</returns>
        private async Task<List<ChartData>> GetDailyTransactionsCount(IQueryable<Transaction> query, DateTime currentDate)
        {
            DateTime dailyModeStartDate = currentDate.AddDays(-7);

            List<ChartData> chartData = await query
                .GroupBy(t => t.TransactionDate.Date)
                .OrderBy(g => g.Key)
                .Select(subgroup => new ChartData
                {
                    Label = subgroup.Key.ToString("MMM dd"),
                    DataUnits = subgroup
                        .GroupBy(t => t.TransactionTypeId)
                        .Select(subgroup => new DataUnit
                        {
                            Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                            Value = subgroup.Count()
                        }).ToList()
                }).ToListAsync();
            chartData = chartData
                .Select(cd => new ChartData
                {
                    Label = cd.Label,
                    DataUnits = ChartDataConstants
                        .ALL_TRANSACTION_TYPES
                        .GroupJoin(
                            cd.DataUnits,
                            type => type.Label,
                            dataUnit => dataUnit.Label,
                            (type, matchingDataUnits) => new DataUnit
                            {
                                Label = type.Label,
                                Value = matchingDataUnits.FirstOrDefault()?.Value ?? 0
                            }
                        ).ToList()
                }).ToList();
            for (int dayIterator = 0; dailyModeStartDate.AddDays(dayIterator) <= currentDate; dayIterator++)
            {
                string newLabel = dailyModeStartDate.AddDays(dayIterator).ToString("MMM dd");
                if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                    chartData.Insert(dayIterator, new()
                    {
                        Label = newLabel,
                        DataUnits = ChartDataConstants.ALL_TRANSACTION_TYPES
                    });
            }

            return chartData;
        }
        /// <summary>
        /// Retrieves weekly bar chart data, aggregating transaction counts into four weekly groups over the past 28 days.
        /// </summary>
        /// <param name="currentDate">The reference date.</param>
        /// <returns>A list of chart data with "Week 1", "Week 2", etc., as labels and total weekly counts.</returns>
        private async Task<List<ChartData>> GetWeeklyTransactionsCount(IQueryable<Transaction> query, DateTime currentDate)
        {
            //  Get the start date of the weekly filter mode.
            DateTime weeklyModeStartDate = currentDate.AddDays(-28);
            //  Group the transactions by day and cast the result into a temporary object.
            var result = await query
                .GroupBy(t => t.TransactionDate.Date)
                .OrderBy(g => g.Key)
                .Select(subgroup => new
                {
                    Label = subgroup.Key,
                    DataUnits = subgroup
                        .GroupBy(t => t.TransactionTypeId)
                        .Select(subgroup => new DataUnit
                        {
                            Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                            Value = subgroup.Count()
                        }).ToList()
                }).ToListAsync();
            result = result
                .Select(cd => new
                {
                    Label = cd.Label,
                    DataUnits = ChartDataConstants
                        .ALL_TRANSACTION_TYPES
                        .GroupJoin(
                            cd.DataUnits,
                            type => type.Label,
                            dataUnit => dataUnit.Label,
                            (type, matchingDataUnits) => new DataUnit
                            {
                                Label = type.Label,
                                Value = matchingDataUnits.FirstOrDefault()?.Value ?? 0
                            }
                        ).ToList()
                }).ToList();

            //  Initialize fillers for days where no transactions took place.
            for (int dayIterator = 0; weeklyModeStartDate.AddDays(dayIterator) <= currentDate; dayIterator++)
            {
                DateTime newLabel = weeklyModeStartDate.AddDays(dayIterator);
                if (!result.Any(t => t.Label == newLabel))
                    result.Insert(dayIterator, new
                    {
                        Label = newLabel,
                        DataUnits = ChartDataConstants.ALL_TRANSACTION_TYPES
                    });
            }

            //  Cast the result into the LineChartData object indexed by week.
            List<ChartData> chartData = new();
            int weekindex = 1;
            for (int i = 0; i < result.Count; i += 7)
            {
                string newLabel = $"Week {weekindex++}";
                int take = Math.Min(7, result.Count - i);
                var dailyDataInWeek = result
                    .Skip(i)
                    .Take(take)
                    .ToList();
                var unitSummations = Enumerable.Range(0, dailyDataInWeek[0].DataUnits.Count)
                    .Select(col => new DataUnit
                    {
                        Label = dailyDataInWeek[0].DataUnits[col].Label,
                        Value = dailyDataInWeek.Sum(day => day.DataUnits[col].Value)
                    }).ToList();
                chartData.Add(new ChartData
                {
                    Label = newLabel,
                    DataUnits = unitSummations
                });
            }

            return chartData;
        }
        /// <summary>
        /// Retrieves monthly bar chart data for the current year, showing transaction counts per month.
        /// </summary>
        /// <param name="currentDate">The reference date used to determine the current year.</param>
        /// <returns>A list of chart data with monthly labels (e.g., "Jan", "Feb") and counts.</returns>
        private async Task<List<ChartData>> GetMonthlyTransactionsCount(IQueryable<Transaction> query, DateTime currentDate)
        {
            DateTime monthlyModeStartDate = currentDate.AddMonths(-currentDate.Month + 1);

            List<ChartData> chartData = await query
                .GroupBy(t => t.TransactionDate.Month)
                .OrderBy(g => g.Key)
                .Select(subgroup => new ChartData
                {
                    Label = new DateTime(1, subgroup.Key, 1).ToString("MMM"),
                    DataUnits = subgroup
                        .GroupBy(t => t.TransactionTypeId)
                        .Select(subgroup => new DataUnit
                        {
                            Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                            Value = subgroup.Count()
                        }).ToList()
                }).ToListAsync();

            chartData = chartData
                .Select(cd => new ChartData
                {
                    Label = cd.Label,
                    DataUnits = ChartDataConstants
                        .ALL_TRANSACTION_TYPES
                        .GroupJoin(
                            cd.DataUnits,
                            type => type.Label,
                            dataUnit => dataUnit.Label,
                            (type, matchingDataUnits) => new DataUnit
                            {
                                Label = type.Label,
                                Value = matchingDataUnits.FirstOrDefault()?.Value ?? 0
                            }
                        ).ToList()
                }).ToList();


            for (int monthIterator = 0; monthlyModeStartDate.AddMonths(monthIterator) <= currentDate; monthIterator++)
            {
                string newLabel = monthlyModeStartDate.AddMonths(monthIterator).ToString("MMM");
                if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                    chartData.Insert(monthIterator, new ChartData
                    {
                        Label = newLabel,
                        DataUnits = ChartDataConstants.ALL_TRANSACTION_TYPES
                    });
            }

            return chartData;
        }
        /// <summary>
        /// Retrieves yearly bar chart data for the past 10 years, showing transaction counts per year.
        /// </summary>
        /// <param name="currentDate">The reference date used to determine the year range.</param>
        /// <returns>A list of chart data with year labels and counts.</returns>
        private async Task<List<ChartData>> GetYearlyTransactionsCount(IQueryable<Transaction> query, DateTime currentDate)
        {
            DateTime yearlyModeStartDate = currentDate.AddYears(-10);

            List<ChartData> chartData = await query
                .GroupBy(t => t.TransactionDate.Year)
                .OrderBy(g => g.Key)
                .Select(subgroup => new ChartData
                {
                    Label = subgroup.Key.ToString(),
                    DataUnits = subgroup
                        .GroupBy(t => t.TransactionTypeId)
                        .Select(subgroup => new DataUnit
                        {
                            Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                            Value = subgroup.Count()
                        }).ToList()
                }).ToListAsync();
            chartData = chartData
                .Select(cd => new ChartData
                {
                    Label = cd.Label,
                    DataUnits = ChartDataConstants
                        .ALL_TRANSACTION_TYPES
                        .GroupJoin(
                            cd.DataUnits,
                            type => type.Label,
                            dataUnit => dataUnit.Label,
                            (type, matchingDataUnits) => new DataUnit
                            {
                                Label = type.Label,
                                Value = matchingDataUnits.FirstOrDefault()?.Value ?? 0
                            }
                        ).ToList()
                }).ToList();

            for (int yearIterator = 0; yearlyModeStartDate.AddYears(yearIterator) <= currentDate; yearIterator++)
            {
                string newLabel = yearlyModeStartDate.AddYears(yearIterator).Year.ToString();
                if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                    chartData.Insert(yearIterator, new ChartData
                    {
                        Label = newLabel,
                        DataUnits = ChartDataConstants.ALL_TRANSACTION_TYPES
                    });
            }

            return chartData;
        }
        #endregion
        private IQueryable<Transaction> BuildTransactionQuery(
            TransactionRepository transactionRepo, 
            string filterMode, 
            DateTime currentDate, 
            int accountId = 0)
        {
            var queryBuilder = transactionRepo
                .Query
                .ExceptStatus(TransactionStatus.CANCELLED)
                .ExceptStatus(TransactionStatus.DENIED)
                .HasStartDate(filterMode switch
                {
                    AdminDashboardTimeFilters.HOURLY => currentDate.Date,
                    AdminDashboardTimeFilters.DAILY => currentDate.AddDays(-7),
                    AdminDashboardTimeFilters.WEEKLY => currentDate.AddDays(-28),
                    AdminDashboardTimeFilters.MONTHLY => currentDate.AddMonths(-currentDate.Month + 1),
                    AdminDashboardTimeFilters.YEARLY => currentDate.AddYears(-10),
                    _ => currentDate.Date
                });
            if (accountId > 0)
                queryBuilder.HasMainAccountId(accountId);

            return queryBuilder.GetQuery();
        }

        #region Data Chart Fillers
        private List<ChartData> FillHourlyData(List<ChartData> chartData, DateTime currentDate)
        {

            int currentHour = currentDate.Hour;
            for (int i = 0; i <= currentHour; i++)
            {
                string newLabel = new DateTime(1, 1, 1, i, 0, 0).ToString("hh tt");
                if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                    chartData.Insert(i, new()
                    {
                        Label = newLabel,
                        Value = 0
                    });
            }

            return chartData;
        }
        private List<ChartData> FillDailyData(List<ChartData> chartData, DateTime currentDate)
        {

            //  fillers
            DateTime dailyModeStartDate = currentDate.AddDays(-7);
            for (int dayIterator = 0; dailyModeStartDate.AddDays(dayIterator) <= currentDate; dayIterator++)
            {
                string newLabel = dailyModeStartDate.AddDays(dayIterator).ToString("MMM dd");
                if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                    chartData.Insert(dayIterator, new()
                    {
                        Label = newLabel,
                        Value = 0
                    });
            }

            return chartData;
        }
        private List<TempChartData> FillWeeklyData(List<TempChartData> chartData, DateTime currentDate)
        {
            //  Get the start date of the weekly filter mode.
            DateTime weeklyModeStartDate = currentDate.AddDays(-28);
            //  Initialize fillers for days where no transactions took place.
            for (int dayIterator = 0; weeklyModeStartDate.AddDays(dayIterator) <= currentDate; dayIterator++)
            {
                DateTime newLabel = weeklyModeStartDate.AddDays(dayIterator);
                if (!chartData.Any(t => t.Label == newLabel))
                    chartData.Insert(dayIterator, new TempChartData
                    {
                        Label = newLabel,
                        Value = 0.0m
                    });
            }
            return chartData;
        }
        private List<ChartData> FillMonthlyData(List<ChartData> chartData, DateTime currentDate)
        {
            DateTime monthlyModeStartDate = currentDate.AddMonths(-currentDate.Month + 1);

            for (int monthIterator = 0; monthlyModeStartDate.AddMonths(monthIterator) <= currentDate; monthIterator++)
            {
                string newLabel = monthlyModeStartDate.AddMonths(monthIterator).ToString("MMM");
                if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                    chartData.Insert(monthIterator, new ChartData
                    {
                        Label = newLabel,
                        Value = 0
                    });
            }
            return chartData;
        }
        private List<ChartData> FillYearlyData(List<ChartData> chartData, DateTime currentDate)
        {
            DateTime yearlyModeStartDate = currentDate.AddYears(-10);
            for (int yearIterator = 0; yearlyModeStartDate.AddYears(yearIterator) <= currentDate; yearIterator++)
            {
                string newLabel = yearlyModeStartDate.AddYears(yearIterator).Year.ToString();
                if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                    chartData.Insert(yearIterator, new ChartData
                    {
                        Label = newLabel,
                        Value = 0
                    });
            }
            return chartData;
        }
        #endregion
        #endregion
        #endregion
    }
}
