namespace ViewModels.AdminDashboard
{
    public class TransactionBreakdown
    {
        public int Count { get; set; }
        public decimal Total { get; set; } = 0.00m;
        public decimal Average { get; set; } = 0.00m;
        public string Notes { get; set; } = string.Empty;

    }
}
