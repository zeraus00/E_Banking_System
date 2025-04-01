namespace E_BankingSystem.Data.Models.Authentication
{
    // Roles Table
    // Roles are used to group users with similar access rights
    public class Role
    {
        public int RoleId { get; set; } // Primary Key
        public string RoleName { get; set; } = string.Empty;   // Required; Customer, Employee, Admin

    }
}
