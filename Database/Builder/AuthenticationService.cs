namespace Database.Builder
{
    public class AuthenticationService(DbContext context)
    {
        DbContext _context = context;

        public void AddCustomerAuth(CustomerAuth customerAuth)
        {
            _context.Set<CustomerAuth>().Add(customerAuth);
            _context.SaveChanges();
        }
    }

    public class CustomerAuthBuilder 
    {
        private int _accountId;
        private int _userInfoId;
        private string _userName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;

        public CustomerAuthBuilder WithAccountId(int accountId)
        {
            _accountId = accountId;
            return this;
        }

        public CustomerAuthBuilder WithUserInfoId(int userInfoId)
        {
            _userInfoId = userInfoId;
            return this;
        }

        public CustomerAuthBuilder WithUserName(string userName)
        {
            _userName = userName;
            return this;
        }

        public CustomerAuthBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public CustomerAuthBuilder WithPassword(string password)
        {
            _password = password;
            return this;
        }

        public CustomerAuth Build()
        {
            return new CustomerAuth
            {
                AccountId = _accountId,
                UserInfoId = _userInfoId,
                UserName = _userName,
                Email = _email,
                Password = _password
            };
        }
    }

}
