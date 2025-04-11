namespace Exceptions
{
    public class IncorrectPasswordException : Exception
    {
        public IncorrectPasswordException() : base("Password is incorrect.") { }
        public IncorrectPasswordException(string email) : base($"Password for {email} is incorrect.") { }
    }
}
