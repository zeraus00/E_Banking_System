namespace Data.Models.Finance
{
    public class Loan
    {
        /*  Properties   */
        public int LoanId { get; set; }                         //  Primary key
        public string LoanNumber { get; set; } = string.Empty;  //  Loan reference; Required; Varchar(20)
        public int AccountId { get; set; }                      //  Foreign key to Account
        public int UserInfoId { get; set; }                     //  Foreign Key to UserInfo
        public int LoanTypeId { get; set; }                     //  Foreign key to LoanType
        public string LoanPurpose { get; set; } = string.Empty; //  Required; Varchar(30)
        public decimal LoanAmount { get; set; }                 //  Required; Decimal(18,2)
        public decimal InterestRate { get; set; }               //  Required; Decimal(5,2)
        public int LoanTermMonths { get; set; }                 //  Required; 3, 6, or 12 months
        public int PaymentFrequency { get; set; }              //  Required; In one year; Monthly: 12; Bi-monthly: 6; Quarterly: 3;
        public decimal PaymentAmount { get; set; }              //  Required; Decimal(18,2)
        public decimal RemainingLoanBalance { get; set; }       //  Required; Decimal(18,2)
        public DateTime ApplicationDate { get; set; }           //  Required; Default: CURDATE()
        public string LoanStatus { get; set; } = string.Empty;  //  Required; Default: 'Pending'
        public DateTime StartDate { get; set; }                 //  Required: Loan start date
        public DateTime DueDate { get; set; }                   //  Required: Loan due date
        public DateTime UpdateDate { get; set; }                //  Required: Loan payment update date
        public DateTime EndDate { get; set; }                   //  Required: Loan end date
        public string Remarks { get; set; } = string.Empty;     //  Optional

        /*  Navigation Properties   */

        public Account Account { get; set; } = null!;
        public UserInfo UserInfo { get; set; } = null!;
        public LoanType LoanType { get; set; } = null!;     
        public ICollection<LoanTransaction> LoanTransactions { get; set; } = new List<LoanTransaction>();
    }
}
