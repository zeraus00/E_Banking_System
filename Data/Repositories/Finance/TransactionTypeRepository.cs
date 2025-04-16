namespace Data.Repositories.Finance
{
    /// <summary>
    /// CRUD operations handler for TransactionTypes table.
    /// Methods for adding, updating, deleting and retrieving data from the database
    /// </summary>
    /// <param name="context"></param>
    public class TransactionTypeRepository : Repository
    {
        public TransactionTypeRepository(EBankingContext context) : base(context) { }
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
