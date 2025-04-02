namespace E_BankingSystem.Data.Models.Authentication
{
    // Roles Table
    public class Role
    {
        public int RoleId { get; private set; } // Primary Key
        public string RoleName { get; private set; } = string.Empty;   // Required; Customer, Employee, Admin

    }
}
