using Data.Constants;

namespace ViewModels.AdminDashboard
{
    public class ChartData : IChartData
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; } = 0.0m;
        public List<DataUnit> DataUnits { get; set; } = new();
    }

    public class TempChartData
    {
        public DateTime Label { get; set; }
        public decimal Value { get; set; } = 0.0m;
        public List<DataUnit> DataUnits { get; set; } = new();
    }

    public class DataUnit
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    public class ChartDataConstants
    {
        public static readonly List<DataUnit> ALL_TRANSACTION_TYPES = new()
        {
            new(){ Label = TransactionTypes.WITHDRAWAL },
            new(){ Label = TransactionTypes.DEPOSIT },
            new(){ Label = TransactionTypes.OUTGOING_TRANSFER },
            new(){ Label = TransactionTypes.INCOMING_TRANSFER},
            new(){ Label = TransactionTypes.LOAN_PAYMENT }
        };
    }
}
