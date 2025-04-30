namespace Exceptions
{
    public class AdminControlledSessionNotFound : Exception
    {
        public AdminControlledSessionNotFound(string sessionScheme) : base($"Admin controlled session scheme {sessionScheme} not found.") { }
    }
}
