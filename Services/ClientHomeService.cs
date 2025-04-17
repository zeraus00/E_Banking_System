using Data;
using Data.Repositories.Finance;
using Data.Repositories.User;


namespace Services
{
    public class ClientHomeService : Service
    {
        private UserInfoRepository _userInfoRepository;
        private AccountRepository _accountRepository;
        private UserInfo? _userInfo { get; set; }
        private Account? _account { get; set; }
        public ClientHomeService(EBankingContext context) : base(context)
        {
            _userInfoRepository = new UserInfoRepository(_context);
            _accountRepository = new AccountRepository(_context);
        }


        public void GetUserInfoSync(int userInfoId)
        {
            var query = _userInfoRepository.ComposeQuery(includeName: true);
            _userInfo = _userInfoRepository.GetUserInfoByIdSync(userInfoId, query);
        }
        public async Task GetUserInfoAsync(int userInfoId)
        {
            var query = _userInfoRepository.ComposeQuery(includeName: true);
            _userInfo = await _userInfoRepository.GetUserInfoByIdAsync(userInfoId, query);
        }
        public string GetUserFullName()
        {
            if(_userInfo == null)
            {
                throw new Exception();  // ex to be updated.
            }
            List<string?> names = new List<string?>
            {
                _userInfo.UserName.FirstName,
                _userInfo.UserName.MiddleName,
                _userInfo.UserName.LastName,
                _userInfo.UserName.Suffix
            };
            string fullName = string.Empty;

            foreach (var name in names)
            {
                fullName += name + " ";
            }

            return fullName.Trim();
        }
        
        public void GetAccountSync(int accountId)
        {
            _account = _accountRepository.GetAccountByIdSync(accountId);
        }
        
        public async Task GetAccountAsync(int accountId)
        {
            _account = await _accountRepository.GetAccountByIdAsync(accountId);
        }

        public string GetAccountNumber()
        {
            if(_account == null)
            {
                throw new Exception();  // ex to be updated.
            }

            return _account.AccountNumber;
        }
    }
}
