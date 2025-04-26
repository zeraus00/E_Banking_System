namespace Data.Constants
{
    public class AccountTypeNames
    {
        public const string PersonalAccount = "Personal Account";
        public const string JointAccount = "Joint Account";

        public readonly List<string> AccountTypeNameList = new()
        {
            PersonalAccount,
            JointAccount
        };
    }
}
