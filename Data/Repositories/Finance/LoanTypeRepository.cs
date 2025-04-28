namespace Data.Repositories.Finance
{
    /// <summary>
    /// CRUD operations handler for LoanTypes table.
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// </summary>
    /// <param name="context"></param>
    public class LoanTypeRepository : Repository
    {
        public LoanTypeRepository(EBankingContext context) : base(context) { }
    }

    /// <summary>
    /// Builder class for LoanType
    /// </summary>
    public class LoanTypeBuilder
    {
        private string _loanTypeName = string.Empty;
        private decimal _interestRatePerAnnum;
        private int _loanTermInMonths;

        public LoanTypeBuilder WithLoanTypeName(string loanTypeName)
        {
            _loanTypeName = loanTypeName.Trim();
            return this;
        }
        public LoanTypeBuilder WithInterestRatePerAnnum(decimal interestRatePerAnnum)
        {
            _interestRatePerAnnum = interestRatePerAnnum;
            return this;
        }
        public LoanTypeBuilder WithLoanTermInMonths(int loanTermInMonths)
        {
            _loanTermInMonths = loanTermInMonths;
            return this;
        }

        /// <summary>
        /// Builds the LoanType object with the specified properties
        /// </summary>
        /// <returns></returns>
        public LoanType Build()
        {
            return new LoanType
            {
                LoanTypeName = _loanTypeName,
                InterestRatePerAnnum = _interestRatePerAnnum,
                LoanTermInMonths = _loanTermInMonths
            };
        }
    }
}
