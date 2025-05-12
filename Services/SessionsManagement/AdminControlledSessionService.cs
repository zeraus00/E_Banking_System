using Data.Constants;
using Exceptions;
using ViewModels.RoleControlledSessions;
using ViewModels.Sessions;

namespace Services.SessionsManagement
{
    public class AdminControlledSessionService
    {
        private readonly UserSessionService _userSessionService;

        public AdminControlledSessionService(UserSessionService userSessionService)
        {
            _userSessionService = userSessionService;
        }

        #region Pending Account Session
        public async Task<PendingAccountSession> GetPendingAccountSession(AdminSession? adminSession = null)
        {
            adminSession = await HandleNullAdminSession(adminSession);
            return adminSession.PendingAccountSession
                ?? throw new ControlledSessionNotFound(
                    SessionSchemes.ADMIN_SESSION,
                    SessionSchemes.PENDING_ACCOUNT_SESSION
                );
        }
        public async Task ClearPendingAccountSession(AdminSession? adminSession = null) => 
            await SetPendingAccountSession(null, adminSession);

        public async Task SetPendingAccountSession(PendingAccountSession? pendingAccountSession, AdminSession? adminSession = null)
        {
            adminSession = await HandleNullAdminSession(adminSession);

            adminSession.PendingAccountSession = pendingAccountSession;

            await _userSessionService.UpdateAdminSession(adminSession);
        }
        #endregion
        #region Account View Session
        public async Task<AccountViewSession> GetAccountViewSession(AdminSession? adminSession = null)
        {
            adminSession = await HandleNullAdminSession(adminSession);
            return adminSession.AccountViewSession
                ?? throw new ControlledSessionNotFound(
                    SessionSchemes.ADMIN_SESSION,
                    SessionSchemes.ACCOUNT_VIEW_SESSION
                );
        }
        public async Task ClearAccountViewSession(AdminSession? adminSession = null) =>
            await SetAccountViewSession(null, adminSession);
        public async Task SetAccountViewSession(AccountViewSession? accountViewSession, AdminSession? adminSession = null)
        {
            adminSession = await HandleNullAdminSession(adminSession);

            adminSession.AccountViewSession = accountViewSession;

            await _userSessionService.UpdateAdminSession(adminSession);
        }
        #endregion
        #region Loan View Session
        public async Task<LoanViewSession> GetLoanViewSession(AdminSession? adminSession = null)
        {
            adminSession = await HandleNullAdminSession(adminSession);
            return adminSession.LoanViewSession ??
                throw new ControlledSessionNotFound(
                    SessionSchemes.ADMIN_SESSION,
                    SessionSchemes.LOAN_VIEW_SESSION
                );
        }
        public async Task ClearLoanViewSession(AdminSession? adminSession = null) =>
            await SetLoanViewSession(null, adminSession);
        public async Task SetLoanViewSession(LoanViewSession? loanViewSession, AdminSession? adminSession = null)
        {
            adminSession = await HandleNullAdminSession(adminSession);

            adminSession.LoanViewSession = loanViewSession;

            await _userSessionService.UpdateAdminSession(adminSession);
        }
        #endregion
        public async Task ClearAdminControlledSessions(AdminSession? adminSession = null)
        {
            adminSession = await HandleNullAdminSession(adminSession);
            adminSession.PendingAccountSession = null;
            adminSession.AccountViewSession = null;
            adminSession.LoanViewSession = null;
            await _userSessionService.UpdateAdminSession(adminSession);
        }
        public async Task<AdminSession> HandleNullAdminSession(AdminSession? adminSession = null)
            => adminSession is null ?
                await _userSessionService.GetAdminSession() :
                adminSession;
    }
}
