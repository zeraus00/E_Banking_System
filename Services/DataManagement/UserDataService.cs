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
        /// Attempts to retrieve a UserAuth from the database with specified includes.
        /// Throws an error if no userauth is found.
        /// </summary>
        /// <param name="userAuthId">The primary key of the UserAuth.</param>
        /// <param name="includeRole">Decide wheter to include the Role.</param>
        /// <param name="includeAccounts">Decide wheter to include the Accounts.</param>
        /// <param name="includeUserInfo">Decide wheter to include the UserInfo.</param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        public async Task<UserAuth> TryGetUserAuthAsync(
            int userAuthId,
            bool includeRole = false,
            bool includeAccounts = false,
            bool includeUserInfo = false)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var userAuthRepo = new UserAuthRepository(dbContext);
                return await userAuthRepo
                    .Query
                    .HasUserAuthId(userAuthId)
                    .IncludeRole(includeRole)
                    .IncludeUserInfo(includeUserInfo)
                    .GetQuery()
                    .FirstOrDefaultAsync()
                    ?? throw new UserNotFoundException();
            }
        }

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
        /// <returns>The UserInfo if found. Null otherwise.</returns>
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

                var queryBuilder = userInfoRepo.Query;

                if (includeUserAuth)
                    queryBuilder.IncludeUserAuth();
                if (includeUserName)
                    queryBuilder.IncludeUserName();
                if (includeBirthInfo)
                    queryBuilder.IncludeBirthInfo();
                if (includeAddress)
                    queryBuilder.IncludeAddress();
                if (includeFatherName)
                    queryBuilder.IncludeFatherName();
                if (includeMotherName)
                    queryBuilder.IncludeMotherName();
                if (includeReligion)
                    queryBuilder.IncludeReligion();
                if (includeUserInfoAccounts)
                    queryBuilder.IncludeUserInfoAccounts();

                var query = queryBuilder.GetQuery();

                return await userInfoRepo.GetUserInfoByIdAsync(userInfoId, query) ?? throw new UserNotFoundException();
            }
        }
        public async Task<Address> TryGetAddressAsync(int addressId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var addressRepo = new AddressRepository(dbContext);

                var query = addressRepo.Query
                    .HasAddressId(addressId)
                    .IncludeRegion()
                    .IncludeProvince()
                    .IncludeCity()
                    .IncludeBarangay()
                    .GetQuery();

                return await query.FirstOrDefaultAsync() ?? throw new NullReferenceException("NO SUCH ADDRESS");
            }
        }
        public async Task<BirthInfo> TryGetBirthInfoAsync(int birthInfoId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var birthInfoRepo = new BirthInfoRepository(dbContext);
                var query = birthInfoRepo
                    .Query
                    .HasBirthInfoId(birthInfoId)
                    .IncludeRegion()
                    .IncludeProvince()
                    .IncludeCity()
                    .GetQuery();

                return await query.FirstOrDefaultAsync() ?? throw new NullReferenceException();
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
        /// Retrieves a list of transactions associated with the specified account ID.
        /// Includes parameters for filtering the query.
        /// </summary>
        /// <param name="accountId">The ID of the account whose transactions are to be retrieved.</param>
        /// <param name="transactionCount">The number of entries to be taken from the list.</param>
        /// <param name="transactionTypeId">The ID of the type of transactions to be retrieved.</param>
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

                //  Include TransactionType navigation property in query.
                var query = transactionRepository
                    .ComposeQuery(includeTransactionType: true, includeMainAccount: true, includeCounterAccount: true, includeExternalVendor: true);
                query = transactionRepository
                    .FilterQuery(query, transactionTypeId, transactionStartDate, transactionEndDate);


                //  Return the list of transactions.
                int skipCount = (pageNumber - 1) * pageSize;
                int takeCount = pageSize;
                var transactionList = await transactionRepository.GetRecentTransactionsAsListAsync(accountId, skipCount, takeCount, query);

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
            bool isAccountLinkedOnline = true)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                try
                {
                    var userInfoAccountRepo = new UserInfoAccountRepository(dbContext);

                    return await userInfoAccountRepo
                        .Query
                        .HasUserInfoId(userInfoId)
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
        /// <param name="includeUsersAuth">Specifies whether to include the associated UsersAuth. Default is false.</param>
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
