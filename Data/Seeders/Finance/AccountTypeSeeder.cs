using Data.Repositories.Finance;

namespace Data.Seeders.Finance
{
    public class AccountTypeSeeder : Seeder
    {
        AccountTypeRepository _accountTypeRepository;
        AccountProductTypeRepository _accountProductTypeRepository;
        public AccountTypeSeeder(EBankingContext context) : base(context)
        {
            _accountTypeRepository = new AccountTypeRepository(_context);
            _accountProductTypeRepository = new AccountProductTypeRepository(_context);
        }

        public async Task SeedAccountTypes()
        {
            if (!await _context.AccountTypes.AnyAsync())
            {
                var accountTypeBuilder = new AccountTypeBuilder();
                var accountTypeNames = new[]
                {
                    "Personal Account",
                    "Joint Account"
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

            if (!await _context.AccountProductTypes.AnyAsync())
            {
                var accountProductTypeBuilder = new AccountProductTypeBuilder();
                var accountProductTypeNames = new[]
                {
                    "Savings",
                    "Checking",
                    "Loan"
                };
                
                foreach (var accountProductTypeName in accountProductTypeNames)
                {
                    var accountProductType = new AccountProductTypeBuilder()
                        .WithAccountProductTypeName(accountProductTypeName)
                        .Build();
                    await _accountProductTypeRepository.AddAsync(accountProductType);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
