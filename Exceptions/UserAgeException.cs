namespace Exceptions
{
    public class UserAgeException : Exception
    {
        public UserAgeException() : base("You must be at least 18 years old to register for an account.") { }

        public UserAgeException(string AgeValidation) : base($"{AgeValidation}") { }
    }
}
