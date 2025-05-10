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

        public async Task<PendingAccountSession> GetPendingAccountSession(AdminSession? adminSession = null)
        {
            if (adminSession is null)
                adminSession = await _userSessionService.GetAdminSession();
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
            if (adminSession is null)
                adminSession = await _userSessionService.GetAdminSession();

            adminSession.PendingAccountSession = pendingAccountSession;

            await _userSessionService.UpdateAdminSession(adminSession);
        }

        public async Task<LinkedAccount> GetAccountViewSession(AdminSession? adminSession = null)
        {
            if (adminSession is null)
                adminSession = await _userSessionService.GetAdminSession();
            return adminSession.AccountViewSession
                ?? throw new ControlledSessionNotFound(
                    SessionSchemes.ADMIN_SESSION,
                    SessionSchemes.ACCOUNT_VIEW_SESSION
                );
        }

        public async Task ClearAccountViewSession(AdminSession? adminSession = null) =>
            await SetAccountViewSession(null, adminSession);

        public async Task SetAccountViewSession(LinkedAccount? accountViewSession, AdminSession? adminSession = null)
        {
            if (adminSession is null)
                adminSession = await _userSessionService.GetAdminSession();
            
            adminSession.AccountViewSession = accountViewSession;

            await _userSessionService.UpdateAdminSession(adminSession);
        }
    }
}
