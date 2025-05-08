namespace Data.Constants
{
    public class TransactionTypes
    {
        public const string DEPOSIT = "DEPOSIT";
        public const string WITHDRAWAL = "WITHDRAWAL";
        public const string INCOMING_TRANSFER = "INCOMING_TRANSFER";
        public const string OUTGOING_TRANSFER = "OUTGOING_TRANSFER";

        public static List<string> AS_STRING_LIST { get; } = new List<string>
        {
            DEPOSIT,
            WITHDRAWAL,
            INCOMING_TRANSFER,
            OUTGOING_TRANSFER
        };

        public static TransactionType DEPOSIT_TYPE { get; } = new TransactionType { 
            TransactionTypeId = 1, 
            TransactionTypeName = DEPOSIT 
        };
        public static TransactionType WITHDRAWAL_TYPE { get; } = new TransactionType { 
            TransactionTypeId = 2,
            TransactionTypeName = WITHDRAWAL 
        };
        public static TransactionType INCOMING_TRANSFER_TYPE { get; } = new TransactionType {
            TransactionTypeId = 3, 
            TransactionTypeName = INCOMING_TRANSFER
        };
        public static TransactionType OUTGOING_TRANSFER_TYPE { get; } = new TransactionType { 
            TransactionTypeId = 4,
            TransactionTypeName = OUTGOING_TRANSFER
        };


        public static List<TransactionType> AS_TRANSACTION_TYPE_LIST { get; } = new List<TransactionType>
        {
            DEPOSIT_TYPE,
            WITHDRAWAL_TYPE,
            INCOMING_TRANSFER_TYPE,
            OUTGOING_TRANSFER_TYPE
        };
    }
}
