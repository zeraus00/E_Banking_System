namespace Data.Repositories.Finance
{
    public class LoanTransactionRepository
    {
        private readonly EBankingContext _context;

        public LoanTransactionRepository(EBankingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new entry to the LoanTransactions table.
        /// </summary>
        /// <param name="loanTransaction"></param>
        public void AddLoanTransactionSync(LoanTransaction loanTransaction)
        {
            _context.Set<LoanTransaction>().Add(loanTransaction);
        }

        /// <summary>
        /// Adds a new entry to the LoanTransactions table.
        /// </summary>
        /// <param name="loanTransaction"></param>
        public async Task AddLoanTransactionAsync(LoanTransaction loanTransaction)
        {
            await _context.Set<LoanTransaction>().AddAsync(loanTransaction);
        }
    }

    /// <summary>
    /// Builder class for LoanTransaction
    /// </summary>
    public class LoanTransactionBuilder
    {
        private int _accountId;
        private int _loanId;
        private decimal _amountPaid;
        private decimal _remainingLoanBalance;
        private decimal _interestAmount;
        private decimal _principalAmount;
        private DateTime _dueDate;
        private DateTime _transactionDate;
        private TimeSpan _transactionTime;
        private string _notes = string.Empty;

        public LoanTransactionBuilder WithAccountId(int accountId)
        {
            _accountId = accountId;
            return this;
        }
        public LoanTransactionBuilder WithLoanId(int loanId)
        {
            _loanId = loanId;
            return this;
        }

        public LoanTransactionBuilder WithAmountPaid(decimal amountPaid)
        {
            _amountPaid = amountPaid;
            return this;
        }
        public LoanTransactionBuilder WithRemainingLoanBalance(decimal remainingLoanBalance)
        {
            _remainingLoanBalance = remainingLoanBalance;
            return this;
        }

        public LoanTransactionBuilder WithInterestAmount(decimal interestAmount)
        {
            _interestAmount = interestAmount;
            return this;
        }

        public LoanTransactionBuilder WithPrincipalAmount(decimal principalAmount)
        {
            _principalAmount = principalAmount;
            return this;
        }

        public LoanTransactionBuilder WithDueDate(DateTime dueDate)
        {
            _dueDate = dueDate;
            return this;
        }

        public LoanTransactionBuilder WithTransactionDate(DateTime transactionDate)
        {
            _transactionDate = transactionDate;
            return this;
        }

        public LoanTransactionBuilder WithTransactionTime(TimeSpan transactionTime)
        {
            _transactionTime = transactionTime;
            return this;
        }

        public LoanTransactionBuilder WithNotes(string notes)
        {
            _notes = notes.Trim();
            return this;
        }
        /// <summary>
        /// Builds the LoanTransaction object with the specified properties
        /// </summary>
        /// <returns></returns>
        public LoanTransaction Build()
        {
            return new LoanTransaction
            {
                AccountId = _accountId,
                LoanId = _loanId,
                AmountPaid = _amountPaid,
                RemainingLoanBalance = _remainingLoanBalance,
                InterestAmount = _interestAmount,
                PrincipalAmount = _principalAmount,
                DueDate = _dueDate,
                TransactionDate = _transactionDate,
                TransactionTime = _transactionTime,
                Notes = _notes
            };
        }
    }
}
