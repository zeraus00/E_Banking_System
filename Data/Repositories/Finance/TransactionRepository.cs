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
        private DateTime _transactionDate;     //  Default value curdate
        private TimeSpan _transactionTime;     //  Default value curent time
        private decimal _transactionFee;       //  Default value 0

        #region Builder Methods
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
        #endregion Builder Methods

        /// <summary>
        /// Builds the Transaction object with the specified properties
        /// </summary>
        /// <returns></returns>
        public Transaction Build()
        {
            var transaction = new Transaction
            {
                AccountId = _accountId,
                TransactionTypeId = _transactionTypeId,
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
