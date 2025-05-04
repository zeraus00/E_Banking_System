using ViewModels.RoleControlledSessions;

namespace ViewModels.Sessions
{
    public class UserSession : SessionModel
    {
        /* username
         * contactno
         * account no
         * account name
         * account ids
         */
        public int UserAuthId { get; set; }
        public int UserInfoId { get; set; }
        public string CurrentUserName { get; set; } = string.Empty;
        public string CurrentUserEmail { get; set; } = string.Empty;
        public string CurrentUserContact { get; set; } = string.Empty;

        //  For user account sessions.
        public List<LinkedAccount> LinkedAccountList { get; set; } = new();
        public LinkedAccount ActiveAccountSession { get; set; } = new();

        //  For transaction session.
        public string TransactionSessionScheme { get; set; } = string.Empty;
        public TransactionSession? TransactionSession { get; set; } = null;
    }
}
