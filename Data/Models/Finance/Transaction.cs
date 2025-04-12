namespace Data.Models.Finance
{
    // Transactions Table
    public class Transaction
    {
        /*  Table Properties    */
        public int TransactionId { get; private set; }   // Primary 
        public int AccountId { get; set; }              // Foreign Key to Accounts Table
        public int TransactionTypeId { get; set; }      // Foreign Key to TransactionTypes Table
        public decimal Amount { get; set; }             // Required; Amount of money involved in transaction
        public decimal PreviousBalance { get; set; }    // Required; Balance before transaction
        public decimal NewBalance { get; set; }         // Required; Balance after transaction
        public DateTime TransactionDate { get; set; }   // Required; Date of transaction
        public TimeSpan TransactionTime { get; set; }   // Required; Default DateTime.UtcNow
        public decimal TransactionFee { get; set; }     // Required; Default 0.0

        /*  Navigation Properties   */
        public Account Account { get; set; } = null!;   
        public TransactionType TransactionType { get; set; } = null!;   
    }
}
