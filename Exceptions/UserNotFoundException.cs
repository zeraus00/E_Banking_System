namespace Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base ("Email does not exist.") {}

        public UserNotFoundException(string Email) : base($"Email {Email} does not exist.") {}
    }
}
