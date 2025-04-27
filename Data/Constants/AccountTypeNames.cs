namespace Data.Constants
{
    public class AccountTypeNames
    {
        public const string PERSONAL_ACCOUNT = "Personal Account";
        public const string JOINT_ACCOUNT = "Joint Account";

        public readonly List<string> AccountTypeNameList = new()
        {
            PERSONAL_ACCOUNT,
            JOINT_ACCOUNT
        };
    }
}
