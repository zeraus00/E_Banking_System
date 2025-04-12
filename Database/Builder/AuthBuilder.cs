namespace Database.Builder
{
    public class AuthBuilder
    {

        private readonly DbContext _context;
        public AuthBuilder(DbContext context)
        {
            _context = context;
        }

        public void AddCustomerAuth(CustomerAuth customerAuth)
        {
            _context.Set<CustomerAuth>().Add(customerAuth);
            _context.SaveChanges();
        }
        public async Task AddCustomerAuthAsync(CustomerAuth customerAuth)
        {
            await _context.Set<CustomerAuth>().AddAsync(customerAuth);
            await _context.SaveChangesAsync();
        }

        public void AddEmployeeAuth(EmployeeAuth employeeAuth)
        {
            _context.Set<EmployeeAuth>().Add(employeeAuth);
            _context.SaveChanges();
        }

        public async Task AddEmployeeAuthAsync(EmployeeAuth employeeAuth)
        {
            await _context.Set<EmployeeAuth>().AddAsync(employeeAuth);
            await _context.SaveChangesAsync();
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


    public class EmployeeAuthBuilder
    {
        private string _userName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;

        public EmployeeAuthBuilder WithUserName(string userName)
        {
            _userName = userName;
            return this;
        }

        public EmployeeAuthBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public EmployeeAuthBuilder WithPassword(string password)
        {
            _password = password;
            return this;
        }

        public EmployeeAuth Build()
        {
            return new EmployeeAuth
            {
                UserName = _userName,
                Email = _email,
                Password = _password
            };
        }
    }
}
