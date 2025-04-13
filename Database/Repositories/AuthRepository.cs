namespace Database.Repositories
{
    /// <summary>
    /// CRUD operations handler for AuthSchema
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// </summary>
    public class AuthRepository
    {

        private readonly DbContext _context;
        public AuthRepository(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        public void saveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a new UserAuth to the database
        /// </summary>
        /// <param name="userAuth"></param>
        public void AddUserAuthSync(UserAuth userAuth)
        {
            _context.Set<UserAuth>().Add(userAuth);
        }

        public async Task AddUserAuthAsync(UserAuth userAuth)
        {
            await _context.Set<UserAuth>().AddAsync(userAuth);
        }

        /// <summary>
        /// Adds a new Role to the database
        /// </summary>
        /// <param name="role"></param>
        public void AddRoleSync(Role role)
        {
            _context.Set<Role>().Add(role);
        }

        public async Task AddRoleAsync(Role role)
        {
            await _context.Set<Role>().AddAsync(role);
        }


    }
    /// <summary>
    /// Builder class for UserAuth
    /// Methods for setting properties of UserAuth
    /// </summary>
    public class UserAuthBuilder
    {
        private int _roleId;
        private int? _accountId;
        private int? _userInfoId;
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

        /// <summary>
        /// Builds the UserAuth object with the specified properties
        /// </summary>
        /// <returns> UserAuth object </returns>
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

    /// <summary>
    /// Builder class for Role
    /// Methods for setting properties of Role
    /// </summary>
    public class RoleBuilder
    {
        private string _roleName = string.Empty;

        public RoleBuilder WithRoleName(string roleName)
        {
            _roleName = roleName;
            return this;
        }

        /// <summary>
        /// Builds the Role object with the specified properties
        /// </summary>
        /// <returns> Role Object </returns>
        public Role Build()
        {
            return new Role
            {
                RoleName = _roleName
            };
        }
    }
}
