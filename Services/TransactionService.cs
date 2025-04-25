using Data;
using Data.Enums;
using Data.Models.Finance;
using Data.Repositories.Finance;
using E_BankingSystem.Components.Client_page.Withdraw;
using Exceptions;
using Microsoft.Identity.Client;

namespace Services
{
    /// <summary>
    /// Service for transactions.
    /// </summary>
    public class TransactionService : Service
    {
        /// <summary>
        /// The constructor of the TransactionService.
        /// Injects an IDbContextFactory that generates a new dbContext connection
        /// for each method to avoid concurrency.
        /// </summary>
        /// <param name="contextFactory">The IDbContextFactory</param>
        public TransactionService(IDbContextFactory<EBankingContext> contextFactory) : base(contextFactory) { }

        public async Task Withdraw(int accountId, decimal withdrawAmount)
        {
            try
            {
                await using (var dbContext = await _contextFactory.CreateDbContextAsync())
                {
                    //  Retrieve account from database.
                    var account = await this.GetAccountAsync(dbContext, accountId);
                    if (account is null) throw new AccountNotFoundException(accountId);

                    //  Check if balance is sufficient.
                    if (account.Balance < withdrawAmount) throw new InsufficientBalanceException();

                    //  Process withdraw transaction.
                    await this.ProcessTransactionAsync(dbContext, account, (int) TransactionTypes.Withdrawal, withdrawAmount);
                }

            } catch (AccountNotFoundException)
            {
                Console.WriteLine("""
                        

                            ERROR : ACCOUNT COULD NOT BE RESOLVED.


                    """);
            } catch (InsufficientBalanceException)
            {
                Console.WriteLine($"""


                            ERROR : INSUFFICIENT BALANCE.


                    """);
            }
            
        }

        public async Task Withdraw(string accountNumber, decimal withdrawAmount)
        {
            try
            {
                await using (var dbContext = await _contextFactory.CreateDbContextAsync())
                {
                    //  Retrieve account from database.
                    var account = await this.GetAccountAsync(dbContext, accountNumber);
                    if (account is null) throw new AccountNotFoundException(accountNumber);

                    //  Check if balance is sufficient.
                    if (account.Balance < withdrawAmount) throw new InsufficientBalanceException();

                    //  Process withdraw transaction.
                    await this.ProcessTransactionAsync(dbContext, account, (int)TransactionTypes.Withdrawal, withdrawAmount);
                }
            } catch (AccountNotFoundException)
            {
                Console.WriteLine("""


                            ERROR : ACCOUNT COULD NOT BE RESOLVED.


                    """);
            } catch (InsufficientBalanceException)
            {
                Console.WriteLine($"""


                            ERROR : INSUFFICIENT BALANCE.


                    """);
            }
        }

        public async Task Deposit(int accountId, decimal depositAmount)
        {
            try
            {
                await using (var dbContext = await _contextFactory.CreateDbContextAsync())
                {
                    //  Retrieve account from database.
                    var account = await this.GetAccountAsync(dbContext, accountId);
                    if (account is null) throw new AccountNotFoundException(accountId);

                    //  Process transaction.
                    await this.ProcessTransactionAsync(dbContext, account, (int)TransactionTypes.Deposit, depositAmount);
                }
            } catch (AccountNotFoundException)
            {

                Console.WriteLine("""


                            ERROR : ACCOUNT COULD NOT BE RESOLVED.


                    """);
            }
        }

        public async Task Deposit(string accountNumber, decimal depositAmount)
        {
            try
            {
                await using (var dbContext = await _contextFactory.CreateDbContextAsync())
                {
                    //  Retrieve account from database.
                    var account = await this.GetAccountAsync(dbContext, accountNumber);
                    if (account is null) throw new AccountNotFoundException(accountNumber);

                    //  Process transaction.
                    await this.ProcessTransactionAsync(dbContext, account, (int)TransactionTypes.Deposit, depositAmount);
                }
            } catch (AccountNotFoundException)
            {
                Console.WriteLine("""


                            ERROR : ACCOUNT COULD NOT BE RESOLVED.


                    """);
            }
        }

        private async Task<Account?> GetAccountAsync(EBankingContext dbContext, int accountId)
        {

            var accountRepository = new AccountRepository(dbContext);
            var account = await accountRepository.GetAccountByIdAsync(accountId);
            return account;
        }

        private async Task<Account?> GetAccountAsync(EBankingContext dbContext, string accountNumber)
        {
            var accountRepository = new AccountRepository(dbContext);
            var account = await accountRepository.GetAccountByAccountNumberAsync(accountNumber);
            return account;
        }

        private async Task ProcessTransactionAsync(EBankingContext dbContext, Account mainAccount, int transactionTypeId, decimal amount, string counterAccountNumber = "")
        {
            //  Define repository and builder requirements.
            var transactionRepository = new TransactionRepository(dbContext);
            var transactionBuilder = new TransactionBuilder();

            //  Calculate previous and new balance
            decimal previousBalance = mainAccount.Balance;
            decimal newBalance = this.calculateBalanceByTransactionType(transactionTypeId, mainAccount.Balance, amount);

            //  Assign new balance to account
            mainAccount.Balance = newBalance;

            //  Build transaction entry
            var withdrawTransaction = transactionBuilder
                .WithAccountId(mainAccount.AccountId)
                .WithTransactionTypeId(transactionTypeId)
                .WithAmount(amount)
                .WithPreviousBalance(previousBalance)
                .WithNewBalance(newBalance);

            //  Add counteraccountnumber if necessary

            //  Add new transaction entry
            await transactionRepository.AddAsync(withdrawTransaction);

            //  Save changes to account and transaction.
            await transactionRepository.SaveChangesAsync();
        }

        private decimal calculateBalanceByTransactionType(int transactionTypeId, decimal balance, decimal amount)
        {
            return transactionTypeId switch
            {
                (int)TransactionTypes.Withdrawal => balance - amount,
                (int)TransactionTypes.Deposit => balance + amount,
                (int)TransactionTypes.Incoming_Transfer => balance + amount,
                (int)TransactionTypes.Outgoing_Transfer => balance - amount,
                _ => throw new ArgumentException("INVALID TRANSACTION TYPE ID.")
            };
        }

    }
}
