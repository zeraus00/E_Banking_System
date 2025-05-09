namespace Data.Constants
{
    public class AdminDashboardTimeFilters
    {
        public const string HOURLY = "hourly";
        public const string DAILY = "daily";
        public const string WEEKLY = "weekly";
        public const string MONTHLY = "monthly";
        public const string YEARLY = "yearly";

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
