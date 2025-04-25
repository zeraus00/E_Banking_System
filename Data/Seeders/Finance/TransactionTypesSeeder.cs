using Data.Repositories.Finance;
using Data.Enums;

namespace Data.Seeders.Finance
{
    public class TransactionTypesSeeder : Seeder
    {
        TransactionTypeRepository _transactionTypesRepo;

        public TransactionTypesSeeder(EBankingContext context) : base (context) 
        {
            _transactionTypesRepo = new TransactionTypeRepository(_context);
        }

        public async Task SeedTransactionTypes()
        {
            var transactionTypes = new List<string>
            {
                "Deposit",
                "Withdrawal",
                "Incoming Transfer",
                "Outgoing Transfer"
            };

            foreach (var transactionTypeName in transactionTypes)
            {
                var transactionType = new TransactionTypeBuilder()
                    .WithTransactionTypeName(transactionTypeName)
                    .Build();

                await _transactionTypesRepo.AddAsync(transactionType);
            }

            await _transactionTypesRepo.SaveChangesAsync();
        }

    }
}
