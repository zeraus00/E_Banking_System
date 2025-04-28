using Data;
using Data.Enums;
using Data.Repositories.Finance;

namespace Services
{
    public class AdminDataService : Service
    {

        public AdminDataService(IDbContextFactory<EBankingContext> contextFactory) : base(contextFactory) { }

        /// <summary>
        /// Counts the number of transactions for each transaction type.
        /// </summary>
        /// <param name="transactionList">The list of <see cref="Transaction"/> objects to analyze.</param>
        /// <returns>
        /// A dictionary where the key is the transaction type ID and the value is the count of transactions of that type.
        /// </returns>
        public Dictionary<int, int> GetTransactionCounts(List<Transaction> transactionList)
        {
            Dictionary<int, int> transactionCounts = new();

            //  Convert the TransactionTypes enum to an IEnumerable.
            foreach (var typeId in Enum.GetValues<TransactionTypes>())
            {
                //  Get the count of elmeents matching the transaction type in the list
                //  and assign it to the new dictionary key.
                int count = transactionList.Count(t => t.TransactionTypeId == (int)typeId);
                transactionCounts[(int)typeId] = count;
            }

            return transactionCounts;
        }

        /// <summary>
        /// Retrieves a list of transactions filtered by an optional start and end date.
        /// </summary>
        /// <param name="startDate">The start date for filtering transactions. If null, no lower bound is applied.</param>
        /// <param name="endDate">The end date for filtering transactions. If null, no upper bound is applied.</param>
        /// <returns>A reversed list of <see cref="Transaction"/> entities matching the specified date range.</returns>
        /// <remarks>
        /// Transactions are ordered such that the most recent ones appear first.
        /// </remarks>
        public async Task<List<Transaction>> GetTransactionListAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                //  Declare repository dependency.
                var transactionRepo = new TransactionRepository(dbContext);

                //  Compose Query
                var query = transactionRepo.ComposeQuery();
                query = transactionRepo.FilterQuery(
                    transactionStartDate: startDate, 
                    transactionEndDate: endDate
                    );

                //  Get query as list.
                List<Transaction> transactionList = await query.ToListAsync();

                //  Reverse the list.
                transactionList.Reverse();

                return transactionList;
            }
        }

        
    }
}
