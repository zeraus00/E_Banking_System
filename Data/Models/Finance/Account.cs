namespace Data.Models.Finance
{

    // Accounts Table
    public class Account
    {
        public int AccountId { get; private set; }                          // Primary Key
        public string AccountType { get; private set; } = string.Empty;     // Required; Checking, Savings, etc.
        public string AccountNumber { get; private set; } = string.Empty;   // Required
        public string AccountName { get; private set; } = string.Empty;     // Required; MaxLength=30
        public string AccountStatus { get; private set; } = string.Empty;   // Required; Open, Closed, Suspended
        public decimal Balance { get; private set; } = 0.0m;                // Required; Default 0.0

        public DateTime DateOpened { get; private set; } = DateTime.UtcNow; // Required; Default DateTime.UtcNow
        public DateTime? DateClosed { get; private set; }


        public ICollection<CustomerAuth> CustomersAuth { get; private set; } = null!; // Navigation Property
        public ICollection<Transaction> Transactions { get; private set; } = null!;  // Navigation Property
        public ICollection<LoanTransaction> LoanTransactions { get; set; } = null!; // Navigation Property
        public ICollection<Loan> ActiveLoans { get; private set; } = null!; // Navigation Property
    }
}
