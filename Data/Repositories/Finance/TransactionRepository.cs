namespace Data.Repositories.Finance
{
    /// <summary>
    /// CRUD operations handler for Transactions table.
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// </summary>
    /// <param name="context"></param>
    public class TransactionRepository : Repository
    {
        public TransactionRepository(EBankingContext context) : base(context) { }

        public async Task<List<Transaction>> GetTransactionsAsListAsync(int mainAccountId, IQueryable<Transaction>? query = null)
        {
            List<Transaction> transactionsList = new();

            if (query is not  null)
            {
                transactionsList = await query
                     .Where(t => t.MainAccountId == mainAccountId)
                     .ToListAsync();
            } else
            {
                transactionsList = await _context.Transactions
                    .Where(t => t.MainAccountId == mainAccountId)
                    .ToListAsync();
            }

            return transactionsList;
        }

        public IQueryable<Transaction> QueryIncludeAll()
        {
            return this.ComposeQuery(true, true, true);
        }

        public IQueryable<Transaction> ComposeQuery(
            bool includeTransactionType = false,
            bool includeMainAccount = false,
            bool includeCounterAccount = false
            bool includeExternalVendor = false
            )
        {
            IQueryable<Transaction> query = _context.Transactions.AsQueryable();
            if (includeTransactionType) 
                query = query.Include(t => t.TransactionType);
            if (includeMainAccount)
                query = query.Include(t => t.MainAccount);
            if (includeCounterAccount)
                query = query.Include(t => t.CounterAccount);
            if (includeExternalVendor)
                query = query.Include(t => t.ExternalVendor);
            return query;
        }

        public IQueryable<Transaction> FilterQuery(
            IQueryable<Transaction>? query = null,
            int transactionTypeId = 0,
            DateTime? transactionStartDate = null,
            DateTime? transactionEndDate = null
            )
        {
            if (query is null)
                query = _context.Transactions.AsQueryable();
            if (transactionTypeId is not 0)
                query = query.Where(t => t.TransactionTypeId == transactionTypeId);
            if (transactionStartDate.HasValue)
                query = query.Where(t => t.TransactionDate >= transactionStartDate);
            if (transactionEndDate.HasValue)
                query = query.Where(t => t.TransactionDate <= transactionEndDate);

            return query;
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
