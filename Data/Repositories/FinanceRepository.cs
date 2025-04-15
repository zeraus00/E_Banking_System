using Exceptions;

namespace Data.Repositories
{
    /// <summary>
    /// CRUD operations handler for FinanceSchema
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// </summary>
    public class FinanceRepository
    {
        DbContext _context;

        public FinanceRepository(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new object to the finance schema
        /// </summary>
        /// <returns></returns>
        public void AddAccountSync(Account account)
        {
            _context.Set<Account>().Add(account);
        }

        public async Task AddAccountAsync(Account account)
        {
            await _context.Set<Account>().AddAsync(account);
        }

        public void AddLoanSync(Loan loan)
        {
            _context.Set<Loan>().Add(loan);
        }

        public async Task AddLoanAsync(Loan loan)
        {
            await _context.Set<Loan>().AddAsync(loan);
        }

        public void AddLoanTransactionSync(LoanTransaction loanTransaction)
        {
            _context.Set<LoanTransaction>().Add(loanTransaction);
        }

        public async Task AddLoanTransactionAsync(LoanTransaction loanTransaction)
        {
            await _context.Set<LoanTransaction>().AddAsync(loanTransaction);
        }

        public void AddLoanTypeSync(LoanType loanType)
        {
            _context.Set<LoanType>().Add(loanType);
        }

        public async Task AddLoanTypeAsync(LoanType loanType)
        {
            await _context.Set<LoanType>().AddAsync(loanType);
        }

        public void AddTransactionSync(Transaction transaction)
        {
            _context.Set<Transaction>().Add(transaction);
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await _context.Set<Transaction>().AddAsync(transaction);
        }

        public void AddTransactionTypeSync(TransactionType transactionType)
        {
            _context.Set<TransactionType>().Add(transactionType);
        }

        public async Task AddTransactionTypeAsync(TransactionType transactionType)
        {
            await _context.Set<TransactionType>().AddAsync(transactionType);
        }
        /// <summary>
        /// Methods for querying the finance schema
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public Account GetAccountByIdSync(int accountId)
        {
            //
            var account = _context.Set<Account>().Find(accountId);
            if (account == null)
            {
                throw new AccountNotFoundException(accountId);
            }
            return account;
        }

        public async Task<Account> GetAccountByIdAsync(int accountId)
        {
            var account = await _context.Set<Account>().FindAsync(accountId);
            if (account == null)
            {
                throw new AccountNotFoundException(accountId);
            }
            return account;
        }

        public Account GetAccountByAccountNumberSync(string accountNumber)
        {
            var trimmedAccountNumber = accountNumber.Trim();
            var account = _context
                .Set<Account>()
                .FirstOrDefault(
                    acc => acc.AccountNumber == trimmedAccountNumber
                );
            if (account == null)
            {
                throw new AccountNotFoundException(accountNumber);
            }
            return account;
        }

        public async Task<Account> GetAccountByAccountNumberAsync(string accountNumber)
        {
            var trimmedAccountNumber = accountNumber.Trim();
            var account = await _context
                .Set<Account>()
                .FirstOrDefaultAsync(
                    acc => acc.AccountNumber == trimmedAccountNumber
                );
            if (account == null)
            {
                throw new AccountNotFoundException(accountNumber);
            }
            return account;
        }
    }
    /// <summary>
    /// Builder class for Account
    /// </summary>
    public class AccountBuilder
    {
        private string _accountType = string.Empty;
        private string _accountNumber = string.Empty;
        private string _accountName = string.Empty;
        private string _accountStatus = string.Empty;
        private decimal _balance = 0.0m;
        private DateTime _dateOpened = DateTime.UtcNow;
        private DateTime? _dateClosed;

        public AccountBuilder WithAccountType(string accountType)
        {
            _accountType = accountType.Trim();
            return this;
        }

        public AccountBuilder WithAccountNumber(string accountNumber)
        {
            _accountNumber = accountNumber.Trim();
            return this;
        }

        public AccountBuilder WithAccountName(string accountName)
        {
            _accountName = accountName.Trim();
            return this;
        }

        public AccountBuilder WithAccountStatus(string accountStatus)
        {
            _accountStatus = accountStatus.Trim();
            return this;
        }

        public AccountBuilder WithBalance(decimal balance)
        {
            _balance = balance;
            return this;
        }

        public AccountBuilder WithDateOpened(DateTime dateOpened)
        {
            _dateOpened = dateOpened;
            return this;
        }

        public AccountBuilder WithDateClosed(DateTime? dateClosed)
        {
            _dateClosed = dateClosed;
            return this;
        }

        /// <summary>
        /// Builds the Account object with the specified properties
        /// </summary>
        /// <returns></returns>
        public Account Build()
        {
            return new Account
            {
                AccountType = _accountType,
                AccountNumber = _accountNumber,
                AccountName = _accountName,
                AccountStatus = _accountStatus,
                Balance = _balance,
                DateOpened = _dateOpened,
                DateClosed = _dateClosed
            };
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

    /// <summary>
    /// Builder class for Transaction
    /// </summary>
    public class TransactionBuilder
    {
        private int _accountId;
        private int _transactionTypeId;
        private decimal _amount;
        private decimal _previousBalance;
        private decimal _newBalance;
        private DateTime _transactionDate;
        private TimeSpan _transactionTime;
        private decimal _transactionFee;

        public TransactionBuilder WithAccountId(int accountId)
        {
            _accountId = accountId;
            return this;
        }
        public TransactionBuilder WithTransactionTypeId(int transactionTypeId)
        {
            _transactionTypeId = transactionTypeId;
            return this;
        }
        public TransactionBuilder WithAmount(decimal amount)
        {
            _amount = amount;
            return this;
        }
        public TransactionBuilder WithPreviousBalance(decimal previousBalance)
        {
            _previousBalance = previousBalance;
            return this;
        }
        public TransactionBuilder WithNewBalance(decimal newBalance)
        {
            _newBalance = newBalance;
            return this;
        }
        public TransactionBuilder WithTransactionDate(DateTime transactionDate)
        {
            _transactionDate = transactionDate;
            return this;
        }
        public TransactionBuilder WithTransactionTime(TimeSpan transactionTime)
        {
            _transactionTime = transactionTime;
            return this;
        }
        public TransactionBuilder WithTransactionFee(decimal transactionFee)
        {
            _transactionFee = transactionFee;
            return this;
        }

        /// <summary>
        /// Builds the Transaction object with the specified properties
        /// </summary>
        /// <returns></returns>
        public Transaction Build()
        {
            return new Transaction
            {
                AccountId = _accountId,
                TransactionTypeId = _transactionTypeId,
                Amount = _amount,
                PreviousBalance = _previousBalance,
                NewBalance = _newBalance,
                TransactionDate = _transactionDate,
                TransactionTime = _transactionTime,
                TransactionFee = _transactionFee
            };
        }
    }

    /// <summary>
    /// Builder class for TransactionType
    /// </summary>
    public class TransactionTypeBuilder
    {
        private string _transactionTypeName = string.Empty;

        public TransactionTypeBuilder WithTransactionTypeName(string transactionTypeName)
        {
            _transactionTypeName = transactionTypeName.Trim();
            return this;
        }

        /// <summary>
        /// Builds the TransactionType object with the specified properties
        /// </summary>
        /// <returns></returns>
        public TransactionType Build()
        {
            return new TransactionType
            {
                TransactionTypeName = _transactionTypeName
            };
        }
    }
}
