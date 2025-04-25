using Data;
using Data.Repositories.Auth;
using Data.Repositories.Finance;
using Data.Repositories.User;


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
        /// Get UserInfo through userInfoId.
        /// Includes the Name navigation property.
        /// </summary>
        /// <param name="userInfoId">The primary key of the UserInfo.</param>
        /// <returns>The UserInfo if found. Null otherwise.</returns>
        public UserInfo? GetUserInfoSync(int userInfoId)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                UserInfoRepository userInfoRepo = new UserInfoRepository(dbContext);

                IQueryable<UserInfo> query = userInfoRepo.ComposeQuery(includeName: true);
                UserInfo? userInfo = userInfoRepo.GetUserInfoByIdSync(userInfoId, query);
                
                return userInfo;
            }
        }

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
        /// The UserInfo instance MUST include the Name navigation property.
        /// </summary>
        /// <param name="userInfo">The UserInfo instance.</param>
        /// <returns>A string containing the user's full name. Null if the userInfo is null.</returns>
        public string? GetUserFullName(UserInfo? userInfo)
        {
            if(userInfo == null)
            {
                return null;  // ex to be updated.
            }
            List<string?> names = new List<string?>
            {
                userInfo.UserName.FirstName,
                userInfo.UserName.MiddleName,
                userInfo.UserName.LastName,
                userInfo.UserName.Suffix
            };
            string fullName = string.Empty;

            foreach (var name in names)
            {
                fullName += name + " ";
            }

            return fullName.Trim();
        }
        
        public int? GetFirstAccountSync(int userAuthId)
        {
            var accountIdList = this.GetAccountIdListSync(userAuthId);
            return accountIdList?[0] ?? null;
        }

        public async Task<int?> GetFirstAccountAsync(int userAuthId)
        {
            var accountIdList = await this.GetAccountIdListAsync(userAuthId);
            return accountIdList?[0] ?? null;
        }

        public List<Account>? GetAccountListSync(int userAuthId)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                UserAuthRepository userAuthRepo = new UserAuthRepository(dbContext);

                var query = userAuthRepo.ComposeQuery(includeAccounts: true);
                var userAuth = userAuthRepo.GetUserAuthByIdSync(userAuthId, query);

                return userAuth?.Accounts.ToList() ?? null;
            }
        }

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
        /// Retrieves a list of the account ids associated with the user auth id.
        /// </summary>
        /// <param name="userAuthId">The id of the user's authentication details.</param>
        /// <returns>A list of the accounds associated witht the user.</returns>
        public List<int>? GetAccountIdListSync(int userAuthId)
        {
            List<int> accountIdList = new();
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                UserAuthRepository userAuthRepo = new UserAuthRepository(dbContext);

                var query = userAuthRepo.ComposeQuery(includeAccounts: true);
                var userAuth = userAuthRepo.GetUserAuthByIdSync(userAuthId, query);
                
                if (userAuth is null)
                {
                    return null;
                }

                foreach(var account in userAuth.Accounts)
                {
                    accountIdList.Add(account.AccountId);
                }

                return accountIdList;
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
        /// Retrieves an account by its account id.
        /// </summary>
        /// <param name="accountId">The account's id/primary key.</param>
        /// <returns>The account if found.</returns>

        public Account? GetAccountSync(int accountId)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                AccountRepository accountRepo = new AccountRepository(dbContext);


                IQueryable<Account> query = accountRepo.ComposeAccountQuery(includeTransactions: true);
                Account? account = accountRepo.GetAccountByIdSync(accountId, query);

                return account;
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


                IQueryable<Account> query = accountRepo.ComposeAccountQuery(includeTransactions: true);
                Account? account = await accountRepo.GetAccountByIdAsync(accountId, query);

                return account;
            }
        }

        /// <summary>
        /// Retrieves an account by its account number.
        /// </summary>
        /// <param name="accountNumber">The account number.</param>
        /// <returns>The account if found.</returns>
        public Account? GetAccountSync(string accountNumber)
        {
            using (var dbContext = _contextFactory.CreateDbContext())
            {
                AccountRepository accountRepo = new AccountRepository(dbContext);

                IQueryable<Account> query = accountRepo.ComposeAccountQuery(includeTransactions: true);
                Account? account = accountRepo.GetAccountByAccountNumberSync(accountNumber, query);
                
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

                IQueryable<Account> query = accountRepo.ComposeAccountQuery(includeTransactions: true);
                Account? account = await accountRepo.GetAccountByAccountNumberAsync(accountNumber, query);

                return account;
            }
        }

    }
}
