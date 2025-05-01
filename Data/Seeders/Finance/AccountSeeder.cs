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
                    .WithAccountType((int)AccountTypes.PersonalAccount)
                    .WithAccountProductTypeId((int)AccountProductTypes.Checking)
                    .WithAccountNumber("123456789ABC")
                    .WithATMNumber("1234567891011123")
                    .WithAccountName("Bogart Dela Mon Sr.")
                    .WithAccountContactNo("11122233344")
                    .WithAccountStatus((int)AccountStatusTypes.Active)
                    .WithBalance(696969)
                    .Build();
                var account2 = new AccountBuilder()
                    .WithAccountType((int)AccountTypes.JointAccount)
                    .WithAccountProductTypeId((int)AccountProductTypes.Savings)
                    .WithAccountNumber("222333444ABC")
                    .WithATMNumber("1111222233334444")
                    .WithAccountContactNo("55566677788")
                    .WithAccountName("Bogart Dela Mon Jr.")
                    .WithAccountStatus((int)AccountStatusTypes.Active)
                    .WithBalance(100000)
                    .Build();

                await _accountRepository.AddAsync(account);
                await _accountRepository.AddAsync(account2);
                await _accountRepository.SaveChangesAsync();
            }
        }
    }
}
