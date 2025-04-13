namespace Data.Models.Authentication
{


    // CustomersAuth Table
    public class UserAuth
    {
        public int UserAuthId { get; set; }                     // Primary Key
        public int RoleId { get; set; }                         // Foreign Key to Roles Table
        public int? AccountId { get; set; }                     // Foreign Key to Accounts Table
        public int? UserInfoId { get; set; }                    // Foreign Key to UsersInfo Table
        public string UserName { get; set; } = string.Empty;    // Required; Unique
        public string Email { get; set; } = string.Empty;       // Required; Unique
        public string Password { get; set; } = string.Empty;    // Hashed password: Required


        /*  Navigation Properties   */
        public Role Role { get; set; } = null!;              
        public Account? Account { get; set; } 
        public UserInfo? UserInfo { get; set; }
    }
}
