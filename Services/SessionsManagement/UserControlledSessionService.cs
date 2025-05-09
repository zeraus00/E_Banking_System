using Data.Constants;
using Data.Enums;
using Exceptions;
using ViewModels.RoleControlledSessions;
using ViewModels.Sessions;
using Services.DataManagement;

namespace Services.SessionsManagement
{
    public class UserControlledSessionService
    {
        private readonly DataMaskingService _dataMaskingService;
        private readonly UserDataService _userDataService;
        private readonly UserSessionService _userSessionService;

        public UserControlledSessionService(
            DataMaskingService dataMaskingService,
            UserDataService userDataService,
            UserSessionService userSessionService
            )
        {
            _dataMaskingService = dataMaskingService;
            _userDataService = userDataService;
            _userSessionService = userSessionService;
        }

        #region Account Session
        /*      User Account List       */
        /// <summary>
        /// Get the linked accounts of the user inside the user session as a 
        /// list of <see cref="LinkedAccount"/> objects.
        /// </summary>
        /// <param name="userSession">
        /// The user session containing the linked accounts.
        /// </param>
        /// <returns></returns>
        public async Task<List<LinkedAccount>> GetUserAccountListAsync(UserSession? userSession = null)
        => userSession is null
            ? (await _userSessionService.GetUserSession()).LinkedAccountList
            : userSession.LinkedAccountList;

        /*      Active Account Session      */

        /// <summary>
        /// Retrieves the current active session inside the user session as a
        /// <see cref="LinkedAccount"/> object.
        /// </summary>
        /// <param name="userSession"></param>
        /// <returns></returns>
        public async Task<LinkedAccount> GetActiveAccountSessionAsync(UserSession? userSession = null)
        => userSession is null
            ? (await _userSessionService.GetUserSession()).ActiveAccountSession
            : userSession.ActiveAccountSession;

        /// <summary>
        /// Replaces the active account session in the user session and updates the user session.
        /// </summary>
        /// <param name="activeAccountSession"></param>
        /// <param name="userSession"></param>
        /// <returns></returns>
        public async Task SetActiveAccountSessionAsync(LinkedAccount? activeAccountSession = null, UserSession? userSession = null)
        {
            if (userSession is null)
                userSession = await _userSessionService.GetUserSession();

            if (activeAccountSession is null)
            {
                //int accountId = userSession.UserAccountIdList[0];
                //Account account = await _userDataService.GetAccountAsync(accountId);
                //activeAccountSession = CreateAccountSession(account);
                activeAccountSession = userSession.LinkedAccountList[0];
            }
            userSession.ActiveAccountSession = SetAccountPermissions(activeAccountSession);
            await _userSessionService.UpdateUserSession(userSession);
        }
        /// <summary>
        /// Used when logging in.
        /// Creates a new active account session.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public LinkedAccount CreateAccountSession(Account account)
        {
            LinkedAccount activeAccountSession = new LinkedAccount
            {

                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountNumber = _dataMaskingService.MaskAccountNumber(account.AccountNumber),
                AccountStatusId = account.AccountStatusTypeId
            };

            return SetAccountPermissions(activeAccountSession);
        }
        #endregion

        #region Transaction Session
        /*      Transaction Session     */

        /// <summary>
        /// Used if you have not previously called <see cref="UserSessionService.GetUserSession"/>.
        /// Gets the <<see cref="TransactionSession"/>> object from the <see cref="UserSession"> object.
        /// </summary>
        /// <param name="userSession">
        /// The <see cref="UserSession"/> object.
        /// </param>
        /// <returns></returns>
        /// <exception cref="ControlledSessionNotFound">
        /// Thrown when there is no existing transaction session. 
        /// </exception>
        public async Task<TransactionSession> GetTransactionSessionAsync(UserSession? userSession = null)
        {
            if (userSession is null)
                userSession = await _userSessionService.GetUserSession();
            return userSession.TransactionSession ?? throw new ControlledSessionNotFound(SessionSchemes.USER_SESSION, userSession.TransactionSessionScheme);
        }

