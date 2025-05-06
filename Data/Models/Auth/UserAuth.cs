namespace Data.Models.Authentication
{


    // CustomersAuth Table
    public class UserAuth
    {
        public int UserAuthId { get; set; }                     // Primary Key
        public int RoleId { get; set; }                         // Foreign Key to Roles Table
        public string UserName { get; set; } = string.Empty;    // Required; Unique
        public string Email { get; set; } = string.Empty;       // Required; Unique
        public string Password { get; set; } = string.Empty;    // Hashed password: Required


        /*  Navigation Properties   */
        public Role Role { get; set; } = null!;
        public UserInfo UserInfo { get; set; } = null!;
    }
}
