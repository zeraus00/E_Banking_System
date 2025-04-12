namespace Data.Models.Finance
{
    public class LoanTransaction
    {
        /*  Properties   */
        public int LoanTransactionId { get; private set; }      // primary key
        public int AccountId { get; set; }                      // foreign key to Account
        public int LoanId { get; set; }                         // foreign key to loans table
        public decimal AmountPaid { get; set; }                 // total amount paid in this transaction
        public decimal RemainingLoanBalance { get; set; } 
        public decimal InterestAmount { get; set; }             // portion of amount that covers interest
        public decimal PrincipalAmount { get; set; }            //portion of amount that reduces the principal or the original amount borrowed
        public DateTime DueDate { get; set; }
        public DateTime TransactionDate { get; set; }           // when will the payment was supposed to happen
        public TimeSpan TransactionTime { get; set; }
        public string Notes { get; set; } = string.Empty;       // remarks


        /*  Navigation Properties   */
        public Account Account { get; set; } = null!;
        public Loan Loan { get; set; } = null!;
    }
}
