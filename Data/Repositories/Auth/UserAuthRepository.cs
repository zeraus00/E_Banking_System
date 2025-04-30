using Exceptions;

namespace Data.Repositories.Auth
{
    /// <summary>
    /// Class for handling CRUD operations in UsersAuth table.
    /// </summary>
    public class UserAuthRepository : Repository
    {
        public UserAuthRepository(EBankingContext context) : base(context) { }

        #region Read Methods

        /// <summary>
        /// Retrieves a UserAuth entry asynchronously by its primary key.
        /// </summary>
        /// <param name="userAuthId">The primary key of the UserAuth entity.</param>
        /// <returns>The UserAuth entity if found or null if not.</returns>
        public async Task<UserAuth?> GetUserAuthByIdAsync(int userAuthId) => await GetById<UserAuth>(userAuthId);
        /// <summary>
        /// Retrieves a UserAuth entry asynchronously by its primary key.
        /// Accepts a pre-composed IQueryable with desired includes (e.g., Role, Account).
        /// </summary>
        /// <param name="userAuthId">The primary key of the UserAuth entity.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// If null, a basic lookup using DbContext.Find is performed.
        /// </param>
        /// <returns>The UserAuth entity if found or null if not.</returns>
        public async Task<UserAuth?> GetUserAuthByIdAsync(int userAuthId, IQueryable<UserAuth> query) => await Get<UserAuth>(ua => ua.UserAuthId == userAuthId, query);

        /// <summary>
        /// Retrieves a UserAuth entry by UserName or Email asynchronously.
        /// Optionally accepts a pre-composed IQueryable with desired includes (e.g., Role, Account).
        /// </summary>
        /// <param name="usernameOrEmail">The username or email of the user to search for.</param>
        /// <param name="query">
        /// An optional IQueryable with includes already applied.
        /// If null, a basic lookup using DbContext.Find is performed.
        /// </param>
        /// <returns>The UserAuth entity if found or null if not.</returns>
        public async Task<UserAuth?> GetUserAuthByUserNameOrEmailAsync(string usernameOrEmail, IQueryable<UserAuth>? query = null)
        {
            var trimmedUserNameOrEmail = usernameOrEmail.Trim();
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
        /// Checks asynchronously if the username already exists.
        /// </summary>
        /// <param name="username">The username to be checked.</param>
        /// <returns>True if the username exists.</returns>
        public async Task<bool> IsUserNameExistsAsync(string username)
        {
            return (await _context.UsersAuth.FirstOrDefaultAsync(ua => ua.UserName == username)) is not null;
        }

        /// <summary>
        /// Checks asynchronously if the email already exists.
        /// </summary>
        /// <param name="email">The email to be checked.</param>
        /// <returns>True if the email exists.</returns>
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return (await _context.UsersAuth.FirstOrDefaultAsync(ua => ua.Email == email)) is not null;
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
        /// Builds an IQueryable for querying the UsersAuth table that includes all related entities.
        /// </summary>
        /// <returns>An IQueryable of UserAuth with all includes.</returns>
        public IQueryable<UserAuth> QueryIncludeAll()
        {
            return this.ComposeQuery(includeRole: true, includeAccounts: true, includeUserInfo: true);
        }

        /// <summary>
        /// Builds an IQueryable for querying the UsersAuth table with optional related entities.
        /// </summary>
        /// <param name="includeRole">Whether to include the related Role entity.</param>
        /// <param name="includeAccounts">Whether to include the related Accounts.</param>
        /// <param name="includeUserInfo">Whether to include the related UserInfo entity.</param>
        /// <returns>An IQueryable of UserAuth with optional includes.</returns>
        public IQueryable<UserAuth> ComposeQuery(
                bool includeRole = false,
                bool includeAccounts = false,
                bool includeUserInfo = false
                )
        {
            var query = _context
                .UsersAuth
                .AsQueryable();
            if (includeRole) { query = query.Include(ua => ua.Role); }
            if (includeAccounts) { query = query.Include(ua => ua.Accounts); }
            if (includeUserInfo) { query = query.Include(ua => ua.UserInfo); }

            return query;
        }

        #endregion Read Methods

        #region Update Methods
        /// <summary>
        /// Updates the email address of a user identified by their user authentication ID.
        /// </summary>
        /// <param name="userAuthId">The ID of the user authentication record.</param>
        /// <param name="email">The new email address to set.</param>
        public async Task UpdateEmailAsync(int userAuthId, string email)
        {
            var userAuth = await this.GetUserAuthByIdAsync(userAuthId);
            if (userAuth is not null) userAuth.Email = email;
        }

        /// <summary>
        /// Updates the email address of a user identified by their username or current email.
        /// </summary>
        /// <param name="usernameOrEmail">The username or current email of the user.</param>
        /// <param name="email">The new email address to set.</param>
        public async Task UpdateEmailAsync(string usernameOrEmail, string email)
        {
            var userAuth = await this.GetUserAuthByUserNameOrEmailAsync(usernameOrEmail);
            if (userAuth is not null) userAuth.Email = email;
        }

        /// <summary>
        /// Updates the username of a user identified by their user authentication ID.
        /// </summary>
        /// <param name="userAuthId">The ID of the user authentication record.</param>
        /// <param name="username">The new username to set.</param>
        public async Task UpdateUsernameAsync(int userAuthId, string username)
        {
            var userAuth = await this.GetUserAuthByIdAsync(userAuthId);
            if (userAuth is not null) userAuth.UserName = username;
        }

        /// <summary>
        /// Updates the username of a user identified by their current username or email.
        /// </summary>
        /// <param name="usernameOrEmail">The current username or email of the user.</param>
        /// <param name="username">The new username to set.</param>
        public async Task UpdateUsernameAsync(string usernameOrEmail, string username)
        {
            var userAuth = await this.GetUserAuthByUserNameOrEmailAsync(usernameOrEmail);
            if (userAuth is not null) userAuth.UserName = username;
        }
        #endregion Update Methods
    }
}
/// <summary>
/// Builder class for UserAuth
/// Methods for setting properties of UserAuth
/// </summary>
public class UserAuthBuilder
{
    private int _roleId;
    private string _userName = string.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;

    #region Builder Methods
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

    #endregion Builder Methods

    /// <summary>
    /// Builds the UserAuth object with the specified properties
    /// </summary>
    /// <returns> UserAuth object </returns>
    public UserAuth Build()
    {
        return new UserAuth
        {
            RoleId = _roleId,
            UserName = _userName,
            Email = _email,
            Password = _password
        };
    }
}