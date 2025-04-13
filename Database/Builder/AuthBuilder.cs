namespace Database.Builder
{
    public class AuthBuilder
    {

        private readonly DbContext _context;
        public AuthBuilder(DbContext context)
        {
            _context = context;
        }

        public void AddUserAuthSync(UserAuth userAuth)
        {
            _context.Set<UserAuth>().Add(userAuth);
            _context.SaveChanges();
        }

        public async Task AddUserAuthAsync(UserAuth userAuth)
        {
            await _context.Set<UserAuth>().AddAsync(userAuth);
            await _context.SaveChangesAsync();
        }

        public void AddRoleSync(Role role)
        {
            _context.Set<Role>().Add(role);
            _context.SaveChanges();
        }

        public async Task AddRoleAsync(Role role)
        {
            await _context.Set<Role>().AddAsync(role);
            await _context.SaveChangesAsync();
        }
    }

    public class UserAuthBuilder
    {
        private int _roleId;
        private int _accountId;
        private int _userInfoId;
        private string _userName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;

        public UserAuthBuilder WithRoleId(int roleId)
        {
            _roleId = roleId;
            return this;
        }

        public UserAuthBuilder WithAccountId(int accountId)
        {
            _accountId = accountId;
            return this;
        }

        public UserAuthBuilder WithUserInfoId(int userInfoId)
        {
            _userInfoId = userInfoId;
            return this;
        }

        public UserAuthBuilder WithUserName(string userName)
        {
            _userName = userName;
            return this;
        }

        public UserAuthBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public UserAuthBuilder WithPassword(string password)
        {
            _password = password;
            return this;
        }

        public UserAuth Build()
        {
            return new UserAuth
            {
                RoleId = _roleId,
                AccountId = _accountId,
                UserInfoId = _userInfoId,
                UserName = _userName,
                Email = _email,
                Password = _password
            };
        }
    }

    public class RoleBuilder
    {
        private string _roleName = string.Empty;

        public RoleBuilder WithRoleName(string roleName)
        {
            _roleName = roleName;
            return this;
        }

        public Role Build()
        {
            return new Role
            {
                RoleName = _roleName
            };
        }
    }
}
