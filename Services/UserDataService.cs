using Data;
using Data.Repositories.Finance;
using Data.Repositories.User;


namespace Services
{
    public class UserDataService : Service
    {
        public UserDataService(IDbContextFactory<EBankingContext> contextFactory) : base(contextFactory) { }

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
