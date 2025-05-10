namespace ViewModels.AdminDashboard
{
    public class ChartData : IChartData
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; } = 0.0m;
    }
}
