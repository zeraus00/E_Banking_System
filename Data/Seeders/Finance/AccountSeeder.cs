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
                var accountNumber = new CredentialFactory()
                    .GenerateAccountNumber(
                        DateTime.Now.Date,
                        (int)AccountTypes.PersonalAccount,
                        (int)AccountProductTypes.Checking
                    );
                var atmNumber = new CredentialFactory()
                    .GenerateAtmNumber(
                        DateTime.Now.Date,
                        (int)AccountTypes.PersonalAccount,
                        (int)AccountProductTypes.Checking
                    );
                var accountName = new CredentialFactory()
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
                var accountNumber2 = new CredentialFactory()
                    .GenerateAccountNumber(
                        DateTime.Now.Date,
                        (int)AccountTypes.JointAccount,
                        (int)AccountProductTypes.Savings
                    );
                var atmNumber2 = new CredentialFactory()
                    .GenerateAtmNumber(
                        DateTime.Now.Date,
                        (int)AccountTypes.JointAccount,
                        (int)AccountProductTypes.Savings
                    );
                var accountName2 = new CredentialFactory()
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
                CredentialFactory factory = new CredentialFactory();

                List<Account> accounts = await _context.Accounts.ToListAsync();

                foreach (var account in accounts)
                {
                    account.ATMNumber = factory
                        .GenerateAtmNumber(
                            account.DateOpened,
                            account.AccountTypeId,
                            account.AccountProductTypeId
                        );
                    account.AccountNumber = factory
                        .GenerateAccountNumber(
                            account.DateOpened,
                            account.AccountTypeId,
                            account.AccountProductTypeId
                        );
                    account.AccountName = factory
                        .GenerateAccountName(
                            account.AccountTypeId,
                            account.AccountProductTypeId
                        );
                }

                await _context.SaveChangesAsync();
            }
            
        }
    }

}
