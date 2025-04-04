namespace Data.Models.Authentication
{
    // EmployeesAuth Table
    public class EmployeeAuth
    {
        public int EmployeeAuthId { get; private set; }         // Primary Key
        public string UserName { get; set; } = string.Empty;    // Required; Unique
        public string Email { get; set; } = string.Empty;       // Required; Unique
        public string Password { get; set; } = string.Empty;    // Hashed password; Required
    }
}
