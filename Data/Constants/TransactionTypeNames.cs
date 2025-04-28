namespace Data.Constants
{
    public class TransactionTypeNames
    {
        public const string DEPOSIT = "DEPOSIT";
        public const string WITHDRAWAL = "WITHDRAWAL";
        public const string INCOMING_TRANSFER = "INCOMING_TRANSFER";
        public const string OUTGOING_TRANSFER = "OUTGOING_TRANSFER";

        public static List<string> TransactionTypeNameList { get; set; } = new List<string>
        {
            DEPOSIT,
            WITHDRAWAL,
            INCOMING_TRANSFER,
            OUTGOING_TRANSFER
        };
    }
}
