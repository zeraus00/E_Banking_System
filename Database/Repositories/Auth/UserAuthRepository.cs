using Exceptions;
using Data;

namespace Database.Repositories.Auth
{
    /// <summary>
    /// Class for handling CRUD operations in UsersAuth table.
    /// Contains both sync and async versions of each method.
    /// 
    /// NOTE: There is no persistent saving here. You MUST call
    /// DbContext.SaveChanges or DbContext.SaveChangesAsync externally.
    /// </summary>
    public class UserAuthRepository
    {
        private readonly EBankingContext _context;

        public UserAuthRepository (EBankingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add a new entry to the UserAuths Table
        /// </summary>
        /// <param name="userAuth"></param>
        /// 
        public void AddUserAuthSync(UserAuth userAuth)
        {
            _context.Set<UserAuth>().Add(userAuth);
        }
        /// <summary>
        /// Add a new entry to the UserAuths Table
        /// </summary>
        /// <param name="userAuth"></param>
        /// 
        public async Task AddUserAuthAsync(UserAuth userAuth)
        {
            await _context.Set<UserAuth>().AddAsync(userAuth);
        }

        /// <summary>
        /// Query entries by primary key in UserAuth Table
        /// </summary>
        /// <param name="userAuthId">The primary key.</param>
        /// <param name="includeRole">Set to true to include Role nav property</param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        public UserAuth GetUserAuthByIdSync(int userAuthId, bool includeRole = false)
        {
            UserAuth? userAuth;

            if (includeRole)
            {
                userAuth = _context
                    .Set<UserAuth>()
                    .Include(ua => ua.Role)
                    .FirstOrDefault(ua => ua.UserAuthId == userAuthId);
            } else
            {
                userAuth = _context
                    .Set<UserAuth>()
                    .Find(userAuthId);
            }

            if (userAuth == null)
            {
                throw new UserNotFoundException($"User with ID {userAuthId} not found.");
            }

            return userAuth;
        }
        /// <summary>
        /// Query entries by primary key in UserAuth Table
        /// </summary>
        /// <param name="userAuthId">The primary key.</param>
        /// <param name="includeRole">Set to true to include Role nav property</param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        public async Task<UserAuth> GetUserAuthByIdAsync(int userAuthId, bool includeRole = false)
        {
            UserAuth? userAuth;

            if (includeRole)
            {
                userAuth = await _context
                    .Set<UserAuth>()
                    .Include(ua => ua.Role)
                    .FirstOrDefaultAsync(ua => ua.UserAuthId == userAuthId);
            } else
            {
                userAuth = await _context
                    .Set<UserAuth>()
                    .FindAsync(userAuthId);
            }

            if (userAuth == null)
            {
                throw new UserNotFoundException($"User with ID {userAuthId} not found.");
            }
            return userAuth;
        }

        /// <summary>
        /// Query entries by UserName or Email in UsersAuth table
        /// </summary>
        /// <param name="userNameOrEmail"></param>
        /// <param name="includeRole">Set to true to include Role nav property</param>
        /// <returns></returns>
        /// <exception cref="AuthenticationException"></exception>
        public UserAuth GetUserAuthByUserNameOrEmailSync(string userNameOrEmail, bool includeRole = false)
        {
            var trimmedUserNameOrEmail = userNameOrEmail.Trim();

            var query = _context
                .Set<UserAuth>()
                .AsQueryable();

            if (includeRole)
            {
                query = query.Include(ua => ua.Role);
            }

            var userAuth = query
                .FirstOrDefault(
                    ua => ua.UserName == trimmedUserNameOrEmail ||
                          ua.Email == trimmedUserNameOrEmail
                );

            if (userAuth == null)
            {
                throw new AuthenticationException();
            }
            return userAuth;
        }

        /// <summary>
        /// Query entries by UserName or Email in UsersAuth table.
        /// </summary>
        /// <param name="userNameOrEmail"></param>
        /// <param name="includeRole">Set to true to include Role nav property</param>
        /// <returns></returns>
        /// <exception cref="AuthenticationException"></exception>
        public async Task<UserAuth> GetUserAuthByUserNameOrEmailAsync(string userNameOrEmail, bool includeRole = false)
        {
            var trimmedUserNameOrEmail = userNameOrEmail.Trim();

            var query = _context
                .Set<UserAuth>()
                .AsQueryable();

            if (includeRole)
            {
                query = query.Include(ua => ua.Role);
            }

            var userAuth = await query
                .FirstOrDefaultAsync(
                    ua => ua.UserName == trimmedUserNameOrEmail ||
                          ua.Email == trimmedUserNameOrEmail
                );
            if (userAuth == null)
            {
                throw new AuthenticationException();
            }
            return userAuth;
        }

        /// <summary>
        /// Query entries by RoleId in UsersAuth Table.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        public List<UserAuth> GetUsersAuthByRoleIdSync(int roleId)
        {
            var usersAuth = _context
                .Set<UserAuth>()
                .Where(ua => ua.RoleId == roleId)
                .ToList();
            if (usersAuth.Count == 0)
            {
                throw new UserNotFoundException($"User with role ID {roleId} not found.");
            }
            return usersAuth;
        }

        /// <summary>
        /// Query entries by RoleId in UsersAuth Table.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        public async Task<List<UserAuth>> GetUsersAuthByRoleIdAsync(int roleId)
        {
            var usersAuth = await _context
                .Set<UserAuth>()
                .Where(ua => ua.RoleId == roleId)
                .ToListAsync();
            if (usersAuth.Count == 0)
            {
                throw new UserNotFoundException($"User with role ID {roleId} not found.");
            }
            return usersAuth;
        }
        /// <summary>
        /// Get the Role of a user through querying by primary key.
        /// </summary>
        /// <param name="userAuthId">The userAuthId or primary key of the UsersAuth table.</param>
        /// <returns></returns>
        public Role GetUserRoleSync(int userAuthId)
        {
            return this.GetUserAuthByIdSync(userAuthId, true).Role;
        }

        /// <summary>
        /// Get the Role of a user through querying by primary key.
        /// </summary>
        /// <param name="userAuthId">The userAuthId or primary key of the UsersAuth table.</param>
        /// <returns></returns>
        public async Task<Role> GetUserRoleAsync(int userAuthId)
        {
            var userAuth = await this.GetUserAuthByIdAsync(userAuthId, true);
            return userAuth.Role;
        }

        /// <summary>
        /// Get the Role of a user through querying by Username or Email.
        /// </summary>
        /// <param name="userNameOrEmail">The username or email of the user.</param>
        /// <returns></returns>
        public Role GetUserRoleSync(string userNameOrEmail)
        {
            return this.GetUserAuthByUserNameOrEmailSync(userNameOrEmail, true).Role;
        }

        /// <summary>
        /// Get the Role of a user through querying by Username or Email
        /// </summary>
        /// <param name="userNameOrEmail">The username or email of the user.</param>
        /// <returns></returns>
        public async Task<Role> GetUserRoleAsync(string userNameOrEmail)
        {
            var userAuth = await this.GetUserAuthByUserNameOrEmailAsync(userNameOrEmail, true);
            return userAuth.Role;
        }
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

    /// <summary>
    /// Specify the RoleId
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public UserAuthBuilder WithRoleId(int roleId)
    {
        _roleId = roleId;
        return this;
    }

    /// <summary>
    /// Specify the AccountId
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    public UserAuthBuilder WithAccountId(int accountId)
    {
        _accountId = accountId;
        return this;
    }

    /// <summary>
    /// Specify the UserInfoId
    /// </summary>
    /// <param name="userInfoId"></param>
    /// <returns></returns>
    public UserAuthBuilder WithUserInfoId(int userInfoId)
    {
        _userInfoId = userInfoId;
        return this;
    }

    /// <summary>
    /// Specify the UserName
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public UserAuthBuilder WithUserName(string userName)
    {
        _userName = userName.Trim();
        return this;
    }

    /// <summary>
    /// Specify the Email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public UserAuthBuilder WithEmail(string email)
    {
        _email = email.Trim();
        return this;
    }


    /// <summary>
    /// Specify the password.
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
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