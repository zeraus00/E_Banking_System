namespace Data.Models.Finance
{
    public class LoanTransactionType
    {
        public int LoanTransactionTypeId { get; set; } // primary key
        public string LoanTypeName { get; set; } = string.Empty; // Loan type

        public ICollection<LoanTransactions> LoanTransactions { get; set; } = null!;
    }
}
