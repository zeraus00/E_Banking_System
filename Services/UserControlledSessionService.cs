using Data.Constants;
using Data.Enums;
using Exceptions;
using ViewModels.RoleControlledSessions;
using ViewModels.Sessions;
using Services.DataManagement;

namespace Services
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

        public async Task<ActiveAccountSession> GetActiveAccountSessionAsync(UserSession? userSession = null)
        => userSession is null
            ? (await _userSessionService.GetUserSession()).ActiveAccountSession
            : userSession.ActiveAccountSession;

        public async Task SetActiveAccountSessionAsync(ActiveAccountSession? activeAccountSession=null, UserSession? userSession = null)
        {
            if (userSession is null)
                userSession = await _userSessionService.GetUserSession();
            
            if (activeAccountSession is null)
            {
                int accountId = userSession.UserAccountIdList[0];
                Account account = await _userDataService.GetAccountAsync(accountId);
                activeAccountSession = CreateActiveAccountSession(account);
            }
            userSession.ActiveAccountSession = SetAccountPermissions(activeAccountSession);
            await _userSessionService.UpdateUserSession(userSession);
        }
        public ActiveAccountSession CreateActiveAccountSession(Account account)
        {
            ActiveAccountSession activeAccountSession = new ActiveAccountSession
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountNumber = _dataMaskingService.MaskAccountNumber(account.AccountNumber),
                AccountStatusId = account.AccountStatusTypeId
            };

            return SetAccountPermissions(activeAccountSession);
        }
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
            var transactionSessionScheme = GetControlledSessionScheme(transactionTypeId);
            userSession.TransactionSessionScheme = transactionSessionScheme;
            userSession.TransactionSession = transactionSession;
            await _userSessionService.UpdateUserSession(userSession);
        }

        private string GetControlledSessionScheme(int transactionTypeId) => transactionTypeId switch
        {
            (int)TransactionTypes.Deposit => SessionSchemes.DEPOSIT_SESSION,
            (int)TransactionTypes.Withdrawal => SessionSchemes.WITHDRAW_SESSION,
            (int)TransactionTypes.Incoming_Transfer => "",   // incomingtransfer session
            (int)TransactionTypes.Outgoing_Transfer => "",      // outgoing transfer session
            _ => string.Empty
        };

        private ActiveAccountSession SetAccountPermissions(ActiveAccountSession activeAccountSession)
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
