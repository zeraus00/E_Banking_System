namespace Data.Constants
{
    public class AccountProductTypeNames
    {
        public const string SAVINGS = "Savings";
        public const string CHECKING = "Checking";
        public const string LOAN = "Loan";

        public readonly List<string> AccountProductTypeNameList = new()
        {
            SAVINGS,
            CHECKING,
            LOAN
        };
    }
}
