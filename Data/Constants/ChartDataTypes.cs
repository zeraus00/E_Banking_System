namespace Data.Constants
{
    public class ChartDataTypes
    {
        public const string NET_MOVEMENT_OVER_TIME = "Net Movement Over Time";
        public const string LOANED_AMOUNT_OVER_TIME = "Loaned Amount Over Time";
        public const string RECENT_TRANSACTION_TYPES = "Recent Transaction Types";
        public const string TRANSACTION_VOLUME_OVER_TIME = "Transaction Volume Over Time";
        public const string TRANSACTION_COUNT_OVER_TIME = "Transaction Count Over Time";

        public readonly List<string> AS_STRING_LIST = new()
        {
            NET_MOVEMENT_OVER_TIME,
            LOANED_AMOUNT_OVER_TIME,
            RECENT_TRANSACTION_TYPES,
            TRANSACTION_VOLUME_OVER_TIME,
            TRANSACTION_COUNT_OVER_TIME
        };

        public readonly List<string> USER_DATA_LIST = new()
        {
            TRANSACTION_VOLUME_OVER_TIME,
            TRANSACTION_COUNT_OVER_TIME
        };
    }
}
