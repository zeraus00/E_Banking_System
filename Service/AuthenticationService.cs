using Exceptions;

namespace Service
{
    public class AuthenticationService(DbContext context)
    {
        private readonly DbContext _context = context;
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
            var user = await _context.Set<CustomerAuth>().FirstOrDefaultAsync(u => u.Email == email);
            return user != null;
        }

        /* temporary method for testing authentication */
        public async Task<bool> PasswordIsCorrect(string email, string password)
        {
            var user = await _context.Set<CustomerAuth>().FirstAsync(u => u.Email == email);
            if (password == user.Password)
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
