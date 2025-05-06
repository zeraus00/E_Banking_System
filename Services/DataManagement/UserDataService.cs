using Data;
using Data.Models.User;
using Data.Repositories.Auth;
using Data.Repositories.Finance;
using Data.Repositories.JointEntity;
using Data.Repositories.User;
using Exceptions;
using Microsoft.Identity.Client;


namespace Services.DataManagement
{
    /// <summary>
    /// Service for user data retrieval.
    /// </summary>
    public class UserDataService : Service
    {
        private readonly DataMaskingService _dataMaskingService;
        /// <summary>
        /// The constructor of the UserDataService.
        /// Injects an IDbContextFactory that generates a new dbContext connection
        /// for each method to avoid concurrency.
        /// </summary>
        /// <param name="contextFactory">The IDbContextFactory</param>
        public UserDataService(IDbContextFactory<EBankingContext> contextFactory, DataMaskingService dataMaskingService) : base(contextFactory)
        {
            _dataMaskingService = dataMaskingService;
        }
        /// <summary>
        /// Get the user email through UserAuthId.
        /// </summary>
        /// <param name="userAuthId">The id from the UsersAuth table.</param>
        /// <returns> A string containing the user's email </returns>
        /// <exception cref="UserNotFoundException">
        /// Thrown when no UserAuth with the specified userAuthId is found.
        /// </exception>
        public async Task<string> TryGetUserAuthEmail(int userAuthId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var userAuthRepo = new UserAuthRepository(dbContext);
                return await userAuthRepo
                    .Query
                    .HasUserAuthId(userAuthId)
                    .SelectEmail()
                    ?? throw new UserNotFoundException();
            }
        }

        /// <summary>
        /// Get UserInfo through userInfoId asynchronously with specified includes.
        /// </summary>
        /// <param name="userInfoId">The primary key of the UserInfo.</param>
        /// <param name="includeUserAuth">Decides whether UserAuth is included.</param>
        /// <param name="includeUserName">Decides whether UserName is included.</param>
        /// <param name="includeBirthInfo">Decides whether BirthInfo is incldued.</param>
        /// <param name="includeAddress">Decides whether Address is included</param>
        /// <param name="includeFatherName">Decides whether FatherName is included.</param>
        /// <param name="includeMotherName">Decides whether MotherName is included.</param>
        /// <param name="includeReligion">Decides whether Religion is included.</param>
        /// <param name="includeUserInfoAccounts">Decides whether UserInfoAccounts is included.</param>
        /// <returns>The UserInfo if found. Null otherwise.</returns>
        /// <exception cref="UserNotFoundException"></exception>
        public async Task<UserInfo> TryGetUserInfoAsync(
            int userInfoId,
            bool includeUserAuth = false,
            bool includeUserName = false,
            bool includeBirthInfo = false,
            bool includeAddress = false,
            bool includeFatherName = false,
            bool includeMotherName = false,
            bool includeReligion = false,
            bool includeUserInfoAccounts = false
            )
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                UserInfoRepository userInfoRepo = new UserInfoRepository(dbContext);

                //  Build a query with the specified conditions.
                var queryBuilder = userInfoRepo
                    .Query
                    .IncludeUserAuth(includeUserAuth)
                    .IncludeUserName(includeUserName)
                    .IncludeBirthInfo(includeBirthInfo)
                    .IncludeAddress(includeAddress)
                    .IncludeFatherName(includeFatherName)
                    .IncludeMotherName(includeMotherName)
                    .IncludeReligion(includeReligion)
                    .IncludeUserInfoAccounts(includeUserInfoAccounts);

                var query = queryBuilder.GetQuery();

