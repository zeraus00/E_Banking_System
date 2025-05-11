namespace Data.Constants
{
    public class AdminDashboardTimeFilters
    {
        public const string HOURLY = "Hour";
        public const string DAILY = "Day";
        public const string WEEKLY = "Week";
        public const string MONTHLY = "Month";
        public const string YEARLY = "Year";

        public static readonly List<string> AS_STRING_LIST = new()
        {
            HOURLY,
            DAILY,
            WEEKLY,
            MONTHLY,
            YEARLY
        };
    }
}
