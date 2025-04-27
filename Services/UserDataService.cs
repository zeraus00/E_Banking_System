using Data;
using Data.Repositories.Auth;
using Data.Repositories.Finance;
using Data.Repositories.User;
using Exceptions;


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
        /// Get UserInfo through userInfoId asynchronously.
        /// Includes the Name navigation property.
        /// </summary>
        /// <param name="userInfoId">The primary key of the UserInfo.</param>
        /// <returns>The UserInfo if found. Null otherwise.</returns>
        public async Task<UserInfo?> GetUserInfoAsync(int userInfoId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                UserInfoRepository userInfoRepo = new UserInfoRepository(dbContext);

                IQueryable<UserInfo> query = userInfoRepo.ComposeQuery(includeName: true);
                UserInfo? userInfo = await userInfoRepo.GetUserInfoByIdAsync(userInfoId, query);

                return userInfo;
            }
        }


        /// <summary>
        /// Gets the user's full name from the UserInfo object.
        /// If the UserInfo does not include the Name navigation property, queries the database using
        /// the NameId provided in the UserInfo.
        /// </summary>
        /// <param name="userInfo">The UserInfo instance.</param>
        /// <returns>A string containing the user's full name. Null if the userInfo is null.</returns>
        public async Task<string> GetUserFullName(UserInfo? userInfo)
        {
            //  If userInfo is null, throws a UserNotFoundException.
            if(userInfo is null)
            {
                throw new UserNotFoundException(); 
            }

            //  Initialize a list for the name parts.
            List<string?> names = new();

            //  Check if the navigation property of UserName is null.
            //  If null, query the database for the Name using the NameId.
            //  If not null, use the navigation property.
            //  Fill out the list with the name's parts.
            if(userInfo.UserName is null)
            {
                await using (var dbContext = await _contextFactory.CreateDbContextAsync()) 
                {
                    var nameRepo = new NameRepository(dbContext);
                    Name? name = await nameRepo.GetNameByIdAsync(userInfo.UserNameId);
                    if (name is null)
                        throw new NameNotFoundException(userInfo.UserNameId);
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
            } else
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
        /// <summary>
        /// Retrieves a list of transactions associated with the specified account ID.
        /// </summary>
        /// <param name="accountId">The ID of the account whose transactions are to be retrieved.</param>
        /// <returns>
        /// A list of <see cref="Transaction"/> objects associated with the given account.
        /// </returns>
        public async Task<List<Transaction>> GetRecentAccountTransactionsAsync(int accountId, int transactionCount)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                //  Define repository requirements
                var transactionRepository = new TransactionRepository(dbContext);

                //  Include TransactionType navigation property in query.
                var query = transactionRepository.ComposeQuery(includeTransactionType: true, includeMainAccount: true);

                //  Return the list of transactions.
                var transactionList = await transactionRepository.GetTransactionsAsListAsync(accountId, query);
                transactionList.Reverse();
                ;
                return transactionList.Take(transactionCount).ToList();
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
        /// Asynchronously retrieves an account by its account id.
        /// </summary>
        /// <param name="accountId">The account's id/primary key.</param>
        /// <returns>The account if found.</returns>
        public async Task<Account?> GetAccountAsync(int accountId)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                AccountRepository accountRepo = new AccountRepository(dbContext);


                IQueryable<Account> query = accountRepo.ComposeQuery(includeTransactions: true);
                Account? account = await accountRepo.GetAccountByIdAsync(accountId, query);

                return account;
            }
        }

        /// <summary>
        /// Asynchronously retrieves an account by its account number.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <returns>The account if found.</returns>
        public async Task<Account?> GetAccountAsync(string accountNumber)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                AccountRepository accountRepo = new AccountRepository(dbContext);

                IQueryable<Account> query = accountRepo.ComposeQuery(includeTransactions: true);
                Account? account = await accountRepo.GetAccountByAccountNumberAsync(accountNumber, query);

                return account;
            }
        }

    }
}
