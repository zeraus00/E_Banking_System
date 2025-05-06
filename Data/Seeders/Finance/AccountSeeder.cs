using Data.Repositories.Finance;
using Data.Enums;
using Services.DataManagement;

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
                var accountNumber = CredentialFactory
                    .GenerateAccountNumber(
                        DateTime.Now.Date,
                        (int)AccountTypes.PersonalAccount,
                        (int)AccountProductTypes.Checking
                    );
                var atmNumber = CredentialFactory
                    .GenerateAtmNumber(
                        DateTime.Now.Date,
                        (int)AccountTypes.PersonalAccount,
                        (int)AccountProductTypes.Checking
                    );
                var accountName = CredentialFactory
                    .GenerateAccountName(
                        (int)AccountTypes.PersonalAccount,
                        (int)AccountProductTypes.Checking
                    );
                var account = new AccountBuilder()
                    .WithAccountType((int)AccountTypes.PersonalAccount)
                    .WithAccountProductTypeId((int)AccountProductTypes.Checking)
                    .WithAccountNumber(accountNumber)
                    .WithATMNumber(atmNumber)
                    .WithAccountName(accountName)
                    .WithAccountContactNo("11122233344")
                    .WithAccountStatus((int)AccountStatusTypes.Active)
                    .WithBalance(696969)
                    .Build();
                var accountNumber2 = CredentialFactory
                    .GenerateAccountNumber(
                        DateTime.Now.Date,
                        (int)AccountTypes.JointAccount,
                        (int)AccountProductTypes.Savings
                    );
                var atmNumber2 = CredentialFactory
                    .GenerateAtmNumber(
                        DateTime.Now.Date,
                        (int)AccountTypes.JointAccount,
                        (int)AccountProductTypes.Savings
                    );
                var accountName2 = CredentialFactory
                    .GenerateAccountName(
                        (int)AccountTypes.JointAccount,
                        (int)AccountProductTypes.Savings
                    );
                var account2 = new AccountBuilder()
                    .WithAccountType((int)AccountTypes.JointAccount)
                    .WithAccountProductTypeId((int)AccountProductTypes.Savings)
                    .WithAccountNumber(accountNumber2)
                    .WithATMNumber(atmNumber2)
                    .WithAccountContactNo("55566677788")
                    .WithAccountName(accountName2)
                    .WithAccountStatus((int)AccountStatusTypes.Active)
                    .WithBalance(100000)
                    .Build();

                await _accountRepository.AddAsync(account);
                await _accountRepository.AddAsync(account2);
                await _accountRepository.SaveChangesAsync();
            }
        }

        public async Task SeedOrUpdateAtmNumbers()
        {
            if (await _context.Accounts.AnyAsync())
            {

                List<Account> accounts = await _context.Accounts.ToListAsync();

                foreach (var account in accounts)
                {
                    account.ATMNumber = CredentialFactory
                        .GenerateAtmNumber(
                            account.DateOpened,
                            account.AccountTypeId,
                            account.AccountProductTypeId
                        );
                    account.AccountNumber = CredentialFactory
                        .GenerateAccountNumber(
                            account.DateOpened,
                            account.AccountTypeId,
                            account.AccountProductTypeId
                        );
                    account.AccountName = CredentialFactory
                        .GenerateAccountName(
                            account.AccountTypeId,
                            account.AccountProductTypeId
                        );
                    var contactPart1 = $"{Random.Shared.Next(10, 20)}";
                    var contactPart2 = $"{Random.Shared.Next(500, 1000)}";
                    var contactPart3 = $"{Random.Shared.Next(1000, 10000)}";
                    account.AccountContactNo = $"09{contactPart1}{contactPart2}{contactPart3}";
                }

                await _context.SaveChangesAsync();
            }
            
        }
    }

}
