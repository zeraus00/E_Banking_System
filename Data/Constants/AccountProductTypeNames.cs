namespace Data.Constants
{
    public class AccountProductTypeNames
    {
        public const string Savings = "Savings";
        public const string Checking = "Checking";
        public const string Loan = "Loan";

        public readonly List<string> AccountProductTypeNameList = new()
        {
            Savings,
            Checking,
            Loan
        };
    }
}
