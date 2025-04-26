using Data;
using Data.Constants;
using Data.Enums;
using Data.Models.Finance;
using Data.Repositories.Finance;
using E_BankingSystem.Components.Client_page.Withdraw;
using E_BankingSystem.Components.Pages;
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

        public async Task InitiateTransaction(
            string sessionScheme,
            int mainAccountId, 
            int transactionTypeId, 
            decimal amount, 
            int? counterAccountId = null)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                /*  Retrieve main account from database.    */
                //  Throws AccountNotFoundException if account is not found.
                Account mainAccount = await this.GetAccountAsync(dbContext, mainAccountId);

                /*  Get Transaction Date and Time.  */
                DateTime transactionDate = DateTime.UtcNow;
                TimeSpan transactionTime = transactionDate.TimeOfDay;

                /*  Generate Transaction Number */
                string accountNumber = mainAccount.AccountNumber;
                string transactionNumber = this.GenerateTransactionNumber(accountNumber, transactionDate);


                /*  For Withdrawal or Outgoing Transfer Transaction Types   */
                try
                {
                    //  Check for sufficient balance
                    //  Throws InsufficientBalanceException if balance is insufficient.
                    this.EnsureSufficientBalance(transactionTypeId, mainAccount.Balance, amount);
                }
                catch (InsufficientBalanceException)
                {
                    /*  Handle Failed Transaction   */
                    await this.TransactionDeniedOrCancelledAsync(
                            transactionTypeId,
                            transactionNumber,
                            TransactionStatus.Denied,
                            mainAccountId,
                            amount,
                            mainAccount.Balance,
                            transactionDate,
                            transactionTime
                        );

                    //  Rethrow exception
                    throw;
                }

                /*  Store transaction details in the session object.    */
                TransactionSession sessionObject = new TransactionSession
                {
                    TransactionTypeId = transactionTypeId,
                    TransactionNumber = transactionNumber,
                    TransactionDate = transactionDate,
                    TransactionTime = transactionTime,
                    MainAccountId = mainAccountId,
                    Amount = amount,
                    CurrentBalance = mainAccount.Balance,
                    CounterAccountId = counterAccountId
                };

                

                /*  Store session object to the session storage.    */
                await _storageService
                    .StoreSessionAsync(sessionScheme, sessionObject);
            }
        }

        public async Task ProcessTransactionAsync(string sessionScheme)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {

                /*  Define repository and builder requirements. */
                var transactionRepository = new TransactionRepository(dbContext);

                /*  Retrieve transaction details from session object */
                //  Throws SessionNotFound if session is not found.
                TransactionSession transactionSession = await _storageService.FetchSessionAsync<TransactionSession>(sessionScheme);
                int transactionTypeId = transactionSession.TransactionTypeId;       //  TransactionTypeId
                int mainAccountId = transactionSession.MainAccountId;               //  MainAccountId
                string transactionNumber = transactionSession.TransactionNumber;    //  TransactionNumber
                DateTime transactionDate = transactionSession.TransactionDate;      //  TransactionDate
                TimeSpan transactionTime = transactionSession.TransactionTime;      //  TransactionTime
                decimal amount = transactionSession.Amount;                         //  Amount
                decimal currentBalance = transactionSession.CurrentBalance;         //  CurrentBalance

                /*  Retrieve account from database  */
                //  Throws AccountNotFoundException if account is not found.
                Account mainAccount = await this.GetAccountAsync(dbContext, mainAccountId);

                /*  For Withdrawal or Outgoing Transfer Transaction Types   */
                //  Handle situations where account's balance is deducted in the background.
                if (mainAccount.Balance != currentBalance)
                {
                    try
                    {
                        //  Ensure sufficient balance.
                        //  Throws InsufficientBalanceException if balance is insufficient.
                        this.EnsureSufficientBalance(transactionTypeId, mainAccount.Balance, amount);
                        //Assign the account's new balance to current balance.
                        currentBalance = mainAccount.Balance;
                    } catch (InsufficientBalanceException)
                    {
                        /*  Handle Failed Transaction   */
                        await this.TransactionDeniedOrCancelledAsync(
                                transactionTypeId,
                                transactionNumber,
                                TransactionStatus.Denied,
                                mainAccountId,
                                amount,
                                mainAccount.Balance,
                                transactionDate,
                                transactionTime
                            );
                        //  Rethrow exception
                        throw;
                    }
                }

                /*  Generate Confirmation Number    */
                string confirmationNumber = this.GenerateConfirmationNumber(transactionDate);


                /*  Balance operations   */
                decimal previousBalance = currentBalance;       
                decimal newBalance = this
                    .calculateBalanceByTransactionType(transactionTypeId, currentBalance, amount);
                this.UpdateAccountBalance(mainAccount, newBalance);

                /*  Build main transaction entry */
                TransactionBuilder mainTransactionBuilder = new TransactionBuilder();
                mainTransactionBuilder
                    .WithTransactionTypeId(transactionTypeId)
                    .WithTransactionNumber(transactionNumber)
                    .WithStatus(TransactionStatus.Confirmed)
                    .WithConfirmationNumber(confirmationNumber)
                    .WithMainAccountId(mainAccountId)
                    .WithAmount(amount)
                    .WithPreviousBalance(previousBalance)
                    .WithNewBalance(newBalance)
                    .WithTransactionDate(transactionDate)
                    .WithTransactionTime(transactionTime);

                /*  For Incoming Transfer or Outgoing Transfer Transaction Types    */
                //  Retrieve counterAccount as needed.
                int? counterAccountId = transactionSession.CounterAccountId;

                if (counterAccountId is int id)
                    mainTransactionBuilder.WithCounterAccountId(id);

                Transaction mainTransaction = mainTransactionBuilder.Build();

                //  Handle Outgoing Transfers
                bool isOutgoingTransfer = transactionTypeId is (int) TransactionTypes.Outgoing_Transfer;
                if (counterAccountId is int counterId && isOutgoingTransfer)
                {
                    /*  Retrieve counter account from the database  */
                    //  Throws AccountNotFoundException if account not found.
                    Account counterAccount = await this.GetAccountAsync(dbContext, counterId);

                    decimal previousCounterBalance = counterAccount.Balance;
                    decimal newCounterBalance = counterAccount.Balance + amount;
                    this.UpdateAccountBalance(counterAccount, newCounterBalance);

                    Transaction counterTransaction = new TransactionBuilder()
                        .WithTransactionTypeId((int)TransactionTypes.Incoming_Transfer)
                        .WithTransactionNumber(transactionNumber)
                        .WithStatus(TransactionStatus.Confirmed)
                        .WithConfirmationNumber(confirmationNumber)
                        .WithMainAccountId(counterId)
                        .WithCounterAccountId(mainAccountId)
                        .WithAmount(amount)
                        .WithPreviousBalance(previousCounterBalance)
                        .WithNewBalance(newCounterBalance)
                        .WithTransactionDate(transactionDate)
                        .WithTransactionTime(transactionTime)
                        .Build();

                    /*  Add counter transaction to database  */
                    await transactionRepository.AddAsync(counterTransaction);
                }

                /*  Add main transaction to database  */
                await transactionRepository.AddAsync(mainTransaction);
                
                //  Save changes to account and transaction.
                await transactionRepository.SaveChangesAsync();
            }
        }

        public async Task TransactionDeniedOrCancelledAsync(
            int transactionTypeId,
            string transactionNumber,
            string status,
            int accountId,
            decimal amount,
            decimal balance,
            DateTime transactionDate,
            TimeSpan transactionTime)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var transactionRepository = new TransactionRepository(dbContext);

                Transaction failedTransaction = new TransactionBuilder()
                    .WithTransactionTypeId(transactionTypeId)
                    .WithTransactionNumber(transactionNumber)
                    .WithStatus(status)
                    .WithMainAccountId(accountId)
                    .WithAmount(amount)
                    .WithPreviousBalance(balance)
                    .WithNewBalance(balance)
                    .WithTransactionDate(transactionDate)
                    .WithTransactionTime(transactionTime)
                    .Build();

                await transactionRepository.AddAsync(failedTransaction);
                await transactionRepository.SaveChangesAsync();
            }
        }
        private async Task<Account> GetAccountAsync(EBankingContext dbContext, int accountId)
        {

            var accountRepository = new AccountRepository(dbContext);
            var account = await accountRepository.GetAccountByIdAsync(accountId);
            return account ?? throw new AccountNotFoundException(accountId);
        }
        private void EnsureSufficientBalance(int transactionTypeId, decimal balance, decimal deductionAmount)
        {
            bool isDeducting = transactionTypeId is (int)TransactionTypes.Withdrawal
                    or (int)TransactionTypes.Outgoing_Transfer;

            if (isDeducting && balance < deductionAmount)
            {
                throw new InsufficientBalanceException();
            }
        }
        private void UpdateAccountBalance(Account account, decimal newBalance)
        {
            account.Balance = newBalance;
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
        private string GenerateTransactionNumber(string accountNumber, DateTime transactionDate)
        {
            return $"TXN-{accountNumber[^4..]}{transactionDate:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }
        private string GenerateConfirmationNumber(DateTime transactionDate)
        {
            return $"CONFIRM-{transactionDate:yyyyMMddHHmmss}{Random.Shared.Next(100000, 999999)}";
        }

    }
}
