namespace Data.Models.Finance
{

    // Accounts Table
    public class Account
    {
        /*  Table Properties    */
        public int AccountId { get; set; }                          // Primary Key
        public int AccountTypeId { get; set; }                      // FK to AccountTypes
        public int AccountProductTypeId { get; set; }               // FK to AccountProductTypes
        public string AccountNumber { get; set; } = string.Empty;   // Required
        public string ATMNumber { get; set; } = string.Empty;       // Required
        public string AccountName { get; set; } = string.Empty;     // Required; MaxLength=30
        public string AccountContactNo { get; set; } = string.Empty;// Required
        public int AccountStatusTypeId { get; set; }                // FK to AccountStatusTypes
        public decimal Balance { get; set; } = 0.0m;                // Required; Default 0.0
        public int? LinkedBeneficiaryId { get; set; }               // Self reference fk
        public DateTime DateOpened { get; set; } = DateTime.UtcNow; // Required; Default DateTime.UtcNow
        public DateTime? DateClosed { get; set; }


        /*  Navigation Properties   */
        public AccountType AccountType { get; set; } = null!;
        public AccountProductType AccountProductType { get; set; } = null!;
        public AccountStatusType AccountStatusType { get; set; } = null!;
        public Account? LinkedBeneficiaryAccount { get; set; }
        public ICollection<Account> LinkedSourceAccounts { get; set; } = new List<Account>();
        public ICollection<UserAuth> UsersAuth { get; set; } = new List<UserAuth>();
        public ICollection<UserInfo> UsersInfo { get; set; } = new List<UserInfo>();
        public ICollection<Transaction> MainTransactions { get; set; } = new List<Transaction>();
        public ICollection<Transaction> CounterTransactions { get; set; } = new List<Transaction>();
        public ICollection<Loan> Loans { get; set; } = new List<Loan>(); 
        public ICollection<LoanTransaction> LoanTransactions { get; set; } = new List<LoanTransaction>(); 
    }
        
}
