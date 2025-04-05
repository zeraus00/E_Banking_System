namespace Data.Models.Finance
{
    public class ActiveLoan
    {
        public int LoanId { get; set; } // primary key
        public int AccountId { get; set; } //foreign key to Account
        public Account Account { get; set; } = null!;

        public decimal LoanAmount { get; set; }
        public int LoanInMonths { get; set; } // 3, 6, or 12 months
        public DateTime StartDate { get; set; } // when the loan starts
        public DateTime EndDate { get; set; } // when the loan ends

        public ICollection<CustomerAuth> CustomerAuth { get; private set; } = null!; // navigation property
        public ICollection<LoanTransactions> Loantransactions { get; set; } = null!; //navigation property
        public ICollection<Account> Accounts { get; set; } = null!;
    }
}
