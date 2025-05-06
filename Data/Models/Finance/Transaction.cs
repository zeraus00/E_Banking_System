namespace Data.Models.Finance
{
    // Transactions Table
    public class Transaction
    {
        /*  Table Properties    */
        public int TransactionId { get; private set; }                  // Primary 
        public int TransactionTypeId { get; set; }                      // Foreign Key to TransactionTypes Table
        public string TransactionNumber { get; set; } = string.Empty;   // Required; MaxLength=32; FixedLength; Identifier for transaction
        public string Status { get; set; } = string.Empty;              // Required; Confirmed, Cancelled, Denied
        public string? ConfirmationNumber { get; set; }                 // Optional; MaxLength=28; FixedLength; Identifier for confirmed transactions
        public int MainAccountId { get; set; }                          // Foreign Key to Accounts Table
        public int? CounterAccountId { get; set; }                      // Optional; only for Transfer type transactions
        public int? ExternalVendorId { get; set; }                      // Optional; only for Deposit/Withdrawal types
        public decimal Amount { get; set; }                             // Required; Amount of money involved in transaction
        public decimal PreviousBalance { get; set; }                    // Required; Balance before transaction
        public decimal NewBalance { get; set; }                         // Required; Balance after transaction
        public DateTime TransactionDate { get; set; }                   // Required; Date of transaction
        public TimeSpan TransactionTime { get; set; }                   // Required; Default DateTime.UtcNow
        public decimal TransactionFee { get; set; }                     // Required; Default 0.0
        public string Remarks { get; set; } = string.Empty;             

        /*  Navigation Properties   */
        public TransactionType TransactionType { get; set; } = null!;
        public Account MainAccount { get; set; } = null!;
        public Account CounterAccount { get; set; } = null!;
        public ExternalVendor ExternalVendor { get; set; } = null!;
    }
}
