using Data;
using Data.Constants;
using Data.Enums;
using Data.Models.Finance;
using Data.Repositories.Finance;
using Exceptions;
using ViewModels.RoleControlledSessions;

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

        /// <summary>
        /// This should be called ON the *_amount pages or when the user is initiating the transaction.
        /// Receives the details of the transaction including the main account's id, the transaction type,
        /// the amount, the counter account's id (if any), and the external vendor's id (if any).
        /// Creates and returns a TransactionSession object containing the details of the transaction.
        /// </summary>
        /// <param name="mainAccountId">
        /// The <see cref="Account.AccountId"/> of the <see cref="Account"> initiating the transaction. 
        /// This should ALWAYS have a value.
        /// </param>
        /// <param name="transactionTypeId">
        /// The id of the transaction type. See <see cref="TransactionTypeIDs"/> enum for this and use it for
        /// calling this method.
        /// </param>
        /// <param name="amount">
        /// The amount of money (in Philippine Peso) involved in the transaction.
        /// </param>
        /// <param name="counterAccountId">
        /// The <see cref="Account.AccountId"/> of the counter <see cref="Account"/> 
        /// recipient of the transaction. Should only have a value IF the transaction type is
        /// <see cref="TransactionTypeConstants.OUTGOING_TRANSFER_TYPE"/>.
        /// </param>
        /// <param name="externalVendorId">
        /// The <see cref="ExternalVendor.VendorId"/> of the vendor involved in the transaction. Should
        /// only have a value IF the transaction type is <see cref="TransactionTypeConstants.DEPOSIT_TYPE"/>
        /// OR <see cref="TransactionTypeConstants.WITHDRAWAL_TYPE"/>
        /// </param>
        /// <returns>
        /// A <see cref="TransactionSession"/> object containing the details of the transaction which will then be
        /// stored in the UserSession.
        /// </returns>
        public async Task<TransactionSession> CreateTransactionAsync(
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

                /*  Store transaction details in the transaction session object.    */
                TransactionSession transactionSession = new TransactionSession
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



                /*  For Withdrawal or Outgoing Transfer Transaction Types   */
                try
                {
                    //  Check for sufficient balance
                    //  Throws InsufficientBalanceException if balance is insufficient.
                    this.EnsureSufficientBalance(transactionTypeId, mainAccount.Balance, amount);
                }
                catch (InsufficientBalanceException)
                {
                    /*  Handle Denied Transaction   */
                    await this.StoreFailedTransactionAsync(transactionSession, TransactionStatus.DENIED);

                    //  Rethrow exception
                    throw;
                }


                //  Return the transaction session model.
                return transactionSession;
            }
        }
        /// <summary>
        /// Called in the *_confirmation pages, when the user is confirming the transaction.
        /// Processes a transaction by validating the session data, checking balances, updating account balances, 
        /// creating transaction records, and saving the results to the database.
        /// If the user's balance is insufficient, the transaction is denied.
        /// </summary>
        /// <param name="transactionSession">
        /// The <see cref="TransactionSession"/> object containing the transaction's details.
        /// </param>
        /// <returns>
        /// A <see cref="TransactionSession"> object containing the updated transaction's details.
        /// </returns>
        /// <exception cref="AccountNotFoundException">Thrown if the main or counter account cannot be found in the database.</exception>
        /// <exception cref="InsufficientBalanceException">Thrown if the main account does not have enough balance to complete the transaction.</exception>
        public async Task<TransactionSession> ProcessTransactionAsync(TransactionSession transactionSession)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {

                /*  Define repository and builder requirements. */
                var transactionRepository = new TransactionRepository(dbContext);

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
                        /*  Handle Denied Transaction   */
                        await this.StoreFailedTransactionAsync(transactionSession, TransactionStatus.DENIED);

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
                bool isOutgoingTransfer = transactionTypeId is (int) TransactionTypeIDs.Outgoing_Transfer;

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
                        .WithTransactionTypeId((int)TransactionTypeIDs.Incoming_Transfer)
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
                    CounterAccountId = counterAccountId,
                    ExternalVendorId = externalVendorId
                };

                return updatedTransactionSession;
            }
        }

        /// <summary>
        /// Stores a failed transaction (due to cancellation or denial) in the database.
        /// </summary>
        /// <param name="transactionSession">
        /// The <see cref="TransactionSession"/> object containing the transaction's details
        /// </param>
        /// <param name="transactionStatus">
        /// The status of the transaction. Should be either <see cref="TransactionStatus.CANCELLED"/> 
        /// OR <see cref="TransactionStatus.DENIED"/>
        /// </param>
        /// <returns></returns>
        public async Task StoreFailedTransactionAsync(TransactionSession transactionSession, string transactionStatus)
        {
            await using (var dbContext = await _contextFactory.CreateDbContextAsync())
            {
                var transactionRepository = new TransactionRepository(dbContext);

                //  Build the failed transaction entity.
                TransactionBuilder failedTransactionBuilder = new TransactionBuilder()
                    .WithTransactionTypeId(transactionSession.TransactionTypeId)
                    .WithTransactionNumber(transactionSession.TransactionNumber)
                    .WithStatus(transactionStatus)
                    .WithMainAccountId(transactionSession.MainAccountId)
                    .WithAmount(transactionSession.Amount)
                    .WithPreviousBalance(transactionSession.CurrentBalance)
                    .WithNewBalance(transactionSession.CurrentBalance)
                    .WithTransactionDate(transactionSession.TransactionDate)
                    .WithTransactionTime(transactionSession.TransactionTime);

                //  Add the counter account's id as necessary.
                if (transactionSession.CounterAccountId is int counterAccountId)
                    failedTransactionBuilder.WithCounterAccountId(counterAccountId);

                //  Add the external vendor's id as necessary.
                if (transactionSession.ExternalVendorId is int externalVendorId)
                    failedTransactionBuilder.WithExternalVendorId(externalVendorId);

                Transaction failedTransaction = failedTransactionBuilder.Build();

                //  Persist the transaction entity to the database.
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
            bool isDeducting = transactionTypeId is (int)TransactionTypeIDs.Withdrawal
                    or (int)TransactionTypeIDs.Outgoing_Transfer;

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
                (int)TransactionTypeIDs.Withdrawal => balance - amount,
                (int)TransactionTypeIDs.Deposit => balance + amount,
                (int)TransactionTypeIDs.Incoming_Transfer => balance + amount,
                (int)TransactionTypeIDs.Outgoing_Transfer => balance - amount,
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
