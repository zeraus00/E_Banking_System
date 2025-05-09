namespace ViewModels.RoleControlledSessions
{
    public class LoanApplication : RoleControlledSession
    {
        public int LoanId { get; set; }
        public int UserInfoId { get; set; }
    }
}
