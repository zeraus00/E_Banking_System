namespace Exceptions
{
    public class SessionNotFoundException : Exception
    {
        public SessionNotFoundException(string sessionName) : base($"Session '{sessionName}' not found.") { }
    }
}
