using Data.Constants;
using Data.Enums;
using Exceptions;
using ViewModels.RoleControlledSessions;
using ViewModels.Sessions;

namespace Services
{
    public class UserControlledSessionService
    {
        private readonly UserSessionService _userSessionService;

        public UserControlledSessionService(UserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
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

    }
}
