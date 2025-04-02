namespace E_BankingSystem.Data.Models.Authentication
{


    // CustomersAuth Table
    public class CustomerAuth
    {
        public int CustomerAuthId { get; set; }             // Primary Key


        public int AccountId { get; set; }                  // Foreign Key to Accounts Table
        public Account Account { get; set; } = null!;       // Navigation Property


        public int UserInfoId { get; set; }                     // Foreign Key to UsersInfo Table
        public UserInfo UserInfo { get; set; } = null!;                 // Navigation Property


        public string UserName { get; set; } = string.Empty;// Required; Unique
        public string Email { get; set; } = string.Empty;   // Required; Unique
        public string Password { get; set; } = string.Empty;// Required

    }
}
