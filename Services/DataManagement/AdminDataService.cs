using Data;
using Data.Constants;
using Data.Enums;
using Data.Repositories.Auth;
using Data.Repositories.Finance;
using Data.Repositories.JointEntity;
using Data.Repositories.User;
using Exceptions;
using Services.SessionsManagement;

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

        /// <summary>
        /// Asynchronously filters and retrieves a list of pending accounts based on optional criteria.
        /// </summary>
        /// <param name="accountNumber">Optional account number to filter by. If empty or null, this filter is ignored.</param>
        /// <param name="startDate">Optional start date to filter accounts opened on or after this date. If null, this filter is ignored.</param>
        /// <param name="endDate">Optional end date to filter accounts opened on or before this date. If null, this filter is ignored.</param>
        /// <param name="accountTypeId">Optional account type ID to filter by. Only applies if greater than zero.</param>
        /// <returns>A list of accounts matching the specified filters.</returns>
        public async Task<List<Account>> FilterPendingAccountsAsync(
            string accountNumber = "",
            DateTime? startDate = null,
            DateTime? endDate = null,
            int accountTypeId = 0
            )
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                //  Declare repository dependencies.
                var accountRepo = new AccountRepository(dbContext);

                //  Filter the accounts.
                var queryBuilder = accountRepo.Query;
                //  Get only the pending accounts.
                queryBuilder.HasAccountStatusTypeId((int)AccountStatusTypes.Pending);
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
                {
                    queryBuilder.HasAccountTypeId(accountTypeId);
                }

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
                        .HasAccessRoleId((int)AccessRoles.PRIMARY_OWNER)
                        .SelectUserInfoId();

                    //  Prepare a query for the user info table using the UserInfoId.
                    var userInfoQuery = userInfoRepo
                        .Query
                        .HasUserInfoId(userInfoId)
                        .IncludeUserName()
                        .IncludeFatherName()
                        .IncludeMotherName()
                        .IncludeReligion()
                        .GetQuery();

                    //  Execute the query. Throws UserNotFoundException if UserInfo is not found.
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

        /// <summary>
        /// Counts the number of transactions for each transaction type.
        /// </summary>
        /// <param name="transactionList">The list of <see cref="Transaction"/> objects to analyze.</param>
        /// <returns>
        /// A dictionary where the key is the transaction type ID and the value is the count of transactions of that type.
        /// </returns>
        public Dictionary<int, int> GetTransactionCounts(List<Transaction> transactionList)
        {
            Dictionary<int, int> transactionCounts = new();

            //  Convert the TransactionTypes enum to an IEnumerable.
            foreach (var typeId in Enum.GetValues<TransactionTypes>())
            {
                //  If you need to exclude incoming transfers.
                //if (typeId == TransactionTypes.Incoming_Transfer)
                //    continue;
                //  Get the count of elmeents matching the transaction type in the list
                //  and assign it to the new dictionary key.
                int count = transactionList.Count(t => t.TransactionTypeId == (int)typeId);
                transactionCounts[(int)typeId] = count;
            }

            return transactionCounts;
        }

        public List<Transaction> GetLargestTransactions(List<Transaction> transactionList, int count) =>
            transactionList.Where(t => t.TransactionTypeId != (int)TransactionTypes.Incoming_Transfer).Take(count).ToList();

        /// <summary>
        /// Retrieves a list of transactions filtered by an optional start and end date.
        /// </summary>
        /// <param name="transactionStartDate">The start date for filtering transactions. If null, no lower bound is applied.</param>
        /// <param name="transactionEndDate">The end date for filtering transactions. If null, no upper bound is applied.</param>
        /// <returns>A reversed list of <see cref="Transaction"/> entities matching the specified date range.</returns>
        /// <remarks>
        /// Transactions are ordered such that the most recent ones appear first.
        /// </remarks>
        public async Task<List<Transaction>> GetTransactionListAsync(DateTime? transactionStartDate = null, DateTime? transactionEndDate = null)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                //  Declare repository dependency.
                var transactionRepo = new TransactionRepository(dbContext);

                //  Compose Query
                var queryBuilder = transactionRepo
                    .Query
                    .IncludeMainAccount()
                    .OrderByDateAndTimeDescending();

                if (transactionStartDate is DateTime startDate)
                    queryBuilder.HasStartDate(startDate);
                if (transactionEndDate is DateTime endDate)
                    queryBuilder.HasEndDate(endDate);

                List<Transaction> transactions = await queryBuilder.GetQuery().ToListAsync();
                foreach (var transaction in transactions)
                {
                    string accountNumber = transaction.MainAccount.AccountNumber;
                    transaction.MainAccount.AccountNumber = _dataMaskingService.MaskAccountNumber(accountNumber);
                }

                return transactions;
            }
        }

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
    }
}
