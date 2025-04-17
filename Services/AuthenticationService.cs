using Exceptions;
using Data;
using Data.Repositories.Auth;
using Data.Enums;

namespace Services
{
    public class AuthenticationService : Service
    {
        private UserAuthRepository _userAuthRepository;
        public AuthenticationService(EBankingContext context) : base(context) {
            _userAuthRepository = new UserAuthRepository(_context);
        }
        // create a passwordhasher object here!


        /// <summary>
        /// Method to check if the user is authenticated
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool IsAuthenticatedSync(string email, string password)
        {
            try
            {
                string trimmedEmail = email.Trim();
                string trimmedPassword = password.Trim();

                // check if email exists
                var user = _userAuthRepository
                    .GetUserAuthByUserNameOrEmailSync(trimmedEmail) 
                    ?? throw new AuthenticationException();
                
                

                // validate password
                if (!trimmedPassword.Equals(user.Password))
                {
                    throw new AuthenticationException();
                }
                return true;
            }
            catch (AuthenticationException)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to check if the user is authenticated
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> IsAuthenticatedAsync(string email, string password)
        {
            try
            {
                string trimmedEmail = email.Trim();
                string trimmedPassword = password.Trim();

                // check if email exists
                var user = await _userAuthRepository
                    .GetUserAuthByUserNameOrEmailAsync(trimmedEmail)
                    ?? throw new AuthenticationException();

                // validate password
                if (!trimmedPassword.Equals(user.Password))
                {
                    throw new AuthenticationException();
                }
                return true;
            }
            catch (AuthenticationException)
            {
                return false;
            }
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
