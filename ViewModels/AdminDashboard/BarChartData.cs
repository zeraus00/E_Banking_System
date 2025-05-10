using Data.Constants;

namespace ViewModels.AdminDashboard
{
    public class BarChartData : IChartData
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public List<Bar> Bars { get; set; } = new();
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
            new(){Label = TransactionTypes.OUTGOING_TRANSFER },
            new(){Label = TransactionTypes.LOAN_PAYMENT }
        };
    }
}
