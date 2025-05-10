using System.Globalization;
using System.Linq;
using Data;
using Data.Constants;
using Data.Enums;
using Data.Repositories.Auth;
using Data.Repositories.Finance;
using Data.Repositories.JointEntity;
using Data.Repositories.User;
using Exceptions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
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
        #region Pending Accounts Helper Methods
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
        #region Manage Accounts Helper Methods
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
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
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
        #region Line Chart
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
        public async Task<Dictionary<string, List<ChartData>>> GetAllLineChartData(DateTime currentDate)
        {
            Dictionary<string, List<ChartData>> chartCache = new();
            foreach(var filter in AdminDashboardTimeFilters.AS_STRING_LIST)
                chartCache[filter] = await GetLineChartData(filter, currentDate.AddDays(-1));
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
        public async Task<List<ChartData>> GetLineChartData(string filterMode, DateTime currentDate)
        {
            return filterMode switch
            {
                AdminDashboardTimeFilters.HOURLY => await GetHourlyLineChartData(currentDate),
                AdminDashboardTimeFilters.DAILY => await GetDailyLineChartData(currentDate),
                AdminDashboardTimeFilters.WEEKLY => await GetWeeklyLineChartData(currentDate),
                AdminDashboardTimeFilters.MONTHLY => await GetMonthlyLineChartData(currentDate),
                AdminDashboardTimeFilters.YEARLY => await GetYearlyLineChartData(currentDate),
                _ => new List<ChartData>()
            };
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
        private async Task<List<ChartData>> GetHourlyLineChartData(DateTime currentDate)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                TransactionRepository transactionRepo = new TransactionRepository(dbContext);
                var queryBuilder = transactionRepo.Query;

                List<ChartData> chartData = await queryBuilder
                    .HasStartDate(currentDate.Date)
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .GetQuery()
                    .GroupBy(t => t.TransactionTime.Hours)
                    .OrderBy(g => g.Key)
                    .Select(g => new ChartData
                    {
                        Label = new DateTime(1, 1, 1, g.Key, 0, 0).ToString("hh tt"),
                        Value = g.Sum(t => t.Amount)
                    }).ToListAsync();
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
        private async Task<List<ChartData>> GetDailyLineChartData(DateTime currentDate)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                TransactionRepository transactionRepo = new TransactionRepository(dbContext);
                var queryBuilder = transactionRepo.Query;

                DateTime dailyModeStartDate = currentDate.AddDays(-7);
                
                List<ChartData> chartData = await queryBuilder
                    .HasStartDate(dailyModeStartDate.Date)
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .GetQuery()
                    .GroupBy(t => t.TransactionDate.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new ChartData
                    {
                        Label = g.Key.ToString("MMM dd"),
                        Value = g.Sum(t => t.Amount)
                    }).ToListAsync();

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
        private async Task<List<ChartData>> GetWeeklyLineChartData(DateTime currentDate)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                TransactionRepository transactionRepo = new TransactionRepository(dbContext);
                var queryBuilder = transactionRepo.Query;

                //  Get the start date of the weekly filter mode.
                DateTime weeklyModeStartDate = currentDate.AddDays(-28);
                //  Group the transactions by day and cast the result into a temporary object.
                var result = await queryBuilder
                    .HasStartDate(weeklyModeStartDate.Date)
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .GetQuery()
                    .GroupBy(t => t.TransactionDate.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new
                    {
                        Label = g.Key,
                        Value = g.Sum(t => t.Amount)
                    }).ToListAsync();

                //  Initialize fillers for days where no transactions took place.
                for (int dayIterator = 0; weeklyModeStartDate.AddDays(dayIterator) <= currentDate; dayIterator++)
                {
                    DateTime newLabel = weeklyModeStartDate.AddDays(dayIterator);
                    if (!result.Any(t => t.Label == newLabel))
                        result.Insert(dayIterator, new
                        {
                            Label = newLabel,
                            Value = 0.0m
                        });
                }

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
        private async Task<List<ChartData>> GetMonthlyLineChartData(DateTime currentDate)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                TransactionRepository transactionRepo = new TransactionRepository(dbContext);
                var queryBuilder = transactionRepo.Query;
                DateTime monthlyModeStartDate = currentDate.AddMonths(-currentDate.Month + 1);

                List<ChartData> chartData = await queryBuilder
                    .HasStartDate(monthlyModeStartDate.Date)
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .GetQuery()
                    .GroupBy(t => t.TransactionDate.Month)
                    .OrderBy(g => g.Key)
                    .Select(g => new ChartData
                    {
                        Label = new DateTime(1, g.Key, 1).ToString("MMM"),
                        Value = g.Sum(t => t.Amount)
                    }).ToListAsync();

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
        private async Task<List<ChartData>> GetYearlyLineChartData(DateTime currentDate)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                TransactionRepository transactionRepo = new TransactionRepository(dbContext);
                var queryBuilder = transactionRepo.Query;
                DateTime yearlyModeStartDate = currentDate.AddYears(-10);

                List<ChartData> chartData = await queryBuilder
                    .HasStartDate(yearlyModeStartDate)
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .GetQuery()
                    .GroupBy(t => t.TransactionDate.Year)
                    .OrderBy(g => g.Key)
                    .Select(g => new ChartData
                    {
                        Label = g.Key.ToString(),
                        Value = g.Sum(t => t.Amount)
                    }).ToListAsync();

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
        }
        #endregion
        #region Pie Chart
        /// <summary>
        /// Retrieves pie chart data for all time filter modes (hourly, daily, weekly, monthly, yearly),
        /// relative to the specified current date.
        /// </summary>
        /// <param name="currentDate">The base date used to calculate each time filter's start date.</param>
        /// <returns>A dictionary where each key is a time filter label and the value is the pie chart data for that filter.</returns>
        public async Task<Dictionary<string, List<ChartData>>> GetAllPieChartData(DateTime currentDate)
        {
            Dictionary<string, List<ChartData>> chartCache = new();
            foreach (var filter in AdminDashboardTimeFilters.AS_STRING_LIST)
                chartCache[filter] = await GetPieChartData(filter, currentDate.AddDays(-1));
            return chartCache;
        }
        /// <summary>
        /// Retrieves pie chart data based on the specified time filter mode.
        /// </summary>
        /// <param name="filterMode">The time filter mode (e.g., hourly, daily, weekly).</param>
        /// <param name="currentDate">The current date used to calculate the range of the filter.</param>
        /// <returns>A list of ChartData objects grouped by transaction type.</returns>
        public async Task<List<ChartData>> GetPieChartData(string filterMode, DateTime currentDate)
        {
            return filterMode switch
            {
                AdminDashboardTimeFilters.HOURLY => await GetHourlyPieChartData(currentDate),
                AdminDashboardTimeFilters.DAILY => await GetDailyPieChartData(currentDate),
                AdminDashboardTimeFilters.WEEKLY => await GetWeeklyPieChartData(currentDate),
                AdminDashboardTimeFilters.MONTHLY => await GetMonthlyPieChartData(currentDate),
                AdminDashboardTimeFilters.YEARLY => await GetYearlyPieChartData(currentDate),
                _ => new List<ChartData>()
            };
        }
        /// <summary>
        /// Retrieves pie chart data for the last hour relative to the specified date.
        /// </summary>
        /// <param name="currentDate">The current date and time.</param>
        /// <returns>A list of ChartData for the past hour.</returns>
        private async Task<List<ChartData>> GetHourlyPieChartData(DateTime currentDate) =>
            await GetPieChartDataByDateAndTime(currentDate, currentDate.AddHours(-1).TimeOfDay);
        /// <summary>
        /// Retrieves pie chart data for the previous day.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>A list of ChartData for the current day.</returns>
        private async Task<List<ChartData>> GetDailyPieChartData(DateTime currentDate) =>
            await GetPieChartDataByDateAndTime(currentDate);
        /// <summary>
        /// Retrieves pie chart data for the past 7 days.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>A list of ChartData for the past week.</returns>
        private async Task<List<ChartData>> GetWeeklyPieChartData(DateTime currentDate) =>
            await GetPieChartDataByDateAndTime(currentDate.AddDays(-7));
        /// <summary>
        /// Retrieves pie chart data for the past month (approximated using DateTime.AddMonths).
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>A list of ChartData for the past month.</returns>
        private async Task<List<ChartData>> GetMonthlyPieChartData(DateTime currentDate) =>
            await GetPieChartDataByDateAndTime(currentDate.AddMonths(-1));
        /// <summary>
        /// Retrieves pie chart data for the past year (approximated using DateTime.AddYears).
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>A list of ChartData for the past year.</returns>
        private async Task<List<ChartData>> GetYearlyPieChartData(DateTime currentDate) =>
            await GetPieChartDataByDateAndTime(currentDate.AddYears(-1));
        /// <summary>
        /// Retrieves pie chart data starting from a given date and optionally a specific time of day.
        /// Filters out transactions of a specific type (Incoming Transfer).
        /// </summary>
        /// <param name="startDate">The starting date for the data range.</param>
        /// <param name="startTime">Optional: the starting time on the given date to begin filtering.</param>
        /// <returns>A list of ChartData grouped by transaction type.</returns>
        private async Task<List<ChartData>> GetPieChartDataByDateAndTime(DateTime startDate, TimeSpan? startTime = null)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var transactionRepo = new TransactionRepository(dbContext);

                var queryBuilder = transactionRepo.Query;

                if (startTime is not null)
                    queryBuilder.HasStartTime(startTime.Value);

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
        #region Bar Chart
        /// <summary>
        /// Retrieves bar chart data for all predefined time filters (hourly, daily, weekly, monthly, yearly).
        /// </summary>
        /// <param name="currentDate">The reference date used to calculate the time ranges.</param>
        /// <returns>A dictionary mapping each time filter to its corresponding list of chart data.</returns>
        public async Task<Dictionary<string, List<BarChartData>>> GetAllBarChartData(DateTime currentDate)
        {
            Dictionary<string, List<BarChartData>> chartCache = new();
            foreach (var filter in AdminDashboardTimeFilters.AS_STRING_LIST)
                chartCache[filter] = await GetBarChartData(filter, currentDate.AddDays(-1));
            return chartCache;
        }
        /// <summary>
        /// Retrieves bar chart data based on the specified time filter.
        /// </summary>
        /// <param name="filterMode">The time filter mode (e.g., HOURLY, DAILY, etc.).</param>
        /// <param name="currentDate">The reference date used to calculate the time range.</param>
        /// <returns>A list of chart data for the specified filter.</returns>
        public async Task<List<BarChartData>> GetBarChartData(string filterMode, DateTime currentDate)
        {
            return filterMode switch
            {
                AdminDashboardTimeFilters.HOURLY => await GetHourlyBarChartData(currentDate),
                AdminDashboardTimeFilters.DAILY => await GetDailyBarChartData(currentDate),
                AdminDashboardTimeFilters.WEEKLY => await GetWeeklyBarChartData(currentDate),
                AdminDashboardTimeFilters.MONTHLY => await GetMonthlyBarChartData(currentDate),
                AdminDashboardTimeFilters.YEARLY => await GetYearlyBarChartData(currentDate),
                _ => new()
            };
        }
        /// <summary>
        /// Retrieves hourly bar chart data representing the transaction volume per hour from the start of the day to the current hour.
        /// </summary>
        /// <param name="currentDate">The reference date and time.</param>
        /// <returns>A list of chart data with hourly labels and counts.</returns>
        private async Task<List<BarChartData>> GetHourlyBarChartData(DateTime currentDate)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                TransactionRepository transactionRepo = new TransactionRepository(dbContext);
                var queryBuilder = transactionRepo.Query;

                List<BarChartData> chartData = await queryBuilder
                    .HasStartDate(currentDate.Date)
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .GetQuery()
                    .GroupBy(t => t.TransactionTime.Hours)
                    .OrderBy(g => g.Key)
                    .Select(subgroup => new BarChartData
                    {
                        Label = new DateTime(1, 1, 1, subgroup.Key, 0, 0).ToString("hh tt"),
                        Bars = subgroup
                            .GroupBy(t => t.TransactionTypeId)
                            .Select(subgroup => new Bar
                            {
                                Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                                Value = subgroup.Count()
                            }).ToList()
                    }).ToListAsync();
                chartData = chartData
                    .Select(cd => new BarChartData
                    {
                        Label = cd.Label,
                        Bars = BarChartConstants
                            .ALL_TRANSACTION_TYPES
                            .GroupJoin(
                                cd.Bars,
                                type => type.Label,
                                bar => bar.Label,
                                (type, matchingBars) => new Bar
                                {
                                    Label = type.Label,
                                    Value = matchingBars.FirstOrDefault()?.Value ?? 0
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
                            Bars = BarChartConstants.ALL_TRANSACTION_TYPES
                        });
                }

                return chartData;
            }
        }
        /// <summary>
        /// Retrieves daily bar chart data representing transaction counts for each of the past 7 days.
        /// </summary>
        /// <param name="currentDate">The reference date.</param>
        /// <returns>A list of chart data with daily labels (e.g., "Apr 09") and counts.</returns>
        private async Task<List<BarChartData>> GetDailyBarChartData(DateTime currentDate)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                TransactionRepository transactionRepo = new TransactionRepository(dbContext);
                var queryBuilder = transactionRepo.Query;

                DateTime dailyModeStartDate = currentDate.AddDays(-7);

                List<BarChartData> chartData = await queryBuilder
                    .HasStartDate(dailyModeStartDate.Date)
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .GetQuery()
                    .GroupBy(t => t.TransactionDate.Date)
                    .OrderBy(g => g.Key)
                    .Select(subgroup => new BarChartData
                    {
                        Label = subgroup.Key.ToString("MMM dd"),
                        Bars = subgroup
                            .GroupBy(t => t.TransactionTypeId)
                            .Select(subgroup => new Bar
                            {
                                Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                                Value = subgroup.Count()
                            }).ToList()
                    }).ToListAsync();
                chartData = chartData
                    .Select(cd => new BarChartData
                    {
                        Label = cd.Label,
                        Bars = BarChartConstants
                            .ALL_TRANSACTION_TYPES
                            .GroupJoin(
                                cd.Bars,
                                type => type.Label,
                                bar => bar.Label,
                                (type, matchingBars) => new Bar
                                {
                                    Label = type.Label,
                                    Value = matchingBars.FirstOrDefault()?.Value ?? 0
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
                            Bars = BarChartConstants.ALL_TRANSACTION_TYPES
                        });
                }

                return chartData;
            }
        }
        /// <summary>
        /// Retrieves weekly bar chart data, aggregating transaction counts into four weekly groups over the past 28 days.
        /// </summary>
        /// <param name="currentDate">The reference date.</param>
        /// <returns>A list of chart data with "Week 1", "Week 2", etc., as labels and total weekly counts.</returns>
        private async Task<List<BarChartData>> GetWeeklyBarChartData(DateTime currentDate)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                TransactionRepository transactionRepo = new TransactionRepository(dbContext);
                var queryBuilder = transactionRepo.Query;

                //  Get the start date of the weekly filter mode.
                DateTime weeklyModeStartDate = currentDate.AddDays(-28);
                //  Group the transactions by day and cast the result into a temporary object.
                var result = await queryBuilder
                    .HasStartDate(weeklyModeStartDate.Date)
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .GetQuery()
                    .GroupBy(t => t.TransactionDate.Date)
                    .OrderBy(g => g.Key)
                    .Select(subgroup => new
                    {
                        Label = subgroup.Key,
                        Values = subgroup
                            .GroupBy(t => t.TransactionTypeId)
                            .Select(subgroup => new Bar
                            {
                                Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                                Value = subgroup.Count()
                            }).ToList()
                    }).ToListAsync();
                result = result
                    .Select(cd => new
                    {
                        Label = cd.Label,
                        Values = BarChartConstants
                            .ALL_TRANSACTION_TYPES
                            .GroupJoin(
                                cd.Values,
                                type => type.Label,
                                bar => bar.Label,
                                (type, matchingBars) => new Bar
                                {
                                    Label = type.Label,
                                    Value = matchingBars.FirstOrDefault()?.Value ?? 0
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
                            Values = BarChartConstants.ALL_TRANSACTION_TYPES
                        });
                }

                //  Cast the result into the LineChartData object indexed by week.
                List<BarChartData> chartData = new();
                int weekindex = 1;
                for (int i = 0; i < result.Count; i += 7)
                {
                    string newLabel = $"Week {weekindex++}";
                    int take = Math.Min(7, result.Count - i);
                    var dailyDataInWeek = result
                        .Skip(i)
                        .Take(take)
                        .ToList();
                    var barSummations = Enumerable.Range(0, dailyDataInWeek[0].Values.Count)
                        .Select(col => new Bar{
                            Label = dailyDataInWeek[0].Values[col].Label,
                            Value = dailyDataInWeek.Sum(day => day.Values[col].Value)
                        }).ToList();
                    chartData.Add(new BarChartData
                    {
                        Label = newLabel,
                        Bars = barSummations
                    });
                }

                return chartData;
            }
        }
        /// <summary>
        /// Retrieves monthly bar chart data for the current year, showing transaction counts per month.
        /// </summary>
        /// <param name="currentDate">The reference date used to determine the current year.</param>
        /// <returns>A list of chart data with monthly labels (e.g., "Jan", "Feb") and counts.</returns>
        private async Task<List<BarChartData>> GetMonthlyBarChartData(DateTime currentDate)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                TransactionRepository transactionRepo = new TransactionRepository(dbContext);
                var queryBuilder = transactionRepo.Query;
                DateTime monthlyModeStartDate = currentDate.AddMonths(-currentDate.Month + 1);

                List<BarChartData> chartData = await queryBuilder
                    .HasStartDate(monthlyModeStartDate.Date)
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .GetQuery()
                    .GroupBy(t => t.TransactionDate.Month)
                    .OrderBy(g => g.Key)
                    .Select(subgroup => new BarChartData
                    {
                        Label = new DateTime(1, subgroup.Key, 1).ToString("MMM"),
                        Bars = subgroup
                            .GroupBy(t => t.TransactionTypeId)
                            .Select(subgroup => new Bar
                            {
                                Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                                Value = subgroup.Count()
                            }).ToList()
                    }).ToListAsync();
                chartData = chartData
                    .Select(cd => new BarChartData
                    {
                        Label = cd.Label,
                        Bars = BarChartConstants
                            .ALL_TRANSACTION_TYPES
                            .GroupJoin(
                                cd.Bars,
                                type => type.Label,
                                bar => bar.Label,
                                (type, matchingBars) => new Bar
                                {
                                    Label = type.Label,
                                    Value = matchingBars.FirstOrDefault()?.Value ?? 0
                                }
                            ).ToList()
                    }).ToList();


                for (int monthIterator = 0; monthlyModeStartDate.AddMonths(monthIterator) <= currentDate; monthIterator++)
                {
                    string newLabel = monthlyModeStartDate.AddMonths(monthIterator).ToString("MMM");
                    if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                        chartData.Insert(monthIterator, new BarChartData
                        {
                            Label = newLabel,
                            Bars = BarChartConstants.ALL_TRANSACTION_TYPES
                        });
                }

                return chartData;
            }
        }
        /// <summary>
        /// Retrieves yearly bar chart data for the past 10 years, showing transaction counts per year.
        /// </summary>
        /// <param name="currentDate">The reference date used to determine the year range.</param>
        /// <returns>A list of chart data with year labels and counts.</returns>
        private async Task<List<BarChartData>> GetYearlyBarChartData(DateTime currentDate)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                TransactionRepository transactionRepo = new TransactionRepository(dbContext);
                var queryBuilder = transactionRepo.Query;
                DateTime yearlyModeStartDate = currentDate.AddYears(-10);

                List<BarChartData> chartData = await queryBuilder
                    .HasStartDate(yearlyModeStartDate)
                    .ExceptTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
                    .GetQuery()
                    .GroupBy(t => t.TransactionDate.Year)
                    .OrderBy(g => g.Key)
                    .Select(subgroup => new BarChartData
                    {
                        Label = subgroup.Key.ToString(),
                        Bars = subgroup
                            .GroupBy(t => t.TransactionTypeId)
                            .Select(subgroup => new Bar
                            {
                                Label = TransactionTypes.AS_STRING_LIST[subgroup.Key - 1],
                                Value = subgroup.Count()
                            }).ToList()
                    }).ToListAsync();
                chartData = chartData
                    .Select(cd => new BarChartData
                    {
                        Label = cd.Label,
                        Bars = BarChartConstants
                            .ALL_TRANSACTION_TYPES
                            .GroupJoin(
                                cd.Bars,
                                type => type.Label,
                                bar => bar.Label,
                                (type, matchingBars) => new Bar
                                {
                                    Label = type.Label,
                                    Value = matchingBars.FirstOrDefault()?.Value ?? 0
                                }
                            ).ToList()
                    }).ToList();

                for (int yearIterator = 0; yearlyModeStartDate.AddYears(yearIterator) <= currentDate; yearIterator++)
                {
                    string newLabel = yearlyModeStartDate.AddYears(yearIterator).Year.ToString();
                    if (!chartData.Any(cd => cd.Label.Equals(newLabel)))
                        chartData.Insert(yearIterator, new BarChartData
                        {
                            Label = newLabel,
                            Bars = BarChartConstants.ALL_TRANSACTION_TYPES
                        });
                }

                return chartData;
            }
        }
        #endregion
        #endregion
        #endregion
    }
}
