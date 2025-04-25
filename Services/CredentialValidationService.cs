using Exceptions;
using Data;
using Data.Repositories.Auth;
using Data.Enums;

namespace Services
{
    public class CredentialValidationService : Service
    {
        public CredentialValidationService(IDbContextFactory<EBankingContext> contextFactory) : base(contextFactory) { }
        // create a passwordhasher object here!

        /// <summary>
        /// Attempts to authenticate a user asynchronously based on the provided email and password.
        /// This method trims the input email and password, queries the repository for a matching user, 
        /// and checks if the password is correct. If the credentials are valid, the corresponding 
        /// <see cref="UserAuth"/> object is returned; otherwise, <c>null</c> is returned.
        /// </summary>
        /// <param name="email">The email address associated with the user account.</param>
        /// <param name="password">The password provided for authentication.</param>
        /// <returns>
        /// A <see cref="UserAuth"/> object if authentication is successful; otherwise, <c>null</c> if
        /// the credentials do not match or an error occurs during authentication.
        /// </returns>
        public async Task<UserAuth?> TryValidateUserAsync(string email, string password)
        {
            string trimmedEmail = email.Trim();
            string trimmedPassword = password.Trim();

            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                UserAuthRepository userAuthRepo = new UserAuthRepository(dbContext);

                IQueryable<UserAuth> query = userAuthRepo.QueryIncludeAll();
                UserAuth? userAuth = await userAuthRepo.GetUserAuthByUserNameOrEmailAsync(trimmedEmail, query);

                if (userAuth == null || !this.IsPasswordValid(userAuth, trimmedPassword))
                {
                    return null;
                }

                return userAuth;

            }
        }

        /// <summary>
        /// Method to check if the user is authenticated
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsPasswordValid(UserAuth userAuth, string password)
        {
            return userAuth.Password.Equals(password);
        }

        /// <summary>
        /// Returns true if the RoleId corresponds to administrator.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool IsAdmin(int roleId)
        {
            return roleId == (int) RoleTypes.Administrator;
        }

        /// <summary>
        /// Returns true if the RoleId corresponds to user.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool IsUser(int roleId)
        {
            return roleId == (int) RoleTypes.User;
        }

        /// <summary>
        /// Returns true if the RoleId corresponds to employee.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool IsEmployee(int roleId)
        {
            return roleId == (int) RoleTypes.Employee;
        }
    }
}
