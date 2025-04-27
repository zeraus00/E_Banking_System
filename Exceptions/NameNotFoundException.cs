namespace Exceptions
{
    public class NameNotFoundException : Exception
    {
        public NameNotFoundException(int userOrNameId) : base($"Name or User of id {userOrNameId} not found") { }
    }
}
