namespace ViewModels.RoleControlledSessions
{
    public class LoanViewSession : RoleControlledSession
    {
        public int LoanId { get; set; }
        public int UserInfoId { get; set; }
        public int AccountId { get; set; }
    }
}
