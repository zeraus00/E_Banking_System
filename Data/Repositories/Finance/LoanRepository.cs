namespace Data.Repositories.Finance
{
    public class LoanRepository
    {
        private readonly EBankingContext _context;

        public LoanRepository(EBankingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new entry to the Loans table.
        /// </summary>
        /// <returns></returns>
        public void AddLoanSync(Loan loan)
        {
            _context.Set<Loan>().Add(loan);
        }

        /// <summary>
        /// Adds a new entry to the Loans table.
        /// </summary>
        /// <returns></returns>
        public async Task AddLoanAsync(Loan loan)
        {
            await _context.Set<Loan>().AddAsync(loan);
        }
    }

    /// <summary>
    /// Builder class for Loan
    /// </summary>
    public class LoanBuilder
    {
        private int _accountId;
        private int _loanTypeId;
        private decimal _loanAmount;
        private decimal _interestRate;
        private int _loanTermMonths;
        private decimal _monthlyPayment;
        private decimal _remainingLoanBalance;
        private DateTime _applicationDate;
        private string _loanStatus = string.Empty;
        private DateTime _startDate;
        private DateTime _dueDate;
        private DateTime _updateDate;
        private DateTime _endDate;
        public LoanBuilder WithAccountId(int accountId)
        {
            _accountId = accountId;
            return this;
        }
        public LoanBuilder WithLoanTypeId(int loanTypeId)
        {
            _loanTypeId = loanTypeId;
            return this;
        }
        public LoanBuilder WithLoanAmount(decimal loanAmount)
        {
            _loanAmount = loanAmount;
            return this;
        }
        public LoanBuilder WithInterestRate(decimal interestRate)
        {
            _interestRate = interestRate;
            return this;
        }
        public LoanBuilder WithLoanTermMonths(int loanTermMonths)
        {
            _loanTermMonths = loanTermMonths;
            return this;
        }
        public LoanBuilder WithMonthlyPayment(decimal monthlyPayment)
        {
            _monthlyPayment = monthlyPayment;
            return this;
        }
        public LoanBuilder WithRemainingLoanBalance(decimal remainingLoanBalance)
        {
            _remainingLoanBalance = remainingLoanBalance;
            return this;
        }
        public LoanBuilder WithApplicationDate(DateTime applicationDate)
        {
            _applicationDate = applicationDate;
            return this;
        }
        public LoanBuilder WithLoanStatus(string loanStatus)
        {
            _loanStatus = loanStatus.Trim();
            return this;
        }
        public LoanBuilder WithStartDate(DateTime startDate)
        {
            _startDate = startDate;
            return this;
        }
        public LoanBuilder WithDueDate(DateTime dueDate)
        {
            _dueDate = dueDate;
            return this;
        }
        public LoanBuilder WithUpdateDate(DateTime updateDate)
        {
            _updateDate = updateDate;
            return this;
        }
        public LoanBuilder WithEndDate(DateTime endDate)
        {
            _endDate = endDate;
            return this;
        }
        /// <summary>
        /// Builds the Loan object with the specified properties
        ///
        public Loan Build()
        {
            return new Loan
            {
                AccountId = _accountId,
                LoanTypeId = _loanTypeId,
                LoanAmount = _loanAmount,
                InterestRate = _interestRate,
                LoanTermMonths = _loanTermMonths,
                MonthlyPayment = _monthlyPayment,
                RemainingLoanBalance = _remainingLoanBalance,
                ApplicationDate = _applicationDate,
                LoanStatus = _loanStatus,
                StartDate = _startDate,
                DueDate = _dueDate,
                UpdateDate = _updateDate,
                EndDate = _endDate
            };
        }
    }
}
