namespace Data.Models.Finance
{
    public class LoanType
    {
        /*  Properties   */
        public int LoanTypeId { get; set; }                         //  Primary key
        public string LoanTypeName { get; set; } = string.Empty;    //  Required; Max Length: 20
        public decimal InterestRatePerAnnum { get; set; } = 0.00m;  //  Required; decimal
        public int LoanTermInMonths { get; set; }                   //  Suggested Loan Term in Months


        /*  Navigation Properties   */
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
