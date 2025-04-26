namespace ViewModels
{
    public class TransactionSession
    {
        public int TransactionTypeId { get; set; }
        public string TransactionNumber { get; set; } = string.Empty;
        public int MainAccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrentBalance { get; set; }
        public string? ConfirmationNumber { get; set; } = null;
        public int? CounterAccountId { get; set; } = null;

    }
}
