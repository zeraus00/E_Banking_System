using Exceptions;

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
        /// Methods to add new objects to the UserSchema
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

        public void AddRoleSync(Role role)
        {
            _context.Set<Role>().Add(role);
        }

        public async Task AddRoleAsync(Role role)
        {
            await _context.Set<Role>().AddAsync(role);
        }

        public UserAuth GetUserAuthByIdSync(int userAuthId)
        {
            var userAuth = _context.Set<UserAuth>().Find(userAuthId);

            if (userAuth == null)
            {
                throw new UserNotFoundException($"User with ID {userAuthId} not found.");
            }

            return userAuth;
        }

        public async Task<UserAuth> GetUserAuthByIdAsync(int userAuthId)
        {
            var userAuth = await _context.Set<UserAuth>().FindAsync(userAuthId);
            if (userAuth == null)
            {
                throw new UserNotFoundException($"User with ID {userAuthId} not found.");
            }
            return userAuth;
        }
        /// <summary>
        /// Methods for querying the UserSchema
        /// </summary>
        /// <param name="userNameOrEmail"></param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>

        public UserAuth GetUserAuthByUserNameOrEmailSync(string userNameOrEmail)
        {
            var trimmedUserNameOrEmail = userNameOrEmail.Trim();
            var userAuth = _context
                .Set<UserAuth>()
                .FirstOrDefault(
                    ua => ua.UserName == trimmedUserNameOrEmail ||
                          ua.Email == trimmedUserNameOrEmail
                );

            if (userAuth == null)
            {
                throw new UserNotFoundException($"User with username or email {userNameOrEmail} not found.");
            }
            return userAuth;
        }

        public async Task<UserAuth> GetUserAuthByUserNameOrEmailAsync(string userNameOrEmail)
        {
            var trimmedUserNameOrEmail = userNameOrEmail.Trim();
            var userAuth = await _context
                .Set<UserAuth>()
                .FirstOrDefaultAsync(
                    ua => ua.UserName == trimmedUserNameOrEmail ||
                          ua.Email == trimmedUserNameOrEmail
                );
            if (userAuth == null)
            {
                throw new UserNotFoundException($"User with username or email {userNameOrEmail} not found.");
            }
            return userAuth;
        }

        public UserAuth GetUserAuthByRoleIdSync(int roleId)
        {
            var userAuth = _context.Set<UserAuth>().Where(ua => ua.RoleId == roleId).ToList();
            if (userAuth == null)
            {
                throw new UserNotFoundException($"User with role ID {roleId} not found.");
            }
            return userAuth.FirstOrDefault();
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
            _userName = userName.Trim();
            return this;
        }

        public UserAuthBuilder WithEmail(string email)
        {
            _email = email.Trim();
            return this;
        }

        public UserAuthBuilder WithPassword(string password)
        {
            _password = password.Trim();
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
            _roleName = roleName.Trim();
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
