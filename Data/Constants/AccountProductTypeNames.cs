namespace Data.Constants
{
    public class AccountProductTypeNames
    {
        public const string SAVINGS = "SAVINGS";
        public const string CHECKING = "CHECKING";
        public const string LOAN = "LOAN";

        public readonly List<string> AccountProductTypeNameList = new()
        {
            SAVINGS,
            CHECKING,
            LOAN
        };

        public static readonly List<string> AS_STRING_LIST = new()
        {
            SAVINGS,
            CHECKING,
            LOAN
        };

        public static AccountProductType SAVINGS_TYPE { get; } = new AccountProductType { AccountProductTypeName = SAVINGS };
        public static AccountProductType CHECKING_TYPE { get; } = new AccountProductType { AccountProductTypeName = CHECKING };
        public static AccountProductType LOAN_TYPE { get; } = new AccountProductType { AccountProductTypeName = LOAN };

        public static List<AccountProductType> AS_ACCOUNT_PRODUCT_TYPE_LIST { get; } = new()
        {
            SAVINGS_TYPE,
            CHECKING_TYPE,
            LOAN_TYPE
        };
    }
}