                return await userInfoRepo.GetUserInfoByIdAsync(userInfoId, query) ?? throw new UserNotFoundException();
            }
        }

        /// <summary>
        /// Get Address with the specified addressId.
        /// Includes navigation properties of the Regions, Provinces, Cities, and Barangays tables.
        /// </summary>
        /// <param name="addressId">The id of the address in the addresses table.</param>
        /// <returns>The Address found through the query.</returns>
        /// <exception cref="NullReferenceException">
        /// Thrown when no address with the specified id is found.
        /// </exception>
        public async Task<Address> TryGetAddressAsync(int addressId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var addressRepo = new AddressRepository(dbContext);

                return await addressRepo.Query
                    .HasAddressId(addressId)
                    .IncludeRegion()
                    .IncludeProvince()
                    .IncludeCity()
                    .IncludeBarangay()
                    .GetQuery()
                    .FirstOrDefaultAsync()
                    ?? throw new NullReferenceException("NO SUCH ADDRESS");
            }
        }
        /// <summary>
        /// Get BirthInfo with the specified birthInfoId.
        /// Includes navigation properties of the Regions, Provinces, and Cities tables.
        /// </summary>
        /// <param name="birthInfoId">The id of the BirthInfo in the BirthsInfo table.</param>
        /// <returns>The BirthInfo found through the query.</returns>
        /// <exception cref="NullReferenceException">
        /// Throw when no BirthInfo with the specified id is found.
        /// </exception>
        public async Task<BirthInfo> TryGetBirthInfoAsync(int birthInfoId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var birthInfoRepo = new BirthInfoRepository(dbContext);
                return await birthInfoRepo
                    .Query
                    .HasBirthInfoId(birthInfoId)
                    .IncludeRegion()
                    .IncludeProvince()
                    .IncludeCity()
                    .GetQuery()
                    .FirstOrDefaultAsync()
                    ?? throw new NullReferenceException();
            }
        }
        /// <summary>
        /// Gets the user's full name from the UserInfo object.
        /// If the UserInfo does not include the Name navigation property, queries the database using
        /// the NameId provided in the UserInfo.
        /// </summary>
        /// <param name="userInfo">The UserInfo instance.</param>
        /// <returns>A string containing the user's full name. Null if the userInfo or Name is null.</returns>
        public async Task<string?> GetUserFullName(UserInfo? userInfo)
        {
            //  If userInfo is null, return null.
            if (userInfo is null)
            {
                return null;
            }

            //  Initialize a list for the name parts.
            List<string?> names = new();

            //  Check if the navigation property of UserName is null.
            //  If null, query the database for the Name using the NameId.
            //  If not null, use the navigation property.
            //  Fill out the list with the name's parts.
            if (userInfo.UserName is null)
            {
                await using (var dbContext = await _contextFactory.CreateDbContextAsync())
                {
                    //  Query the names table.
                    var nameRepo = new NameRepository(dbContext);
                    Name? name = await nameRepo.GetNameByIdAsync(userInfo.UserNameId);
                    //  return null if name is null.
                    if (name is null)
                        return null;
                    else
                    {
                        names = new List<string?>
                        {
                            name.FirstName,
                            name.MiddleName,
                            name.LastName,
                            name.Suffix
                        };
                    }
                }
            }
            else
            {

                names = new List<string?>
                {
                    userInfo.UserName.FirstName,
                    userInfo.UserName.MiddleName,
                    userInfo.UserName.LastName,
                    userInfo.UserName.Suffix
                };
            }

            //  Concatenate the name parts into one string and return.
            string fullName = string.Empty;

            foreach (var name in names)
            {
                fullName += name + " ";
            }

            return fullName.Trim();
        }
        /// <summary>
        /// Retrieves a bulk list of transactions associated with the specified account ID.
        /// Includes parameters for filtering the query.
        /// </summary>
        /// <param name="accountId">The ID of the account whose transactions are to be retrieved.</param>
        /// <param name="transactionTypeId">The ID of the type of transactions to be retrieved.</param>
        /// <param name="pageSize">The number of transactions to retrieve per page. Used for pagination.</param>
        /// <param name="pageNumber">The page number of the transaction results to retrieve. Starts at 1.</param>
        /// <param name="transactionStartDate">The earliest date of transactions to be retrieved.</param>
        /// <param name="transactionEndDate">The latest date of the transactions to be retrieved.</param>
        /// <returns>
        /// A list of <see cref="Transaction"/> objects associated with the given account.
        /// </returns>
        public async Task<List<Transaction>> GetRecentAccountTransactionsAsync(
            int accountId,
            int transactionTypeId = 0,
            int pageSize = 50,
            int pageNumber = 1,
            DateTime? transactionStartDate = null,
            DateTime? transactionEndDate = null
            )
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                //  Define repository requirements
                var transactionRepository = new TransactionRepository(dbContext);

                //  Return the list of transactions.
                int skipCount = (pageNumber - 1) * pageSize;
                int takeCount = pageSize;

                var queryBuilder = transactionRepository
                    .Query
                    .HasMainAccountId(accountId)
                    .SkipBy(skipCount)
                    .TakeWithCount(takeCount);

                if (transactionTypeId > 0)
                    queryBuilder.HasTransactionTypeId(transactionTypeId);
                if (transactionStartDate is DateTime startDate)
                    queryBuilder.HasStartDate(startDate);
                if (transactionEndDate is DateTime endDate)
                    queryBuilder.HasEndDate(endDate);


                var transactionList = await queryBuilder.GetQuery().ToListAsync();

                if (transactionList.Any())
                {
                    foreach (var transaction in transactionList)
                    {
                        transaction.MainAccount.AccountNumber = _dataMaskingService.MaskAccountNumber(transaction.MainAccount.AccountNumber);
                        if (transaction.CounterAccount is not null)
                            transaction.CounterAccount.AccountNumber = _dataMaskingService.MaskAccountNumber(transaction.CounterAccount.AccountNumber);
                    }
                }

                return transactionList;
            }
        }

        /// <summary>
        /// Used in conjunction with <see cref="GetRecentAccountTransactionsAsync(int, int, int, int, DateTime?, DateTime?)"/>
        /// Further divides the bulk list into pages to be shown in Transactions Page.
        /// Retrieves a page (or sublist) of transactions from the main bulk list to be displayed in
        /// smaller pages.
        /// </summary>
        /// <param name="transactionList">The full list of transactions retrieved from the database.</param>
        /// <param name="pageNumber">The page number to retrieve from the full list. Starts at 1.</param>
        /// <param name="pageSize">The number of transactions per page. Defaults to 10.</param>
        /// <returns>
        /// A sublist of <see cref="Transaction"/> objects representing a single page of transactions from the bulk list.
        /// </returns>
        public List<Transaction> GetTransactionListPage(List<Transaction> transactionList, int pageNumber, int pageSize = 10)
        {
            int skipCount = (pageNumber - 1) * pageSize;
            int takeCount = pageSize;
            return transactionList.Skip(skipCount).Take(takeCount).ToList();
        }

        public async Task<List<UserInfoAccount>> GetUserAccountLinks(
            int userInfoId, 
            bool includeUserInfo = false,
            bool includeAccount = false,
            bool isLinkedToOnlineAccount = false)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                try
                {
                    var userInfoAccountRepo = new UserInfoAccountRepository(dbContext);

                    return await userInfoAccountRepo
                        .Query
                        .HasUserInfoId(userInfoId)
                        .HasLinkToOnlineAccount(isLinkedToOnlineAccount)
                        .IncludeUserInfo(includeUserInfo)
                        .IncludeAccount(includeAccount)
                        .GetQuery()
                        .ToListAsync();
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Asynchronously retrieves an account by its account id with the specified includes.
        /// </summary>
        /// <param name="accountId">The account's id/primary key.</param>
        /// <returns>The account if found.</returns>
        /// <exception cref="AccountNotFoundException">Thrown if no account is found.</exception>
        public async Task<Account> GetAccountAsync(
            int accountId,
            bool includeAccountType = false,
            bool includeUsersInfoAccount = false,
            bool includeTransactions = false,
            bool includeLoans = false
            )
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                //  Declare repository dependencies.
                AccountRepository accountRepo = new AccountRepository(dbContext);

                //  Get query.
                IQueryable<Account>? query = null;

                //  Check if there are no includes.
                bool noIncludes = !(includeAccountType && includeTransactions && includeLoans);
                if (!noIncludes)
                {
                    query = accountRepo.Query
                        .IncludeAccountType(includeAccountType)
                        .IncludeUsersInfoAccount(includeUsersInfoAccount)
                        .IncludeMainTransactions(includeTransactions)
                        .IncludeLoans(includeLoans)
                        .GetQuery();

                    return await accountRepo.GetAccountByIdAsync(accountId, query) ?? throw new AccountNotFoundException(accountId);
                }

                //  Return the result of the query.
                //  Throws AccountNotFoundException if no account is found.
                return await accountRepo.GetAccountByIdAsync(accountId) ?? throw new AccountNotFoundException(accountId);
            }
        }

        /// <summary>
        /// Asynchronously retrieves an account by its account number with the specified includes.
        /// </summary>
        /// <param name="accountNumber">The account number to search for.</param>
        /// <param name="includeAccountType">Specifies whether to include the associated AccountType. Default is false.</param>
        /// <param name="includeTransactions">Specifies whether to include the associated transactions. Default is false.</param>
        /// <param name="includeLoans">Specifies whether to include the associated loans. Default is false.</param>
        /// <returns>
        /// The account if found, otherwise throws an <see cref="AccountNotFoundException"/>.
        /// </returns>
        /// <exception cref="AccountNotFoundException">
        /// Thrown if no account with the specified account number is found.
        /// </exception>
        public async Task<Account?> GetAccountAsync(
            string accountNumber,
            string? accountName = null,
            bool includeAccountType = false,
            bool includeTransactions = false,
            bool includeLoans = false
            )
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                //  Declare repository dependencies.
                AccountRepository accountRepo = new AccountRepository(dbContext);

                var queryBuilder = accountRepo.Query
                    .IncludeAccountType(includeAccountType)
                    .IncludeMainTransactions(includeTransactions)
                    .IncludeLoans(includeLoans)
                    .HasAccountNumber(accountNumber);

                if (accountName is not null)
                    queryBuilder.HasAccountName(accountName);

                return await queryBuilder.GetQuery().FirstOrDefaultAsync();
            }
        }
        public async Task<bool> HasUserLinkedAccount(int userInfoId, int accountId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var userInfoAccountRepo = new UserInfoAccountRepository(dbContext);

                return await userInfoAccountRepo.IsUserAccountLinkExists(userInfoId, accountId);
            }
        }
        public async Task<int> GetAccountIdAsync(string accountNumber, string accountName, int accountTypeId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var accountRepo = new AccountRepository(dbContext);

                //  Returns 0 if no account is found.
                return await accountRepo
                    .Query
                    .HasAccountNumber(accountNumber)
                    .HasAccountName(accountName)
                    .HasAccountTypeId(accountTypeId)
                    .SelectId();
            }
        }

        public async Task<decimal> GetAccountBalanceAsync(int accountId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var accountRepo = new AccountRepository(dbContext);

                return await accountRepo.Query.HasAccountId(accountId).SelectBalance();
            }
        }
    }
}
