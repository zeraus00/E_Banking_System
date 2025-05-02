namespace Exceptions
{
    public class ControlledSessionNotFound : Exception
    {
        public ControlledSessionNotFound(string sessionScheme, string controlledSessionScheme) : base($"Controlled Session Scheme: {controlledSessionScheme} under Session Scheme: {sessionScheme} not found.") { }
    }
}
