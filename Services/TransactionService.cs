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

        /// <summary>
        /// Initiates a transaction by validating the account balance, generating a transaction number, 
        /// and storing transaction details in session storage.
        /// If the account's balance is insufficient, the transaction is denied.
        /// </summary>
        /// <param name="sessionScheme">The key used to identify the session storage.</param>
        /// <param name="mainAccountId">The ID of the main account initiating the transaction.</param>
        /// <param name="transactionTypeId">The ID representing the type of transaction (e.g., withdrawal, transfer).</param>
        /// <param name="amount">The amount involved in the transaction.</param>
        /// <param name="counterAccountId">The ID of the counterparty account for transfer transactions (optional).</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="AccountNotFoundException">Thrown if the main account cannot be found in the database.</exception>
        /// <exception cref="InsufficientBalanceException">Thrown if the main account does not have sufficient balance for the transaction.</exception>
        public async Task InitiateTransaction(
            string sessionScheme,
            int mainAccountId, 
            int transactionTypeId, 
            decimal amount, 
            int? counterAccountId = null,
            int? externalVendorId = null)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                /*  Retrieve main account from database.    */
                //  Throws AccountNotFoundException if account is not found.
                Account mainAccount = await this.GetAccountAsync(dbContext, mainAccountId);

                /*  Get Transaction Date and Time.  */
                DateTime transactionDate = DateTime.UtcNow.Date;
                TimeSpan transactionTime = DateTime.UtcNow.TimeOfDay;

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
                            TransactionStatus.DENIED,
                            mainAccountId,
                            amount,
                            mainAccount.Balance,
                            transactionDate,
                            transactionTime
                        );

                    /*  Delete transaction session from session storage */
                    await _storageService.DeleteSessionAsync(sessionScheme);

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
                    CounterAccountId = counterAccountId,
                    ExternalVendorId = externalVendorId
                };

                

                /*  Store session object to the session storage.    */
                await _storageService
                    .StoreSessionAsync(sessionScheme, sessionObject);
            }
        }
        /// <summary>
        /// Processes a transaction by validating the session data, checking balances, updating account balances, 
        /// creating transaction records, and saving the results to the database.
        /// If the user's balance is insufficient, the transaction is denied.
        /// </summary>
        /// <param name="sessionScheme">The key used to retrieve and update the transaction session from storage.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="SessionNotFoundException">Thrown if the transaction session cannot be found in session storage.</exception>
        /// <exception cref="AccountNotFoundException">Thrown if the main or counter account cannot be found in the database.</exception>
        /// <exception cref="InsufficientBalanceException">Thrown if the main account does not have enough balance to complete the transaction.</exception>
        public async Task ProcessTransactionAsync(string sessionScheme)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {

                /*  Define repository and builder requirements. */
                var transactionRepository = new TransactionRepository(dbContext);

                /*  Retrieve transaction details from session object */
                //  Throws SessionNotFoundException if session is not found.
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
                                TransactionStatus.DENIED,
                                mainAccountId,
                                amount,
                                mainAccount.Balance,
                                transactionDate,
                                transactionTime
                            );

                        /*  Delete transaction session from session storage */
                        await _storageService.DeleteSessionAsync(sessionScheme);

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
                    .WithStatus(TransactionStatus.CONFIRMED)
                    .WithConfirmationNumber(confirmationNumber)
                    .WithMainAccountId(mainAccountId)
                    .WithAmount(amount)
                    .WithPreviousBalance(previousBalance)
                    .WithNewBalance(newBalance)
                    .WithTransactionDate(transactionDate)
                    .WithTransactionTime(transactionTime);

                /*  For Deposit or Withdraw Transaction Types   */
                int? externalVendorId = transactionSession.ExternalVendorId;

                if (externalVendorId is int vendorId)
                    mainTransactionBuilder.WithExternalVendorId(vendorId);

                /*  For Outgoing Transfer Transaction Types    */
                //  Retrieve counterAccount as needed.
                int? counterAccountId = transactionSession.CounterAccountId;
                bool isOutgoingTransfer = transactionTypeId is (int) TransactionTypes.Outgoing_Transfer;

                if (counterAccountId is int counterId && isOutgoingTransfer)
                {
                    //  Add counter id to main transaction
                    mainTransactionBuilder.WithCounterAccountId(counterId);
                    /*  Retrieve counter account from the database  */
                    //  Throws AccountNotFoundException if account not found.
                    Account counterAccount = await this.GetAccountAsync(dbContext, counterId);

                    decimal previousCounterBalance = counterAccount.Balance;
                    decimal newCounterBalance = counterAccount.Balance + amount;
                    this.UpdateAccountBalance(counterAccount, newCounterBalance);

                    /*  Build the incoming transfer counter transaction */
                    Transaction counterTransaction = new TransactionBuilder()
                        .WithTransactionTypeId((int)TransactionTypes.Incoming_Transfer)
                        .WithTransactionNumber(transactionNumber)
                        .WithStatus(TransactionStatus.CONFIRMED)
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
                Transaction mainTransaction = mainTransactionBuilder.Build();
                await transactionRepository.AddAsync(mainTransaction);
                
                /*  Save changes to account and transaction.    */
                await transactionRepository.SaveChangesAsync();

                TransactionSession updatedTransactionSession = new TransactionSession
                {
                    TransactionTypeId = transactionTypeId,
                    TransactionNumber = transactionNumber,
                    TransactionDate = transactionDate,
                    TransactionTime = transactionTime,
                    MainAccountId = mainAccountId,
                    Amount = amount,
                    CurrentBalance = mainAccount.Balance,
                    ConfirmationNumber = confirmationNumber,
                    CounterAccountId = counterAccountId
                };

                /*  Update Session  */
                await _storageService
                    .StoreSessionAsync<TransactionSession>(sessionScheme, updatedTransactionSession);
            }
        }
        /// <summary>
        /// Cancels an ongoing transaction by retrieving its session details, 
        /// recording it as a cancelled transaction in the database, 
        /// and deleting the corresponding session from session storage.
        /// </summary>
        /// <param name="sessionScheme">The session scheme used to retrieve and delete the transaction session.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task TransactionCancelledAsync(string sessionScheme)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                /*  Fetch transaction session details from session storage  */
                TransactionSession transactionSession = await _storageService.FetchSessionAsync<TransactionSession>(sessionScheme);
                
                /*  Call TransactionDeniedOrCancelledAsync  */
                await this.TransactionDeniedOrCancelledAsync(
                    transactionSession.TransactionTypeId,
                    transactionSession.TransactionNumber,
                    TransactionStatus.CANCELLED,
                    transactionSession.MainAccountId,
                    transactionSession.Amount,
                    transactionSession.CurrentBalance,
                    transactionSession.TransactionDate,
                    transactionSession.TransactionTime,
                    transactionSession.CounterAccountId
                    );

                /*  Delete transaction session from session storage */
                await _storageService.DeleteSessionAsync(sessionScheme);
            }
        }

        /// <summary>
        /// Records a denied or cancelled transaction in the database by creating a transaction entry 
        /// without modifying the account balance.
        /// </summary>
        /// <param name="transactionTypeId">The ID representing the type of transaction attempted.</param>
        /// <param name="transactionNumber">The unique transaction number assigned to the transaction.</param>
        /// <param name="status">The status indicating why the transaction was not completed (e.g., Denied, Cancelled).</param>
        /// <param name="accountId">The ID of the main account involved in the transaction.</param>
        /// <param name="amount">The transaction amount that was attempted.</param>
        /// <param name="balance">The current balance of the account at the time of the attempt.</param>
        /// <param name="transactionDate">The date when the transaction was attempted.</param>
        /// <param name="transactionTime">The time when the transaction was attempted.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task TransactionDeniedOrCancelledAsync(
            int transactionTypeId,
            string transactionNumber,
            string status,
            int accountId,
            decimal amount,
            decimal balance,
            DateTime transactionDate,
            TimeSpan transactionTime,
            int? counterAccountId = null)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var transactionRepository = new TransactionRepository(dbContext);

                TransactionBuilder failedTransactionBuilder = new TransactionBuilder()
                    .WithTransactionTypeId(transactionTypeId)
                    .WithTransactionNumber(transactionNumber)
                    .WithStatus(status)
                    .WithMainAccountId(accountId)
                    .WithAmount(amount)
                    .WithPreviousBalance(balance)
                    .WithNewBalance(balance)
                    .WithTransactionDate(transactionDate)
                    .WithTransactionTime(transactionTime);

                if (counterAccountId is int id)
                    failedTransactionBuilder.WithCounterAccountId(id);

                Transaction failedTransaction = failedTransactionBuilder.Build();

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
