namespace Data.Models.Authentication
{
    // Roles Table
    public class Role
    {
        /*  Properties  */
        public int RoleId { get; private set; } // Primary Key
        public string RoleName { get; private set; } = string.Empty;   // Required; Customer, Employee, Admin

        /*  Navigation Properties  */
        public ICollection<CustomerAuth> CustomersAuth { get; set; } = new List<CustomerAuth>();

    }
}
