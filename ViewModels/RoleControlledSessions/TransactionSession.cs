namespace ViewModels.RoleControlledSessions
{
    public class TransactionSession : RoleControlledSession
    {
        public int TransactionTypeId { get; set; }
        public string TransactionNumber { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public TimeSpan TransactionTime { get; set; }
        public int MainAccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrentBalance { get; set; }
        public string? ConfirmationNumber { get; set; } = null;
        //  Transaction third party.
        public int? CounterAccountId { get; set; } = null;
        public int? ExternalVendorId { get; set; } = null;
        public string CounterAccountName { get; set; } = string.Empty;
        public string CounterAccountNumber { get; set; } = string.Empty;

    }
}
