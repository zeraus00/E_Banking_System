namespace Data.Models.Finance
{
    public class LoanApplication
    {
        public int LoanApplicationId { get; set; } //primary key
        public int AccountId { get; set; } // foreign key to Account
        public Account Account { get; set; } = null!; // navigation property to Account

        public decimal LoanAmount { get; set; } //Amount being applied for
        public int LoanInMonths { get; set; } // Loan term (3, 6 or 12 months)
        public DateTime ApplicationDate { get; set; } // when the loan application was submitted
        public string ApplicationStatus { get; set; } = "Pending"; // status of the application default

        public string? Email { get; set; } = string.Empty; // if the user have Email

        public string Occupation { get; set; } = string.Empty;
        public decimal MinimumGrossIncome { get; set; }

        public ICollection<CustomerAuth> customerAuths { get; set; } = null!;
    }

}
