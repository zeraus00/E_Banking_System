using Data;
using Data.Constants;
using Data.Enums;
using Data.Models.Finance;
using Data.Repositories.Finance;
using Exceptions;
using Microsoft.Identity.Client;
using ViewModels;

namespace Services
{
    /// <summary>
    /// Service for transactions.
    /// </summary>
    public class TransactionService : Service
    {
        private readonly SessionStorageService _storageService;
        /// <summary>
        /// The constructor of the TransactionService.
        /// Injects an IDbContextFactory that generates a new dbContext connection
        /// for each method to avoid concurrency.
        /// </summary>
        /// <param name="contextFactory">The IDbContextFactory</param>
        public TransactionService(IDbContextFactory<EBankingContext> contextFactory, SessionStorageService storageService) : base(contextFactory)
        {
            _storageService = storageService;
        }

        public async Task<bool> InitiateWithdraw(int accountId, decimal withdrawAmount)
        {
            try
            {
                await using (var dbContext = await _contextFactory.CreateDbContextAsync())
                {
                    //  Retrieve account from database.
                    //  Throws AccountNotFoundException if account is not found.
                    Account account = await this.GetAccountAsync(dbContext, accountId);

                    //  Throws InsufficientBalanceException if balance is insufficient.
                    this.EnsureSufficientBalance(account.Balance, withdrawAmount);

                    string accountNumber = account.AccountNumber;
                    string transactionNumber = this.GenerateTransactionNumber(accountNumber);

                    TransactionSession withdrawSession = new TransactionSession
                    {
                        TransactionTypeId = (int)TransactionTypes.Withdrawal,
                        TransactionNumber = transactionNumber,
                        MainAccountId = accountId,
                        Amount = withdrawAmount,
                        CurrentBalance = account.Balance
                    };

                    await _storageService.StoreSessionAsync(SessionSchemes.WithdrawTransactionSession, withdrawSession);

                    return true;
                }
            } catch (AccountNotFoundException)
            {
                Console.WriteLine("ACCOUNT NOT FOUND");
                return false;
            } catch (InsufficientBalanceException)
            {
                Console.WriteLine("INSUFFICIENT BALANCE");
                return false;
            }
        }

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

        private async Task<Account> GetAccountAsync(EBankingContext dbContext, int accountId)
        {

            var accountRepository = new AccountRepository(dbContext);
            var account = await accountRepository.GetAccountByIdAsync(accountId);
            return account ?? throw new AccountNotFoundException(accountId);
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

        private void EnsureSufficientBalance(decimal balance, decimal deductionAmount)
        {
            if (balance < deductionAmount)
                throw new InsufficientBalanceException();
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

        private string GenerateTransactionNumber(string accountNumber)
        {
            return $"TXN-{accountNumber[^4..]}{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0,6).ToUpper()}";
        }

        private string GenerateConfirmationNumber()
        {
            return $"CONFIRM-{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(100000,999999)}";
        }
    }
}
