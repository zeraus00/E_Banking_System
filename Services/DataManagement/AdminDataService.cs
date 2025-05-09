using System.Linq;
using Data;
using Data.Constants;
using Data.Enums;
using Data.Repositories.Auth;
using Data.Repositories.Finance;
using Data.Repositories.JointEntity;
using Data.Repositories.User;
using Exceptions;
using Services.SessionsManagement;
using ViewModels.AdminDashboard;

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
        #region Account Helper Methods
        /// <summary>
        /// Asynchronously filters and retrieves a list of pending <see cref="Account"/> based on optional criteria.
        /// </summary>
        /// <param name="accountNumber">Optional account number to filter by. If empty or null, this filter is ignored.</param>
        /// <param name="startDate">Optional start date to filter accounts opened on or after this date. If null, this filter is ignored.</param>
        /// <param name="endDate">Optional end date to filter accounts opened on or before this date. If null, this filter is ignored.</param>
        /// <param name="accountTypeId">Optional account type ID to filter by. Only applies if greater than zero.</param>
        /// <returns>A list of <see cref="Account"> objects matching the specified filters.</returns>
        public async Task<List<Account>> FilterAccountsAsync(
            string accountNumber = "",
            DateTime? startDate = null,
            DateTime? endDate = null,
            int accountTypeId = 0,
            int accountStatusTypeId = 0
            )
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                //  Declare repository dependencies.
                var accountRepo = new AccountRepository(dbContext);

                //  Filter the accounts.
                var queryBuilder = accountRepo.Query;
                queryBuilder.IncludeAccountType();
                queryBuilder.IncludeAccountStatusType();
                queryBuilder.OrderByDateOpenedDescending();

                if (!string.IsNullOrWhiteSpace(accountNumber))
                    queryBuilder.HasAccountNumber(accountNumber);
                if (startDate is not null)
                    queryBuilder.HasOpenedOnOrAfter(startDate);
                if (endDate is not null)
                    queryBuilder.HasOpenedOnOrBefore(endDate);
                if (accountTypeId > 0)
                    queryBuilder.HasAccountTypeId(accountTypeId);
                if (accountStatusTypeId > 0)
                    queryBuilder.HasAccountStatusTypeId(accountStatusTypeId);

                //  Pagination
                int pageNumber = 1;
                int pageSize = 10;
                List<Account> accountList = await queryBuilder
                    .GetQuery()
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
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
        #region Pending Accounts Management
        /// <summary>
        /// Updates the <see cref="Account.AccountStatusType"> of an <see cref="Account"/> with a
        /// specified <see cref="Account.AccountId"/> and a <see cref="Account.AccountStatusTypeId"/>
        /// of <see cref="AccountStatusTypeIDs.Pending"/>.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public async Task UpdatePendingAccountStatus(int accountId, int newStatus)
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
        public int GetTransactionVolume(Dictionary<string, TransactionBreakdown> transactionBreakdowns)
        {
            int sum = 0;
            foreach (var kvp in transactionBreakdowns)
            {
                sum += kvp.Value.Count;
            }
            return sum;
        }
        /// <summary>
        /// Calculates the net movement of currency in the bank through a 
        /// <see cref="Dictionary{string, TransactionBreakdown}"> containing the details of transaction
        /// breakdowns.
        /// </summary>
        /// <param name="transactionBreakdowns">The main <see cref="Dictionary{string, TransactionBreakdown}">.</param>
        /// <returns>The calculated <see cref="decimal"/> net movement based on the provided <see cref="List{Transaction}">.
        /// </returns>
        public decimal GetNetMovement(Dictionary<string, TransactionBreakdown> transactionBreakdowns)
        {
            decimal inflow = transactionBreakdowns[TransactionTypes.DEPOSIT].Total;
            decimal outflow = transactionBreakdowns[TransactionTypes.WITHDRAWAL].Total;

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
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer);

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
                    .OrderByAmountDescending()
                    .TakeWithCount(count);
                if (startDate is DateTime sDate)
                    queryBuilder.HasStartDate(sDate);
                if (endDate is DateTime eDate)
                    queryBuilder.HasEndDate(eDate);

                List<Transaction> largestTransactions = await queryBuilder.GetQuery().ToListAsync();

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
        public async Task<List<Transaction>> GetTransactionListAsync(
            DateTime? transactionStartDate = null, 
            DateTime? transactionEndDate = null,
            int transactionTypeId = 0,
            int pageSize = 0,
            int pageNumber = 0)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                //  Declare repository dependency.
                var transactionRepo = new TransactionRepository(dbContext);

                //  Compose Query
                var queryBuilder = transactionRepo
                    .Query
                    .IncludeMainAccount()
                    .IncludeTransactionType()
                    .OrderByDateAndTimeDescending();

                if (transactionStartDate is DateTime startDate)
                    queryBuilder.HasStartDate(startDate);
                if (transactionEndDate is DateTime endDate)
                    queryBuilder.HasEndDate(endDate);
                if (transactionTypeId > 0)
                    queryBuilder.HasTransactionTypeId(transactionTypeId);
                
                if (pageSize > 0 && pageNumber > 0)
                {
                    int skipCount = (pageNumber - 1) * pageSize;
                    int takeCount = pageSize;
                    queryBuilder
                        .SkipBy(skipCount)
                        .TakeWithCount(takeCount);
                }

                List<Transaction> transactions = await queryBuilder.GetQuery().ToListAsync();
                foreach (var transaction in transactions)
                {
                    string accountNumber = transaction.MainAccount.AccountNumber;
                    transaction.MainAccount.AccountNumber = _dataMaskingService.MaskAccountNumber(accountNumber);
                }

                return transactions;
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
                    .OrderByDateDescending();

                if (startDate is DateTime sDate)
                    queryBuilder.HasStartDateFilter(sDate);
                if (endDate is DateTime eDate)
                    queryBuilder.HasEndDateFilter(eDate);

                return await queryBuilder.GetCountAsync();
            }
        }
        #endregion
        #region Chart Data
        public async Task<Dictionary<string, decimal>> GetTransactionVolumeOverTime(string filterMode, DateTime startDate)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var transactionRepo = new TransactionRepository(dbContext);

                Dictionary<string, decimal> transactionVolumeOverTime = new();

                var queryBuilder = transactionRepo.Query;
                if (filterMode.Equals(AdminDashboardTimeFilters.HOURLY))
                    transactionVolumeOverTime = await queryBuilder
                        .OrderByDateAndTime()
                        .HasStartDate(startDate.Date)
                        .GetQuery()
                        .GroupBy(t => t.TransactionTime.Hours)
                        .ToDictionaryAsync(
                            g => new DateTime(1, 1, 1, g.Key, 0, 0).ToString("hh tt"),
                            g => g.Sum(t => t.Amount)
                        );
                else if (filterMode.Equals(AdminDashboardTimeFilters.DAILY))
                    transactionVolumeOverTime = await queryBuilder
                        .OrderByDateAndTime()
                        .HasStartDate(startDate.AddDays(-7))
                        .GetQuery()
                        .GroupBy(t => t.TransactionDate.DayOfWeek)
                        .ToDictionaryAsync(
                            g => g.Key.ToString(),
                            g => g.Sum(t => t.Amount)
                        );
                else if (filterMode.Equals(AdminDashboardTimeFilters.WEEKLY))
                {
                    var tempDict = await queryBuilder
                        .OrderByDateAndTime()
                        .HasStartDate(startDate.AddDays(-28))
                        .GetQuery()
                        .GroupBy(t => t.TransactionDate.Date)
                        .ToDictionaryAsync(
                            g => g.Key,
                            g => g.Sum(t => t.Amount)
                        );

                    var orderedDays = tempDict.OrderBy(kvp => kvp.Key).ToList();
                    int weekIndex = 1;
                    for (int i = 0; i < orderedDays.Count; i+=7)
                    {
                        string key = $"Week {weekIndex++}";
                        int take = Math.Min(7, orderedDays.Count - i);
                        decimal value = orderedDays.Skip(i).Take(take).Sum(kvp => kvp.Value);
                        transactionVolumeOverTime[key] = value;
                    }
                }
                else if (filterMode.Equals(AdminDashboardTimeFilters.MONTHLY))
                    transactionVolumeOverTime = await queryBuilder
                        .OrderByDateAndTime()
                        .HasStartDate(startDate.AddMonths(-startDate.Month + 1))
                        .GetQuery()
                        .GroupBy(t => t.TransactionDate.Month)
                        .ToDictionaryAsync(
                            g => new DateTime(1, g.Key, 1).ToString("MMM"),
                            g => g.Sum(t => t.Amount)
                        );
                else if (filterMode.Equals(AdminDashboardTimeFilters.YEARLY))
                    transactionVolumeOverTime = await queryBuilder
                        .OrderByDateAndTime()
                        .HasStartDate(startDate.AddYears(-10))
                        .GetQuery()
                        .GroupBy(t => t.TransactionDate.Year)
                        .ToDictionaryAsync(
                            g => g.Key.ToString(),
                            g => g.Sum(t => t.Amount)
                        );

                return transactionVolumeOverTime;
            }
        }
        #endregion
        #endregion
    }
}
