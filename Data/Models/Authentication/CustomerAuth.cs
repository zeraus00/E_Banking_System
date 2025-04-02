namespace E_BankingSystem.Data.Models.Authentication
{


    // CustomersAuth Table
    public class CustomerAuth
    {
        public int CustomerAuthId { get; private set; }         // Primary Key
        public int AccountId { get; private set; }              // Foreign Key to Accounts Table
        public int UserInfoId { get; private set; }                     // Foreign Key to UsersInfo Table
        public string UserName { get; set; } = string.Empty;    // Required; Unique
        public string Email { get; set; } = string.Empty;       // Required; Unique
        public string Password { get; set; } = string.Empty;    // Hashed password: Required



        public Account Account { get; private set; } = null!;   // Navigation Property
        public UserInfo UserInfo { get; private set; } = null!; // Navigation Property
    }
}
