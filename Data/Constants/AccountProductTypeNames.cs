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
    }
}
