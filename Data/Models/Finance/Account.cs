namespace Data.Models.Finance
{

    // Accounts Table
    public class Account
    {
        public int AccountId { get; set; }                          // Primary Key
        public string AccountType { get; set; } = string.Empty;     // Required; Checking, Savings, etc.
        public string AccountNumber { get; set; } = string.Empty;   // Required
        public string AccountName { get; set; } = string.Empty;     // Required; MaxLength=30
        public string AccountStatus { get; set; } = string.Empty;   // Required; Open, Closed, Suspended
        public decimal Balance { get; set; } = 0.0m;                // Required; Default 0.0

        public DateTime DateOpened { get; set; } = DateTime.UtcNow; // Required; Default DateTime.UtcNow
        public DateTime? DateClosed { get; set; }


        public ICollection<UserAuth> UsersAuth { get; set; } = null!; 
        public ICollection<Transaction> Transactions { get; set; } = null!;  
        public ICollection<Loan> Loans { get; set; } = null!; 
        public ICollection<LoanTransaction> LoanTransactions { get; set; } = null!; 
    }
        
}
