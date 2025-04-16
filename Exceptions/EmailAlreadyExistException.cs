namespace Exceptions
{
    public class EmailAlreadyExistException : Exception
    {
        public EmailAlreadyExistException() : base("Email is already used.") { }

        public EmailAlreadyExistException(string userEmail) : base($"{userEmail} is already used.") { }
    }
}
