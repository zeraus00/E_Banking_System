using Exceptions;
using Data;

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

        public async Task<bool> IsAuthenticatedAsync(string Email, string Password)
        {
            try
            {
                // check if email exists
                bool emailExists = await this.EmailExists(Email);
                if (!emailExists)
                {
                    throw new UserNotFoundException();
                }

                bool passwordIsCorrect = await this.PasswordIsCorrect(Email, Password);
                if (!passwordIsCorrect)
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

        // verify if email exists
        public async Task<bool> EmailExists(string email)
        {
            var user = await _context.Set<EmployeeAuth>().FirstOrDefaultAsync(u => u.Email == email);
            if (user!=null)
            Console.WriteLine(user.Email);

            return user != null;
        }

        /* temporary method for testing authentication */
        public async Task<bool> PasswordIsCorrect(string email, string password)
        {
            var user = await _context.Set<EmployeeAuth>().FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false; // fallback safety

            if (password.Trim().Equals(user.Password.Trim()))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
