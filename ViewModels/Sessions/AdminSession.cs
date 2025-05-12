using ViewModels.RoleControlledSessions;

namespace ViewModels.Sessions
{
    public class AdminSession : SessionModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public PendingAccountSession? PendingAccountSession { get; set; } = null;
        public AccountViewSession? AccountViewSession { get; set; } = null;
        public LoanViewSession? LoanViewSession { get; set; } = null;
    }
}
