using Data.Repositories.Finance;
using Data.Enums;

namespace Data.Seeders.Finance
{
    public class AccountSeeder : Seeder
    {
        AccountRepository _accountRepository;
        public AccountSeeder (EBankingContext context) : base (context) 
        {
            _accountRepository = new AccountRepository(_context);
        }

        public async Task SeedAccounts()
        {
            if (!await _context.Accounts.AnyAsync())
            {
                var account = new AccountBuilder()
                    .WithAccountType((int)AccountTypes.PersonalAcocunt)
                    .WithAccountProductTypeId((int)AccountProductTypes.Checking)
                    .WithAccountNumber("123456789ABC")
                    .WithAccountName("Bogart Dela Mon")
                    .WithAccountStatus("Active")
                    .WithBalance(696969)
                    .Build();
                await _accountRepository.AddAsync(account);
                await _accountRepository.SaveChangesAsync();
            }
        }
    }
}
