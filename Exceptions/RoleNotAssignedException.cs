namespace Exceptions
{
    /// <summary>
    /// Thrown when a user is not assigned any role.
    /// </summary>
    public class RoleNotAssignedException : Exception
    {
        public RoleNotAssignedException() : base("User role not found.") { }
        public RoleNotAssignedException(string userName) : base($"User {userName} role not found.") { }
        public RoleNotAssignedException(int userId) : base($"User with id {userId} role not found.") { }

    }
}
