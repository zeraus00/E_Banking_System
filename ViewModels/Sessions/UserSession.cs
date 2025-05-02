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
        public int ActiveAccountId { get; set; }
        public string ActiveAccountNumber { get; set; } = string.Empty;
        public string ActiveAccountName { get; set; } = string.Empty;
        public int ActiveAccountStatusId { get; set; } 
        public List<int> UserAccountIdList { get; set; } = new List<int>();
        public string TransactionSessionScheme { get; set; } = string.Empty;
        public TransactionSession? TransactionSession { get; set; } = null;
    }
}
