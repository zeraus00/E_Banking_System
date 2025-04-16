namespace Exceptions
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException() : base("Invalid Email or Password.") {}
    }
}
