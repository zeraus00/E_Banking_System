using Data.Enums;

namespace ViewModels.RoleControlledSessions
{
    public class ActiveAccountSession : RoleControlledSession
    {
        public int AccountId { get; set; } = 0;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public int AccountStatusId { get; set; } = 0;
        public bool AccountCanTransact { get; set; } = false;
        public bool AccountCanApplyLoan { get; set; } = false;
        public bool AccountCanPayLoan { get; set; } = false;
    }
}
