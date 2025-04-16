using Data.Repositories.Finance;

namespace Data.Seeders.Finance
{
    public class AccountTypeSeeder : Seeder
    {
        AccountTypeRepository _accountTypeRepository;
        public AccountTypeSeeder(EBankingContext context) : base(context)
        {
            _accountTypeRepository = new AccountTypeRepository(_context);
        }

        public async Task SeedAccountTypes()
        {
            if (!await _context.AccountTypes.AnyAsync())
            {
                var accountTypeBuilder = new AccountTypeBuilder();
                var accountTypeNames = new[]
                {
                    "Checking Account",
                    "Savings Account",
                    "Joint Account",
                    "Loan Account"
                };
                
                foreach (var accountTypeName in accountTypeNames)
                {
                    var accountType = new AccountTypeBuilder()
                        .WithAccountTypeName(accountTypeName)
                        .Build();
                    await _accountTypeRepository.AddAsync(accountType);
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
