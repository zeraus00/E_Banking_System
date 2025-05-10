using Data.Repositories.Finance;
using Data.Constants;
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
            if (!_context.TransactionTypes.Any())
            {

                foreach (var transactionTypeName in TransactionTypes.AS_STRING_LIST)
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
}
