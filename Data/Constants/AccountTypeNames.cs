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
    }
}
