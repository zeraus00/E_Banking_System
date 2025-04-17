using Exceptions;

namespace Data.Repositories.Auth
{
    /// <summary>
    /// Class for handling CRUD operations in UsersAuth table.
    /// Contains both sync and async versions of each method.
    /// </summary>
    public class UserAuthRepository : Repository
    {
        public UserAuthRepository(EBankingContext context) : base(context) { }

        /// <summary>
        /// Retrieves a UserAuth entry by its primary key.
        /// Optionally accepts a pre-composed IQueryable with desired includes (e.g., Role, Account).
        /// </summary>
        /// <param name="userAuthId">The primary key of the UserAuth entity.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// If null, a basic lookup using DbContext.Find is performed.
        /// </param>
        /// <returns>The UserAuth entity if found or null if not.</returns>
        public UserAuth? GetUserAuthByIdSync(int userAuthId, IQueryable<UserAuth>? query = null)
        {
            UserAuth? userAuth;

            if (query != null)
            {
                userAuth = query.FirstOrDefault(ua => ua.UserAuthId == userAuthId);
            }
            else
            {
                userAuth = _context
                    .Set<UserAuth>()
                    .Find(userAuthId);
            }
            return userAuth;
        }

        /// <summary>
        /// Retrieves a UserAuth entry asynchronously by its primary key.
        /// Optionally accepts a pre-composed IQueryable with desired includes (e.g., Role, Account).
        /// </summary>
        /// <param name="userAuthId">The primary key of the UserAuth entity.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// If null, a basic lookup using DbContext.Find is performed.
        /// </param>
        /// <returns>The UserAuth entity if found or null if not.</returns>
        public async Task<UserAuth?> GetUserAuthByIdAsync(int userAuthId, IQueryable<UserAuth>? query = null)
        {
            UserAuth? userAuth;

            if (query != null)
            {
                userAuth = await query.FirstOrDefaultAsync(ua => ua.UserAuthId == userAuthId);
            }
            else
            {
                userAuth = await _context
                    .Set<UserAuth>()
                    .FindAsync(userAuthId);
            }
            return userAuth;
        }

        /// <summary>
        /// Retrieves a UserAuth entry by UserName or Email.
        /// Optionally accepts a pre-composed IQueryable with desired includes (e.g., Role, Account).
        /// </summary>
        /// <param name="userNameOrEmail">The username or email of the user to search for.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// If null, a basic lookup using DbContext.Find is performed.
        /// </param>
        /// <returns>The UserAuth entity if found or null if not.</returns>
        public UserAuth? GetUserAuthByUserNameOrEmailSync(string userNameOrEmail, IQueryable<UserAuth>? query = null)
        {
            var trimmedUserNameOrEmail = userNameOrEmail.Trim();
            UserAuth? userAuth;

            if (query != null)
            {
                userAuth = query
                    .FirstOrDefault(
                    ua => ua.UserName == trimmedUserNameOrEmail ||
                          ua.Email == trimmedUserNameOrEmail
                    );
            }
            else {
                userAuth = _context
                .UsersAuth
                .FirstOrDefault(
                    ua => ua.UserName == trimmedUserNameOrEmail ||
                          ua.Email == trimmedUserNameOrEmail
                );
            }
            return userAuth;
        }

        /// <summary>
        /// Retrieves a UserAuth entry by UserName or Email asynchronously.
        /// Optionally accepts a pre-composed IQueryable with desired includes (e.g., Role, Account).
        /// </summary>
        /// <param name="userNameOrEmail">The username or email of the user to search for.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// If null, a basic lookup using DbContext.Find is performed.
        /// </param>
        /// <returns>The UserAuth entity if found or null if not.</returns>
        public async Task<UserAuth?> GetUserAuthByUserNameOrEmailAsync(string userNameOrEmail, IQueryable<UserAuth>? query = null)
        {
            var trimmedUserNameOrEmail = userNameOrEmail.Trim();
            UserAuth? userAuth;

            if (query != null)
            {
                userAuth = await query
                    .FirstOrDefaultAsync(
                    ua => ua.UserName == trimmedUserNameOrEmail ||
                          ua.Email == trimmedUserNameOrEmail
                    );
            }
            else
            {
                userAuth = await _context
                .UsersAuth
                .FirstOrDefaultAsync(
                    ua => ua.UserName == trimmedUserNameOrEmail ||
                          ua.Email == trimmedUserNameOrEmail
                );
            }
            return userAuth;
        }

        /// <summary>
        /// Retrieves a UserAuth entry by roleId.
        /// Optionally accepts a pre-composed IQueryable with desired includes (e.g., Role, Account).
        /// </summary>
        /// <param name="roleId">The role ID of the users to search for.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// If null, a basic lookup using DbContext.Find is performed.
        /// </param>
        /// <returns>A list of UserAuth entities associated with the specified roleId.</returns>
        public List<UserAuth> GetUsersAuthByRoleIdSync(int roleId, IQueryable<UserAuth>? query = null)
        {
            List<UserAuth> usersAuth;
            if (query != null)
            {
                usersAuth = query
                    .Where(ua => ua.RoleId == roleId)
                    .ToList();
            } else
            {
                usersAuth = _context
                .Set<UserAuth>()
                .Where(ua => ua.RoleId == roleId)
                .ToList();
            }
            return usersAuth;
        }

        /// <summary>
        /// Retrieves a UserAuth entry by roleId asynchronously.
        /// Optionally accepts a pre-composed IQueryable with desired includes (e.g., Role, Account).
        /// </summary>
        /// <param name="roleId">The role ID of the users to search for.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// If null, a basic lookup using DbContext.Find is performed.
        /// </param>
        /// <returns>A list of UserAuth entities associated with the specified roleId.</returns>
        public async Task<List<UserAuth>> GetUsersAuthByRoleIdAsync(int roleId, IQueryable<UserAuth>? query = null)
        {
            List<UserAuth> usersAuth;
            if (query != null)
            {
                usersAuth = await query
                    .Where(ua => ua.RoleId == roleId)
                    .ToListAsync();

            } else
            {
                usersAuth = await _context
                .Set<UserAuth>()
                .Where(ua => ua.RoleId == roleId)
                .ToListAsync();
            }
            return usersAuth;
        }

        /// <summary>
        /// Builds an IQueryable for querying the UsersAuth table with optional related entities.
        /// </summary>
        /// <param name="includeRole">Whether to include the related Role entity.</param>
        /// <param name="includeAccount">Whether to include the related Account entity.</param>
        /// <param name="includeUserInfo">Whether to include the related UserInfo entity.</param>
        /// <returns>An IQueryable of UserAuth with optional includes.</returns>
        public IQueryable<UserAuth> ComposeUsersAuthQuery(
                bool includeRole = false,
                bool includeAccount = false,
                bool includeUserInfo = false
                )
        {
            var query = _context
                .UsersAuth
                .AsQueryable();
            if (includeRole) { query = query.Include(ua => ua.Role); }
            if (includeAccount) { query = query.Include(ua => ua.Account); }
            if (includeUserInfo) { query = query.Include(ua => ua.UserInfo); }

            return query;
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