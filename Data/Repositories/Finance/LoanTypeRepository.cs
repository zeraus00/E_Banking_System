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
        public string _loanTypeName = string.Empty;

        public LoanTypeBuilder WithLoanTypeName(string loanTypeName)
        {
            _loanTypeName = loanTypeName.Trim();
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
                LoanTypeName = _loanTypeName
            };
        }
    }
}
