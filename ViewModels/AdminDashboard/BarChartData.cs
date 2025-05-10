using Data.Constants;

namespace ViewModels.AdminDashboard
{
    public class BarChartData
    {
        public string Label { get; set; } = string.Empty;
        public List<Bar> Values { get; set; } = new();
    }

    public class Bar
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }


    public class BarChartConstants
    {
        public static readonly List<Bar> ALL_TRANSACTION_TYPES = new()
        {
            new(){Label = TransactionTypes.WITHDRAWAL },
            new(){Label = TransactionTypes.DEPOSIT },
            new(){Label = TransactionTypes.OUTGOING_TRANSFER }
        };
    }
}
