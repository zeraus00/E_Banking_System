namespace Data.Models.Finance
{
    public class Loan
    {
        /*  Properties   */
        public int LoanId { get; set; }                     //  Primary key
        public int AccountId { get; set; }                  //  Foreign key to Account
        public int LoanTypeId { get; set; }                 //  Foreign key to LoanType
        public decimal LoanAmount { get; set; }             //  Required; Decimal(18,2)
        public decimal InterestRate { get; set; }           //  Required; Decimal(5,2)
        public int LoanTermMonths { get; set; }             //  Required; 3, 6, or 12 months
        public decimal MonthlyPayment { get; set; }         //  Required; Decimal(18,2)
        public decimal RemainingLoanBalance { get; set; }   //  Required; Decimal(18,2)
        public DateTime ApplicationDate { get; set; }       //  Required; Default: CURDATE()
        public String LoanStatus { get; set; } = "Pending"; //  Required; Default: 'Pending'
        public DateTime StartDate { get; set; }             // Required: Loan start date
        public DateTime DueDate { get; set; }               // Required: Loan due date
        public DateTime UpdateDate { get; set; }            // Required: Loan payment update date
        public DateTime EndDate { get; set; }               // Required: Loan end date

        /*  Navigation Properties   */

        public Account Account { get; set; } = null!; 
        public ICollection<LoanTransaction> Loantransactions { get; set; } = null!; 
        public ICollection<Account> Accounts { get; set; } = null!;
    }
}
