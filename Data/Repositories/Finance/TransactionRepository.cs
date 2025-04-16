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
}
