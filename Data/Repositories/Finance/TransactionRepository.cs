using Data.Constants;

namespace Data.Repositories.Finance
{
    /// <summary>
    /// CRUD operations handler for Transactions table.
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// </summary>
    /// <param name="context"></param>
    public class TransactionRepository : Repository
    {
        public TransactionQuery Query;
        public TransactionRepository(EBankingContext context) : base(context) 
        {
            Query = new TransactionQuery(context.Transactions.AsQueryable());
        }

        public async Task<List<Transaction>> GetRecentTransactionsAsListAsync(int mainAccountId, int skipCount, int takeCount, IQueryable<Transaction>? query = null)
        {
            List<Transaction> transactionsList = new();

            if (query is not  null)
            {
                transactionsList = await query
                     .Where(t => t.MainAccountId == mainAccountId)
                     .OrderByDescending(t => t.TransactionDate)
                     .ThenByDescending(t => t.TransactionTime)
                     .Skip(skipCount)
                     .Take(takeCount)
                     .ToListAsync();
            } else
            {
                transactionsList = await _context.Transactions
                    .Where(t => t.MainAccountId == mainAccountId)
                    .OrderByDescending(t => t.TransactionDate)
                    .ThenByDescending(t => t.TransactionTime)
                    .Skip(skipCount)
                    .Take(takeCount)
                    .ToListAsync();
            }

            return transactionsList;
        }
        public class TransactionQuery : CustomQuery<Transaction, TransactionQuery>
        {
            public TransactionQuery(IQueryable<Transaction> query) : base(query) { }
            public TransactionQuery HasTransactionTypeId(int transactionTypeId) => 
                WhereCondition(t => t.TransactionTypeId == transactionTypeId);
            public TransactionQuery ExceptTransactionTypeId(int transactionTypeId) =>
                WhereCondition(t => t.TransactionTypeId != transactionTypeId);
            public TransactionQuery HasStatus(string status) =>
                WhereCondition(t => t.Status == status);
            public TransactionQuery HasStatusConfirmed(bool hasStatusConfirmed = true) =>
                hasStatusConfirmed ? WhereCondition(t => t.Status == TransactionStatus.CONFIRMED) : this;
            public TransactionQuery HasStatusCancelled(bool hasStatusCancelled = true) =>
                hasStatusCancelled ? WhereCondition(t => t.Status == TransactionStatus.CANCELLED) : this;
            public TransactionQuery HasStatusDenied(bool hasStatusDenied = true) =>
                hasStatusDenied ? WhereCondition(t => t.Status == TransactionStatus.DENIED) : this;
            public TransactionQuery ExceptStatus(string status) =>
                WhereCondition(t => t.Status != status);
            public TransactionQuery HasMainAccountId(int accountId) =>
                WhereCondition(t => t.MainAccountId == accountId);
            public TransactionQuery HasCounterAccountId(int accountId) =>
                WhereCondition(t => t.CounterAccountId == accountId);
            public TransactionQuery HasExternalVendorId(int vendorId) =>
                WhereCondition(t => t.ExternalVendorId == vendorId);
            public TransactionQuery HasLoanId(int loanId) =>
                WhereCondition(t => t.LoanId == loanId);
            public TransactionQuery HasStartDate(DateTime startDate) =>
                WhereCondition(t => t.TransactionDate >= startDate.Date);
            public TransactionQuery HasStartTime(TimeSpan startTime) =>
                WhereCondition(t => t.TransactionTime >= startTime);
            public TransactionQuery HasEndDate(DateTime endDate) =>
                WhereCondition(t => t.TransactionDate <= endDate.Date);
            public TransactionQuery IncludeTransactionType(bool include = true) => 
                include ? Include(t => t.TransactionType) : this;
            public TransactionQuery IncludeMainAccount(bool include = true) => 
                include ? Include(t => t.MainAccount) : this;
            public TransactionQuery IncludeCounterAccount(bool include = true) => 
                include ? Include(t => t.CounterAccount) : this;
            public TransactionQuery IncludeExternalVendor(bool include = true) => 
                include ? Include(t => t.ExternalVendor) : this;
            public TransactionQuery OrderByDateAndTime(bool isOrdered = true)
            {
                if (isOrdered)
                    _query = _query
                        .OrderBy(t => t.TransactionDate)
                        .ThenBy(t => t.TransactionTime);
                return this;
            }
            public TransactionQuery OrderByDateAndTimeDescending(bool isOrdered = true)
            {
                if (isOrdered)
                    _query = _query
                        .OrderByDescending(t => t.TransactionDate)
                        .ThenByDescending(t => t.TransactionTime);
                return this;
            }
            public TransactionQuery OrderByAmountDescending(bool isOrdered = true) =>
                isOrdered ? OrderByDescending(t => t.Amount) : this;
        }
    }

    /// <summary>
    /// Builder class for Transaction
    /// </summary>
    public class TransactionBuilder
    {
        private int _transactionTypeId;
        private string _transactionNumber = string.Empty;
        private string _status = string.Empty;
        private string? _confirmationNumber = null;
        private int _mainAccountId;
        private int? _counterAccountId = null;
        private int? _externalVendorId = null;
        private int? _loanId = null;
        private decimal _amount;
        private decimal _previousBalance;
        private decimal _newBalance;
        private DateTime _transactionDate;     //  Default value curdate
        private TimeSpan _transactionTime;     //  Default value curent time
        private decimal _transactionFee;       //  Default value 0

        #region Builder Methods
        public TransactionBuilder WithTransactionTypeId(int transactionTypeId)
        {
            _transactionTypeId = transactionTypeId;
            return this;
        }
        public TransactionBuilder WithTransactionNumber(string transactionNumber)
        {
            _transactionNumber = transactionNumber;
            return this;
        }
        public TransactionBuilder WithStatus(string status)
        {
            _status = status;
            return this;
        }
        public TransactionBuilder WithConfirmationNumber(string confirmationNumber)
        {
            _confirmationNumber = confirmationNumber;
            return this;
        }
        public TransactionBuilder WithMainAccountId(int accountId)
        {
            _mainAccountId = accountId;
            return this;
        }
        public TransactionBuilder WithCounterAccountId(int counterAccountId)
        {
            _counterAccountId = counterAccountId;
            return this;
        }
        public TransactionBuilder WithExternalVendorId(int externalVendorId)
        {
            _externalVendorId = externalVendorId;
            return this;
        }
        public TransactionBuilder WithLoanId(int loanId)
        {
            _loanId = loanId;
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
        #endregion Builder Methods

        /// <summary>
        /// Builds the Transaction object with the specified properties
        /// </summary>
        /// <returns></returns>
        public Transaction Build()
        {
            var transaction = new Transaction
            {
                TransactionTypeId = _transactionTypeId,
                TransactionNumber = _transactionNumber,
                Status = _status,
                ConfirmationNumber = _confirmationNumber,
                MainAccountId = _mainAccountId,
                CounterAccountId = _counterAccountId,
                ExternalVendorId = _externalVendorId,
                LoanId = _loanId,
                Amount = _amount,
                PreviousBalance = _previousBalance,
                NewBalance = _newBalance
            };

            if (_transactionDate != default) transaction.TransactionDate = _transactionDate;
            if (_transactionTime != default) transaction.TransactionTime = _transactionTime;
            if (_transactionFee != default) transaction.TransactionFee = _transactionFee;

            return transaction;
        }

    }
}
