using Data;
using Data.Repositories.Finance;
using Data.Repositories.User;


namespace Services
{
    public class ClientHomeService : Service
    {
        private UserInfoRepository _userInfoRepository;
        private AccountRepository _accountRepository;
        public ClientHomeService(EBankingContext context) : base(context)
        {
            _userInfoRepository = new UserInfoRepository(_context);
            _accountRepository = new AccountRepository(_context);
        }


        public UserInfo? GetUserInfoSync(int userInfoId)
        {
            var query = _userInfoRepository.ComposeQuery(includeName: true);
            var userInfo = _userInfoRepository.GetUserInfoByIdSync(userInfoId, query);
            return userInfo;
        }
        public async Task<UserInfo?> GetUserInfoAsync(int userInfoId)
        {
            var query = _userInfoRepository.ComposeQuery(includeName: true);
            var userInfo = await _userInfoRepository.GetUserInfoByIdAsync(userInfoId, query);
            return userInfo;
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
            var account = _accountRepository.GetAccountByIdSync(accountId);
            return account;
        }
        
        public async Task<Account?> GetAccountAsync(int accountId)
        {
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            return account;
        }

    }
}
