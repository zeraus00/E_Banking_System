using Exceptions;
using Data;
using Database.Repositories;

namespace Service
{
    public class AuthenticationService
    {

        private readonly EBankingContext _context;
        public AuthenticationService(EBankingContext context)
        {
            _context = context;
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
                var user = new AuthRepository(_context).GetUserAuthByUserNameOrEmailSync(trimmedEmail);
                
                // validate password
                if (!trimmedPassword.Equals(user.Password))
                {
                    throw new IncorrectPasswordException();
                }
                return true;
            }
            catch (UserNotFoundException)
            {
                return false;
            }
            catch (IncorrectPasswordException)
            {
                return false;
            }
            //verify password
        }
        public async Task<bool> IsAuthenticatedAsync(string Email, string Password)
        {
            try
            {
                string trimmedEmail = Email.Trim();
                string trimmedPassword = Password.Trim();

                // check if email exists
                var user = await new AuthRepository(_context).GetUserAuthByUserNameOrEmailAsync(trimmedEmail);

                // validate password
                if (!trimmedPassword.Equals(user.Password))
                {
                    throw new IncorrectPasswordException();
                }

                return true;
            }
            catch (UserNotFoundException)
            {
                return false;
            }
            catch (IncorrectPasswordException)
            {
                return false;
            }

            //verify password
        }

    }
}
