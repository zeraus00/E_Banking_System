namespace Data.Repositories.Finance
{
    public class LoanTypeRepository
    {
        private readonly EBankingContext _context;

        public LoanTypeRepository(EBankingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new entry to the LoanTypes table.
        /// </summary>
        /// <param name="loanType"></param>
        public void AddLoanTypeSync(LoanType loanType)
        {
            _context.Set<LoanType>().Add(loanType);
        }

        /// <summary>
        /// Adds a new entry to the LoanTypes table.
        /// </summary>
        /// <param name="loanType"></param>
        public async Task AddLoanTypeAsync(LoanType loanType)
        {
            await _context.Set<LoanType>().AddAsync(loanType);
        }
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
