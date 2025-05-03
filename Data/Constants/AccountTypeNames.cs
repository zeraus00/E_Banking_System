namespace Data.Constants
{
    public class AccountTypeNames
    {
        public const string PERSONAL_ACCOUNT = "PERSONAL ACCOUNT";
        public const string JOINT_ACCOUNT = "JOINT ACCOUNT";

        public readonly List<string> AccountTypeNameList = new()
        {
            PERSONAL_ACCOUNT,
            JOINT_ACCOUNT
        };

        public static List<string> AS_STRING_LIST { get; } = new()
        {
            PERSONAL_ACCOUNT,
            JOINT_ACCOUNT
        };

        public static List<AccountType> AS_ACCOUNT_TYPE_LIST { get; } = new()
        {
            new AccountType { AccountTypeName = PERSONAL_ACCOUNT },
            new AccountType { AccountTypeName = JOINT_ACCOUNT }
        };
    }
}
