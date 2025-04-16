namespace Data.Repositories.Finance
{
    public class TransactionTypeRepository
    {
        private readonly EBankingContext _context;

        public TransactionTypeRepository(EBankingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new entry to the TransactionTypes table.
        /// </summary>
        /// <param name="transactionType"></param>
        public void AddTransactionTypeSync(TransactionType transactionType)
        {
            _context.Set<TransactionType>().Add(transactionType);
        }

        /// <summary>
        /// Adds a new entry to the TransactionTypes table.
        /// </summary>
        /// <param name="transactionType"></param>
        public async Task AddTransactionTypeAsync(TransactionType transactionType)
        {
            await _context.Set<TransactionType>().AddAsync(transactionType);
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
