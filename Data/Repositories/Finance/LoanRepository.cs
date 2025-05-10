

namespace Data.Repositories.Finance
{
    /// <summary>
    /// CRUD operations handler for Loans table.
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// </summary>
    /// <param name="context"></param>
    public class LoanRepository : Repository
    {
        public LoanQuery Query;
        public LoanRepository(EBankingContext context) : base(context) 
        {
            Query = new LoanQuery(context.Loans.AsQueryable());
        }

        public async Task<Loan?> GetLoanById(int loanId) => await GetById<Loan>(loanId);

        public class LoanQuery : CustomQuery<Loan, LoanQuery>
        {
            public LoanQuery(IQueryable<Loan> query) : base(query) { }
            public LoanQuery HasStartDateFilter(DateTime startDate) => 
                WhereCondition(l => l.ApplicationDate >= startDate.Date);
            public LoanQuery HasEndDateFilter(DateTime endDate) =>
                WhereCondition(l => l.ApplicationDate <= endDate.Date);
            public LoanQuery IncludeAccount(bool include = true) => include ? Include(l => l.Account) : this;
            public LoanQuery IncludeUserInfo(bool include = true) => include ? Include(l => l.UserInfo) : this;
            public LoanQuery OrderByDateDescending(bool isOrdered = true)
                => OrderByDescending(l => l.ApplicationDate);
        }
    }

    /// <summary>
    /// Builder class for Loan
    /// </summary>
    public class LoanBuilder
    {
        private string _loanNumber = string.Empty;
        private int _accountId;
        private int _userInfoId;
        private string _contactNo = string.Empty;
        private string _email = string.Empty;
        private int _loanTypeId;
        private string _loanPurpose = string.Empty;
        private decimal _loanAmount;
        private decimal _interestRate;
        private decimal _interestAmount;
        private int _loanTermMonths;
        private int _paymentFrequency;
        private decimal _paymentAmount;
        private decimal _remainingLoanBalance;
        private DateTime _applicationDate;
        private string _loanStatus = string.Empty;
        private DateTime? _startDate = null;
        private DateTime? _dueDate = null;
        private DateTime? _updateDate = null;
        private DateTime? _endDate = null;
        private string _remarks = string.Empty;

        #region Builder Methods
        public LoanBuilder WithLoanNumber(string loanNumber)
        {
            _loanNumber = loanNumber;
            return this;
        }
        public LoanBuilder WithAccountId(int accountId)
        {
            _accountId = accountId;
            return this;
        }
        public LoanBuilder WithUserInfoId(int userInfoId)
        {
            _userInfoId = userInfoId;
            return this;
        }
        public LoanBuilder WithContactNo(string contactNo)
        {
            _contactNo = contactNo;
            return this;
        }
        public LoanBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }
        public LoanBuilder WithLoanTypeId(int loanTypeId)
        {
            _loanTypeId = loanTypeId;
            return this;
        }
        public LoanBuilder WithLoanPurpose(string loanPurpose)
        {
            _loanPurpose = loanPurpose;
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
        public LoanBuilder WithInterestAmount(decimal interestAmount)
        {
            _interestAmount = interestAmount;
            return this;
        }
        public LoanBuilder WithLoanTermMonths(int loanTermMonths)
        {
            _loanTermMonths = loanTermMonths;
            return this;
        }
        public LoanBuilder WithPaymentFrequency(int paymentFrequency)
        {
            _paymentFrequency = paymentFrequency;
            return this;
        }
        public LoanBuilder WithMonthlyPayment(decimal paymentAmount)
        {
            _paymentAmount = paymentAmount;
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
        public LoanBuilder WithRemarks(string remarks)
        {
            _remarks = remarks;
            return this;
        }
        #endregion Builder Methods
        /// <summary>
        /// Builds the Loan object with the specified properties
        /// </summary>
        public Loan Build()
        {
            return new Loan
            {
                LoanNumber = _loanNumber,
                AccountId = _accountId,
                UserInfoId = _userInfoId,
                ContactNo = _contactNo,
                Email = _email,
                LoanTypeId = _loanTypeId,
                LoanPurpose = _loanPurpose,
                LoanAmount = _loanAmount,
                InterestRate = _interestRate,
                InterestAmount = _interestAmount,
                LoanTermMonths = _loanTermMonths,
                PaymentFrequency = _paymentFrequency,
                PaymentAmount = _paymentAmount,
                RemainingLoanBalance = _remainingLoanBalance,
                ApplicationDate = _applicationDate,
                LoanStatus = _loanStatus,
                StartDate = _startDate,
                DueDate = _dueDate,
                UpdateDate = _updateDate,
                EndDate = _endDate,
                Remarks = _remarks
            };
        }
    }
}
