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
        /*      User Account List       */
        public async Task<List<LinkedAccount>> GetUserAccountListAsync(UserSession? userSession = null)
        => userSession is null
            ? (await _userSessionService.GetUserSession()).LinkedAccountList
            : userSession.LinkedAccountList;

        /*      Active Account Session      */
        public async Task<LinkedAccount> GetActiveAccountSessionAsync(UserSession? userSession = null)
        => userSession is null
            ? (await _userSessionService.GetUserSession()).ActiveAccountSession
            : userSession.ActiveAccountSession;

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

        /*      Transaction Session     */
        public async Task<TransactionSession> GetTransactionSessionAsync(UserSession? userSession = null)
        {
            if (userSession is null)
                userSession = await _userSessionService.GetUserSession();
            return userSession.TransactionSession ?? throw new ControlledSessionNotFound(SessionSchemes.USER_SESSION, userSession.TransactionSessionScheme);
        }

        public async Task ClearTransactionSessionAsync(UserSession? userSession = null)
            => await SetTransactionSessionAsync(0, transactionSession: null, userSession);

        public async Task SetTransactionSessionAsync(int transactionTypeId, TransactionSession? transactionSession, UserSession? userSession = null)
        {
            if (userSession is null)
                userSession = await _userSessionService.GetUserSession();
            var transactionSessionScheme = GetTransactionSessionScheme(transactionTypeId);
            userSession.TransactionSessionScheme = transactionSessionScheme;
            userSession.TransactionSession = transactionSession;
            await _userSessionService.UpdateUserSession(userSession);
        }


        /*      Helper Methods      */

        private string GetTransactionSessionScheme(int transactionTypeId) => transactionTypeId switch
        {
            (int)TransactionTypes.Deposit => SessionSchemes.DEPOSIT_SESSION,
            (int)TransactionTypes.Withdrawal => SessionSchemes.WITHDRAW_SESSION,
            (int)TransactionTypes.Incoming_Transfer => "",   // incomingtransfer session
            (int)TransactionTypes.Outgoing_Transfer => "",      // outgoing transfer session
            _ => string.Empty
        };

        private LinkedAccount SetAccountPermissions(LinkedAccount activeAccountSession)
        {
            activeAccountSession.AccountCanTransact = activeAccountSession.AccountStatusId switch
            {
                (int)AccountStatusTypes.Active => true,
                (int)AccountStatusTypes.Restricted => true,
                _ => false
            };

            activeAccountSession.AccountCanApplyLoan = activeAccountSession.AccountStatusId switch
            {
                (int)AccountStatusTypes.Active => true,
                _ => false
            };

            activeAccountSession.AccountCanPayLoan = activeAccountSession.AccountStatusId switch
            {
                (int)AccountStatusTypes.Pending => false,
                (int)AccountStatusTypes.Closed => false,
                (int)AccountStatusTypes.Denied => false,
                _ => true
            };

            return activeAccountSession;
        }
    }
}
