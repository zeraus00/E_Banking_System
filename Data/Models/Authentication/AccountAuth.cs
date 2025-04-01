namespace E_Banking.Models.Authentication
{
    using E_Banking.Models.Finance;
    using E_Banking.Models.User;


    // AccountsAuth Table
    public class AccountAuth
    {
        public int AccountAuthId { get; set; }              // Primary Key


        public int AccountId { get; set; }                  // Foreign Key to Accounts Table
        public Account Account { get; set; } = null!;       // Navigation Property


        public int UserId { get; set; }                     // Foreign Key to UsersInfo Table
        public UserInfo? User { get; set; }                 // Navigation Property


        public string UserName { get; set; } = string.Empty;// Required; Unique
        public string Email { get; set; } = string.Empty;   // Required; Unique
        public string Password { get; set; } = string.Empty;// Required

    }
}
