namespace E_Banking.Models.Authentication
{
    // EmployeesAuth Table
    public class EmployeeAuth
    {
        public int EmployeeAuthId { get; set; }                 // Primary Key
        public string Username { get; set; } = string.Empty;    // Required; Unique
        public string Email { get; set; } = string.Empty;       // Required; Unique
        public string Password { get; set; } = string.Empty;    // Required
    }
}
