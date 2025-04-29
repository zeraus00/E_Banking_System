using Data;
using Data.Repositories.Auth;
using Data.Repositories.Finance;
using Data.Repositories.User;
using Exceptions;
using Microsoft.Identity.Client;


namespace Services
{
    /// <summary>
    /// Service for user data retrieval.
    /// </summary>
    public class UserDataService : Service
    {
        /// <summary>
        /// The constructor of the UserDataService.
        /// Injects an IDbContextFactory that generates a new dbContext connection
        /// for each method to avoid concurrency.
        /// </summary>
        /// <param name="contextFactory">The IDbContextFactory</param>
        public UserDataService(IDbContextFactory<EBankingContext> contextFactory) : base(contextFactory) { }

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
                var query = userAuthRepo.ComposeQuery(includeRole, includeAccounts, includeUserInfo);
                return (await userAuthRepo.GetUserAuthByIdAsync(userAuthId, query)) ?? throw new UserNotFoundException();
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
            bool includeReligion = false
            )
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                UserInfoRepository userInfoRepo = new UserInfoRepository(dbContext);

                IQueryable<UserInfo> query = userInfoRepo.ComposeQuery(
                        includeUserAuth,
                        includeUserName,
                        includeBirthInfo,
                        includeAddress,
                        includeFatherName,
                        includeMotherName,
                        includeReligion
                    );
                return (await userInfoRepo.GetUserInfoByIdAsync(userInfoId, query)) ?? throw new UserNotFoundException();
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

        public async Task<int?> GetFirstAccountAsync(int userAuthId)
        {
            var accountIdList = await this.GetAccountIdListAsync(userAuthId);
            return accountIdList?[0] ?? null;
        }
        /// <summary>
        /// Retrieves a list of accounts associated with the specified user authentication ID.
        /// </summary>
        /// <param name="userAuthId">The ID of the user authentication record to retrieve accounts for.</param>
        /// <returns>
        /// A list of <see cref="Account"/> objects if found; otherwise, <c>null</c> if the user authentication record does not exist.
        /// </returns>
        public async Task<List<Account>?> GetAccountListAsync(int userAuthId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                UserAuthRepository userAuthRepo = new UserAuthRepository(dbContext);

                var query = userAuthRepo.ComposeQuery(includeAccounts: true);
                var userAuth = await userAuthRepo.GetUserAuthByIdAsync(userAuthId, query);

                return userAuth?.Accounts.ToList() ?? null;
            }
        }
        public async Task<List<Account>> GetAccountListAsync(List<int> accountIdList)
        {
            List<Account> accountList = new();
            foreach (var id in accountIdList)
            {
                try
                {
                    //  Throws AccountNotFoundException if account is not found.
                    Account account = await this.GetAccountAsync(id);
                    accountList.Add(account);
                } 
                catch (AccountNotFoundException)
                {
                    //  Do not add accounts that are not found to the list.
                    continue;
                }
            }

            return accountList;
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
            int? transactionCount = null,
            int transactionTypeId = 0,
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
                    .ComposeQuery(includeTransactionType: true, includeMainAccount: true, includeCounterAccount: true);
                query = transactionRepository
                    .FilterQuery(query, transactionTypeId, transactionStartDate, transactionEndDate);
                

                //  Return the list of transactions.
                var transactionList = await transactionRepository.GetTransactionsAsListAsync(accountId, query);
                transactionList.Reverse();

                // Check if 'transactionCount' is an integer and greater than 0
                // If true, return the first 'count' number of elements from 'transactionList' as a list
                // If false (either 'transactionCount' is not an integer or count <= 0), return the entire 'transactionList' as is
                return transactionCount is int count  && count > 0
                    ? transactionList.Take(count).ToList() 
                    : transactionList;
            }
        }
        /// <summary>
        /// Asynchronously retrieves a list of the account ids associated with the user auth id.
        /// </summary>
        /// <param name="userAuthId">The id of the user's authentication details.</param>
        /// <returns>A list of the accounds associated witht the user.</returns>
        public async Task<List<int>?> GetAccountIdListAsync(int userAuthId)
        {
            List<int> accountIdList = new();
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                UserAuthRepository userAuthRepo = new UserAuthRepository(dbContext);

                var query = userAuthRepo.ComposeQuery(includeAccounts: true);
                var userAuth = await userAuthRepo.GetUserAuthByIdAsync(userAuthId, query);

                if (userAuth is null)
                {
                    return null;
                }

                foreach (var account in userAuth.Accounts)
                {
                    accountIdList.Add(account.AccountId);
                }

                return accountIdList;
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
            bool includeUsersAuth = false,
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
                bool noIncludes = !(includeAccountType && includeUsersAuth && includeTransactions && includeLoans);
                if (!noIncludes)
                {
                    query = accountRepo.Query
                        .IncludeAccountType(includeAccountType)
                        .IncludeUsersAuth(includeUsersAuth)
                        .IncludeMainTransactions(includeTransactions)
                        .IncludeLoans(includeLoans)
                        .GetQuery();
                }

                //  Return the result of the query.
                //  Throws AccountNotFoundException if no account is found.
                return (await accountRepo.GetAccountByIdAsync(accountId, query)) ?? throw new AccountNotFoundException(accountId);
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
            bool includeAccountType = false,
            bool includeUsersAuth = false,
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
                bool hasIncludes = (includeAccountType || includeUsersAuth || includeTransactions || includeLoans);
                if (hasIncludes)
                {
                    query = accountRepo.Query
                        .IncludeAccountType(includeAccountType)
                        .IncludeUsersAuth(includeUsersAuth)
                        .IncludeMainTransactions(includeTransactions)
                        .IncludeLoans(includeLoans)
                        .GetQuery();
                }

                //  Return the result of the query.
                //  Throws AccountNotFoundException if no account is found.
                return (await accountRepo.GetAccountByAccountNumberAsync(accountNumber, query)) ?? throw new AccountNotFoundException(accountNumber);
            }
        }
    }
}
