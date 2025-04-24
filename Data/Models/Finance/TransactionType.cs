namespace Data.Models.Finance
{
    // TransactionTypes Table
    public class TransactionType
    {
        /*  Table Properties    */
        public int TransactionTypeId { get; set; }                      // Primary Key
        public string TransactionTypeName { get; set; } = string.Empty; // Required; Deposit, Withdrawal, Incoming Transfer, Outgoing Transfer, etc.
    
        /*  Navigation Properties   */
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>(); // Navigation Property
    }
}
