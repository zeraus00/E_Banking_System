using Exceptions;
using Data;
using Data.Repositories.Auth;
using Data.Enums;

namespace Services
{
    public class AuthenticationService : Service
    {
        private readonly UserAuthRepository _userAuthRepository;
        public AuthenticationService(EBankingContext context) : base(context) {
            _userAuthRepository = new UserAuthRepository(_context);
        }
        // create a passwordhasher object here!


        /// <summary>
        /// Method to check if the user is authenticated
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public bool IsAuthenticatedSync(string Email, string Password)
        {
            try
            {
                string trimmedEmail = Email.Trim();
                string trimmedPassword = Password.Trim();

                // check if email exists
                var user = _userAuthRepository.GetUserAuthByUserNameOrEmailSync(trimmedEmail);
                
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
            //verify password
        }

        /// <summary>
        /// Method to check if the user is authenticated
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public async Task<bool> IsAuthenticatedAsync(string Email, string Password)
        {
            try
            {
                string trimmedEmail = Email.Trim();
                string trimmedPassword = Password.Trim();

                // check if email exists
                var user = await _userAuthRepository.GetUserAuthByUserNameOrEmailAsync(trimmedEmail);

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

            //verify password
        }

        /// <summary>
        /// Method to get the RoleId of a user
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public int GetUserRoleIdSync(string Email)
        {
            return _userAuthRepository.GetUserRoleSync(Email).RoleId;
        }

        /// <summary>
        /// Method to get the RoleId of a user
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public async Task<int> GetUserRoleIdAsync(string Email)
        {
            var Role = await _userAuthRepository.GetUserRoleAsync(Email);
            return Role.RoleId;
        }

        /// <summary>
        /// Returns true if the RoleId corresponds to administrator.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool IsAdmin(int roleId)
        {
            return roleId == (int) RoleType.Administrator;
        }

        /// <summary>
        /// Returns true if the RoleId corresponds to user.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool IsUser(int roleId)
        {
            return roleId == (int) RoleType.User;
        }

        /// <summary>
        /// Returns true if the RoleId corresponds to employee.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool IsEmployee(int roleId)
        {
            return roleId == (int) RoleType.Employee;
        }
    }
}