        /// <summary>
        /// Clears the current transaction session.
        /// </summary>
        /// <param name="userSession"
        /// The <see cref="UserSession"/> object.
        /// ></param>
        /// <returns></returns>
        public async Task ClearTransactionSessionAsync(UserSession? userSession = null)
            => await SetTransactionSessionAsync(0, transactionSession: null, userSession);

        /// <summary>
        /// Sets a new <see cref="TransactionSession"/> inside the current
        /// <see cref="UserSession"/> and updates the <see cref="UserSession"/> in
        /// ProtectedSessionStorage.
        /// </summary>
        /// <param name="transactionTypeId"></param>
        /// <param name="transactionSession"></param>
        /// <param name="userSession"></param>
        /// <returns></returns>
        public async Task SetTransactionSessionAsync(int transactionTypeId, TransactionSession? transactionSession, UserSession? userSession = null)
        {
            if (userSession is null)
                userSession = await _userSessionService.GetUserSession();
            var transactionSessionScheme = GetTransactionSessionScheme(transactionTypeId);
            userSession.TransactionSessionScheme = transactionSessionScheme;
            userSession.TransactionSession = transactionSession;
            await _userSessionService.UpdateUserSession(userSession);
        }

        #endregion

        #region Loan Session
        public async Task<LoanApplication> GetLoanApplicationSessionAsync(UserSession? userSession = null)
        {
            if (userSession is null)
                userSession = await _userSessionService.GetUserSession();

            return userSession.LoanApplication
                ?? throw new ControlledSessionNotFound(
                    SessionSchemes.USER_SESSION, 
                    SessionSchemes.LOAN_APPLICATION_SESSION
                );
        }
        public async Task ClearLoanApplicationSessionAsync(UserSession? userSession = null) =>
            await SetLoanApplicationSessionAsync(null, userSession);
        public async Task SetLoanApplicationSessionAsync(LoanApplication? loanApplication, UserSession? userSession = null)
        {
            if (userSession is null)
                userSession = await _userSessionService.GetUserSession();

            //  Set session scheme.
            if (loanApplication is not null)
                loanApplication.ControlledSessionScheme = SessionSchemes.LOAN_APPLICATION_SESSION;

            userSession.LoanApplication = loanApplication;
            await _userSessionService.UpdateUserSession(userSession);
        }
        #endregion

        #region Helper Methods
        /*      Helper Methods      */

        private string GetTransactionSessionScheme(int transactionTypeId) => transactionTypeId switch
        {
            (int)TransactionTypeIDs.Deposit => SessionSchemes.DEPOSIT_SESSION,
            (int)TransactionTypeIDs.Withdrawal => SessionSchemes.WITHDRAW_SESSION,
            (int)TransactionTypeIDs.Incoming_Transfer => "",   // incomingtransfer session
            (int)TransactionTypeIDs.Outgoing_Transfer => "",      // outgoing transfer session
            _ => string.Empty
        };

        private LinkedAccount SetAccountPermissions(LinkedAccount activeAccountSession)
        {
            activeAccountSession.AccountCanTransact = activeAccountSession.AccountStatusId switch
            {
                (int)AccountStatusTypeIDs.Active => true,
                (int)AccountStatusTypeIDs.Restricted => true,
                _ => false
            };

            activeAccountSession.AccountCanApplyLoan = activeAccountSession.AccountStatusId switch
            {
                (int)AccountStatusTypeIDs.Active => true,
                _ => false
            };

            activeAccountSession.AccountCanPayLoan = activeAccountSession.AccountStatusId switch
            {
                (int)AccountStatusTypeIDs.Pending => false,
                (int)AccountStatusTypeIDs.Closed => false,
                (int)AccountStatusTypeIDs.Denied => false,
                _ => true
            };

            return activeAccountSession;
        }

        #endregion
    }
}
