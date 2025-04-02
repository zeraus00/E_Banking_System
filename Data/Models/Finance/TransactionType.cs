namespace E_BankingSystem.Data.Models.Finance
{
    // TransactionTypes Table
    public class TransactionType
    {
        public int TransactionTypeId { get; set; }  // Primary Key
        public string TransactionTypeName { get; set; } = string.Empty; // Required; Deposit, Withdrawal, Incoming Transfer, Outgoing Transfer, etc.
    
        public ICollection<Transaction> Transactions { get; set; } = null!; // Navigation Property
    }
}
