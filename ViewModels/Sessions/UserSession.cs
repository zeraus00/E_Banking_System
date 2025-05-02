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
        public string CurrentUserName { get; set; } = string.Empty;
        public string CurrentUserEmail { get; set; } = string.Empty;
        public string CurrentUserContact { get; set; } = string.Empty;
        public List<int> UserAccountIdList { get; set; } = new List<int>();
        public ActiveAccountSession ActiveAccountSession { get; set; } = new();
        public string TransactionSessionScheme { get; set; } = string.Empty;
        public TransactionSession? TransactionSession { get; set; } = null;
    }
}
